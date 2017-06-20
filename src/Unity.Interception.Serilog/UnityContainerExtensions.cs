using System;
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Serilog;
using Serilog.Events;

namespace Unity.Interception.Serilog
{
    /// <summary>Extensions for the unity container, adding the capability to register types that will get automatic
    /// trace and error logging through <see cref="Interception"/>.</summary>
    public static class UnityContainerExtensions
    {
        /// <summary>Registers a Serilog <see cref="ILogger" /> instance in the <see cref="IUnityContainer"/>, and
        /// enriches the <see cref="LoggerConfiguration"/> with a set of properties. These describe the
        /// environment of the executed code, and contains ManagedThreadId, MachineName, ProcessId,
        /// ProcessName, ThreadName and AppDomainName.</summary>
        /// <param name="container">The extended container.</param>
        /// <param name="configureLogger">Used to configure the <see cref="ILogger"/>.</param>
        /// <param name="expectedExceptions">A list of exception types which will not be logged as errors but as
        /// trace logs instead.</param>
        /// <param name="ignoredMethods">A list of methods which will not be logged at all.</param>
        /// <param name="level">The event level of the trace logs.</param>
        /// <returns>The Microsoft.Practices.Unity.UnityContainer object that this method was called on (this in
        /// C#, Me in Visual Basic).</returns>
        public static IUnityContainer ConfigureSerilog(this IUnityContainer container, Action<LoggerConfiguration> configureLogger,
            Type[] expectedExceptions = null, MethodIdentifier[] ignoredMethods = null, LogEventLevel level = LogEventLevel.Debug)
        {
            var configuration = new LoggerConfiguration();
            configureLogger(configuration);
            return container.ConfigureSerilog(configuration, expectedExceptions, ignoredMethods, level);
        }

        /// <summary>Registers a Serilog <see cref="ILogger" /> instance in the <see cref="IUnityContainer"/>, and
        /// enriches the <see cref="LoggerConfiguration"/> with a set of properties. These describe the
        /// environment of the executed code, and contains ManagedThreadId, MachineName, ProcessId,
        /// ProcessName, ThreadName and AppDomainName.</summary>
        /// <param name="container">The extended container.</param>
        /// <param name="configuration">The configuration that the <see cref="ILogger"/> will be created
        /// from. It's important that the <see cref="LoggerConfiguration"/> is configured before calling this
        /// method.</param>
        /// <param name="expectedExceptions">A list of exception types which will not be logged as errors but as
        /// trace logs instead.</param>
        /// <param name="ignoredMethods">A list of methods which will not be logged at all.</param>
        /// <param name="level">The event level of the trace logs.</param>
        /// <returns>The Microsoft.Practices.Unity.UnityContainer object that this method was called on (this in
        /// C#, Me in Visual Basic).</returns>
        public static IUnityContainer ConfigureSerilog(this IUnityContainer container, LoggerConfiguration configuration,
            Type[] expectedExceptions = null, MethodIdentifier[] ignoredMethods = null, LogEventLevel level = LogEventLevel.Debug)
        {
            var properties = GetProperties(container);
            configuration.Enrich.With(new CommonPropertiesEnricher(properties));
            // CreateLogger can only be called once
            ILogger logger = configuration.CreateLogger();
            container.RegisterInstance(logger);
            container.RegisterInstance<ISerilogOptions>(new SerilogOptions(expectedExceptions, ignoredMethods, level));
            Log.Logger = logger;
            return container;
        }

        private static ICommonProperties GetProperties(IUnityContainer container)
        {
            if (!container.IsRegistered<ICommonProperties>())
            {
                container.RegisterType<ICommonProperties, CommonProperties>(new ContainerControlledLifetimeManager());
            }
            return container.Resolve<ICommonProperties>();
        }

        /// <summary>Registers a logged type mapping with the container. Logged types are enriched with properties
        /// containing SourceContext (Namespace and type name), EventId (Method name), Arguments, Result, Duration
        /// (The execution time in ms), and LogType (Set to "Trace").</summary>
        /// <typeparam name="TFrom">System.Type that will be requested.</typeparam>
        /// <typeparam name="TTo">System.Type that will actually be returned.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The Microsoft.Practices.Unity.UnityContainer object that this method was called on (this in
        /// C#, Me in Visual Basic).</returns>
        /// <remarks>This method is used to tell the container that when asked for type TFrom, actually return an
        /// instance of type TTo. This is very useful for getting instances of interfaces.
        /// This overload registers a default mapping and transient lifetime.</remarks>
        public static IUnityContainer RegisterLoggedType<TFrom, TTo>(this IUnityContainer container,
            params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            InjectionMember[] members = Init(container, injectionMembers);
            return container.RegisterType<TFrom, TTo>(members);
        }

        /// <summary>Registers a logged type mapping with the container. Logged types are enriched with properties
        /// containing SourceContext (Namespace and type name), EventId (Method name), Arguments, Result, Duration
        /// (The execution time in ms), and LogType (Set to "Trace").</summary>
        /// <typeparam name="TFrom">System.Type that will be requested.</typeparam>
        /// <typeparam name="TTo">System.Type that will actually be returned.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name of this mapping.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The Microsoft.Practices.Unity.UnityContainer object that this method was called on (this in
        /// C#, Me in Visual Basic).</returns>
        /// <remarks>This method is used to tell the container that when asked for type TFrom, actually return an
        /// instance of type TTo. This is very useful for getting instances of interfaces.
        /// This overload registers a default mapping and transient lifetime.</remarks>
        public static IUnityContainer RegisterLoggedType<TFrom, TTo>(this IUnityContainer container, string name,
            params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            InjectionMember[] members = Init(container, injectionMembers);
            return container.RegisterType<TFrom, TTo>(name, members);
        }

