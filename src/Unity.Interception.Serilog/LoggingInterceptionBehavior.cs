using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Practices.Unity.InterceptionExtension;
using Serilog;
using Serilog.Events;

namespace Unity.Interception.Serilog
{
    // ReSharper disable once ClassNeverInstantiated.Global - Unity
    internal class LoggingInterceptionBehavior : IInterceptionBehavior
    {
        private readonly ILogger _logger;
        private readonly IStopWatch _stopWatch;
        private readonly ISerilogOptions _options;

        private readonly ConcurrentDictionary<Type, Func<Task, IMethodInvocation, Task>> _wrapperCreators =
            new ConcurrentDictionary<Type, Func<Task, IMethodInvocation, Task>>();

        public LoggingInterceptionBehavior(ILogger logger, IStopWatch stopWatch, ISerilogOptions options)
        {
            _logger = logger;
            _stopWatch = stopWatch;
            _options = options;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            _stopWatch.Start();
            IMethodReturn result = getNext()(input, getNext);
            if (IgnoreMethod(input))
                return result;

            var method = input.MethodBase as MethodInfo;
            if (result.ReturnValue != null
                && method != null
                && typeof(Task).IsAssignableFrom(method.ReturnType))
            {
                // If this method returns a Task, override the original return value.
                // More info on this pattern: https://msdn.microsoft.com/en-us/magazine/dn574805.aspx
                var task = (Task)result.ReturnValue;
                return input.CreateMethodReturn(
                    GetWrapperCreator(method.ReturnType)(task, input), result.Outputs);
            }

            // If this method does not return a task, just log and return value.
            LogMethodResult(input, result.ReturnValue, result.Exception);
            return result;
        }

        private Func<Task, IMethodInvocation, Task> GetWrapperCreator(Type taskType)
        {
            return _wrapperCreators.GetOrAdd(
                taskType,
                t =>
                {
                    if (t == typeof(Task))
                    {
                        return CreateWrapperTask;
                    }
                    if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Task<>))
                    {
                        return (Func<Task, IMethodInvocation, Task>)GetType()
                            .GetMethod("CreateGenericWrapperTask", BindingFlags.Instance | BindingFlags.NonPublic)
                            .MakeGenericMethod(t.GenericTypeArguments[0])
                            .CreateDelegate(typeof(Func<Task, IMethodInvocation, Task>), this);

                    }
                    // other cases are not supported
                    throw new NotSupportedException($"Encountered an unkown type of task: {t.Name}");
                    //return (task, _) => task;
                });
        }

        private Task CreateWrapperTask(Task task, IMethodInvocation input)
        {
            var tcs = new TaskCompletionSource<bool>();

            task.ContinueWith(
                t =>
                {
                    if (t.IsFaulted)
                    {
                        if (t.Exception == null)
                            throw new NotSupportedException("Found no exception on faulted task");
                        if (t.Exception.InnerException == null)
                            throw new NotSupportedException("Found no inner exception on faulted task");
                        var e = t.Exception.InnerException;

                        LogMethodResult(input, null, e);

                        tcs.SetException(e);
                    }
                    else if (t.IsCanceled)
                    {
                        tcs.SetCanceled();
                    }
                    else
                    {
                        LogMethodResult(input, null, null);

                        tcs.SetResult(true);
                    }
                },
                TaskContinuationOptions.ExecuteSynchronously);
            return tcs.Task;
        }

        // ReSharper disable once UnusedMember.Local - Used in reflection
        private Task CreateGenericWrapperTask<T>(Task task, IMethodInvocation input)
        {
            return DoCreateGenericWrapperTask((Task<T>)task, input);
        }

        private async Task<T> DoCreateGenericWrapperTask<T>(Task<T> task, IMethodInvocation input)
        {
            try
            {
                T value = await task.ConfigureAwait(false);
                LogMethodResult(input, value, null);

                return value;
            }
            catch (Exception e)
            {
                LogMethodResult(input, null, e);
                throw;
            }
        }


        private void LogMethodResult(IMethodInvocation input, object value, Exception exception)
        {
            _stopWatch.Stop();
            var builder = new TraceLogBuilder(input, value, exception, _stopWatch.Elapsed, _logger);
            var message = builder.Build();
            var level = GetLevel(exception);
            builder.Logger.Write(level, exception, message, builder.PropertyValues);
        }

        private LogEventLevel GetLevel(Exception exception)
        {
            if (exception != null)
            {
                var expectedException = _options.ExpectedExceptions.Contains(exception.GetType());
                return expectedException ? _options.Level : LogEventLevel.Error;
            }
            return _options.Level;
        }

        private bool IgnoreMethod(IMethodInvocation input)
        {
            var c = new MethodNameConverter(input);
            bool ignoredByMethodIdentifier = _options.IgnoredMethods.Any(m => m.Type.FullName == c.SourceContext && m.MethodName == c.EventId);
            return ignoredByMethodIdentifier || input.MethodBase.CustomAttributes.ContainsIgnoreAttribute();
        }

        public IEnumerable<Type> GetRequiredInterfaces() => Type.EmptyTypes;

        public bool WillExecute => true;
    }
}