        /// <summary>Registers a logged type mapping with the container. Logged types are enriched with properties
        /// containing SourceContext (Namespace and type name), EventId (Method name), Arguments, Result, Duration
        /// (The execution time in ms), and LogType (Set to "Trace").</summary>
        /// <typeparam name="TFrom">System.Type that will be requested.</typeparam>
        /// <typeparam name="TTo">System.Type that will actually be returned.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime of the
        /// returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The Microsoft.Practices.Unity.UnityContainer object that this method was called on (this in
        /// C#, Me in Visual Basic).</returns>
        public static IUnityContainer RegisterLoggedType<TFrom, TTo>(this IUnityContainer container,
            LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            InjectionMember[] members = Init(container, injectionMembers);
            return container.RegisterType<TFrom, TTo>(lifetimeManager, members);
        }

        /// <summary>Registers a logged type mapping with the container. Logged types are enriched with properties
        /// containing SourceContext (Namespace and type name), EventId (Method name), Arguments, Result, Duration
        /// (The execution time in ms), and LogType (Set to "Trace").</summary>
        /// <typeparam name="TFrom">System.Type that will be requested.</typeparam>
        /// <typeparam name="TTo">System.Type that will actually be returned.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name of this mapping.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime of the
        /// returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The Microsoft.Practices.Unity.UnityContainer object that this method was called on (this in
        /// C#, Me in Visual Basic).</returns>
        public static IUnityContainer RegisterLoggedType<TFrom, TTo>(this IUnityContainer container, string name,
            LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            InjectionMember[] members = Init(container, injectionMembers);
            return container.RegisterType<TFrom, TTo>(name, lifetimeManager, members);
        }

        /// <summary>Registers a logged type mapping with the container. Logged types are enriched with properties
        /// containing SourceContext (Namespace and type name), EventId (Method name), Arguments, Result, Duration
        /// (The execution time in ms), and LogType (Set to "Trace").</summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="instance">The mapped type instance.</param>
        /// <returns>The Microsoft.Practices.Unity.UnityContainer object that this method was called on (this in
        /// C#, Me in Visual Basic).</returns>
        public static IUnityContainer RegisterLoggedInstance<TInterface>(this IUnityContainer container,
            TInterface instance)
        {
            InjectionMember[] members = InitForInstance(container, instance);
            return container.RegisterType<TInterface>(members);
        }

        /// <summary>Registers a logged type mapping with the container. Logged types are enriched with properties
        /// containing SourceContext (Namespace and type name), EventId (Method name), Arguments, Result, Duration
        /// (The execution time in ms), and LogType (Set to "Trace").</summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="instance">The mapped type instance.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime of the
        /// returned instance.</param>
        /// <returns>The Microsoft.Practices.Unity.UnityContainer object that this method was called on (this in
        /// C#, Me in Visual Basic).</returns>
        public static IUnityContainer RegisterLoggedInstance<TInterface>(this IUnityContainer container,
            TInterface instance, LifetimeManager lifetimeManager)
        {
            InjectionMember[] members = InitForInstance(container, instance);
            return container.RegisterType<TInterface>(lifetimeManager, members);
        }

        /// <summary>Registers a logged type mapping with the container. Logged types are enriched with properties
        /// containing SourceContext (Namespace and type name), EventId (Method name), Arguments, Result, Duration
        /// (The execution time in ms), and LogType (Set to "Trace").</summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name of this mapping.</param>
        /// <param name="instance">The mapped type instance.</param>
        /// <returns>The Microsoft.Practices.Unity.UnityContainer object that this method was called on (this in
        /// C#, Me in Visual Basic).</returns>
        public static IUnityContainer RegisterLoggedInstance<TInterface>(this IUnityContainer container, string name,
            TInterface instance)
        {
            InjectionMember[] members = InitForInstance(container, instance);
            return container.RegisterType<TInterface>(name, members);
        }

        /// <summary>Registers a logged type mapping with the container. Logged types are enriched with properties
        /// containing SourceContext (Namespace and type name), EventId (Method name), Arguments, Result, Duration
        /// (The execution time in ms), and LogType (Set to "Trace").</summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name of this mapping.</param>
        /// <param name="instance">The mapped type instance.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime of the
        /// returned instance.</param>
        /// <returns>The Microsoft.Practices.Unity.UnityContainer object that this method was called on (this in
        /// C#, Me in Visual Basic).</returns>
        public static IUnityContainer RegisterLoggedInstance<TInterface>(this IUnityContainer container, string name,
            TInterface instance, LifetimeManager lifetimeManager)
        {
            InjectionMember[] members = InitForInstance(container, instance);
            return container.RegisterType<TInterface>(name, lifetimeManager, members);
        }

        #region Helpers

        private static void Init(this IUnityContainer container)
        {
            if (container.Configure<Microsoft.Practices.Unity.InterceptionExtension.Interception>() == null)
                container.AddNewExtension<Microsoft.Practices.Unity.InterceptionExtension.Interception>();
            if (!container.IsRegistered<IStopWatch>())
                container.RegisterType<IStopWatch, Stopwatch>();
        }

        private static InjectionMember[] Init(IUnityContainer container, params InjectionMember[] injectionMembers)
        {
            container.Init();
            InjectionMember[] loggingMembers =
            {
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<LoggingInterceptionBehavior>()
            };
            return injectionMembers.Concat(loggingMembers).ToArray();
        }

        private static InjectionMember[] InitForInstance<TInterface>(IUnityContainer container, TInterface instance)
        {
            return Init(container, new InjectionFactory(c => instance));
        }

        #endregion
    }
}