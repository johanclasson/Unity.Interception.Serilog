# Unity.Interception.Serilog ![NuGet Status](https://img.shields.io/nuget/v/Unity.Interception.Serilog.svg?style=flat-square) ![Build Status](https://johanclasson.visualstudio.com/_apis/public/build/definitions/888f828d-d1a4-42fb-8b78-2e6420b1b2f8/14/badge)

Types which are registered to the Unity Container with the `RegisterLoggedType` extension method are decorated with a logging behavior. The logs which are produced contains information about the respective types method invocations such as execution time, arguments and result, and are persisted though the Serilog ILogger-adapter.

In addition, all logging through the configured ILogger instance are enriched with properties describing the environment where the logging event occurred.

![Unity.Interception.Serilog](doc/overview.png)

*Overview of Unity.Interception.Serilog*

## TL;DR

### Install NuGet-packages

```powershell
Install-Package Unity.Interception.Serilog
Install-Package Serilog.Sinks.Literate # Substitute this for your favorite Sink
```

### Sample Application

```cs
public interface IDummy
{
    void DoStuff();
}

internal class Dummy : IDummy
{
    public void DoStuff() { }
}

internal class Program
{
    public static void Main(string[] args)
    {
        using (var container = new UnityContainer())
        {
            container
                .ConfigureSerilog(c => c.MinimumLevel.Debug().WriteTo.LiterateConsole())
                .RegisterLoggedType<IDummy, Dummy>();
            Log.Logger.Information("Application starting"); // Event log example
            var dummy = container.Resolve<IDummy>();
            dummy.DoStuff(); // Method call is logged
            container.Resolve<ILogger>().Information("Application finished"); // Event log example
        }
    }
}
```

## Configuration

The logging is set up by calling the `ConfigureSerilog` extension method on a container, and either pass in a `LoggerConfiguration` instance or use the overload with an `Action` parameter where the `LoggerConfiguration` is created for you.

The `ConfigureSerilog` method registers an `ILogger` instance in the container, so it's important that the `LoggerConfiguration` is configured before.

Thrown exceptions are logged as errors by default, but it is possible to configure a list of expected exceptions which are then logged at the event level of trace logs instead.

It is also possible to ignore methods from being logged at all, by passing a list of `MethodIdentifier`s. As an alternative to configure ignored methods though `ConfigureSerilog`, you can use the `IgnoreMember` attribute. It is not only applicable to methods, but parameters as well.

```cs
public interface IMyType
{
    [IgnoreMember]
    void DoSecretStuff();

    void DoStuffWithSecretParameter(string username, [IgnoreMember] string password);
}

```

The event level of trace logs can be set through the level parameter. If it is not specified, the level defaults to `Debug`. You have to specify the minimum level on the `LoggerConfiguration` instance yourself.

Serilog's default minimum level is set to `Information`. Because of that only `Error` logs will be written to your sink if you do not specify the log level.

The configuration could be done like the following example.

```cs
var container = new UnityContainer();
var expectedExceptions = new[]
{
    typeof(MyException)
};
var ignoredMethods = new[]
{
    new MethodIdentifier(typeof(IMyType), nameof(IMyType.DoStuff))
};
var level = LogEventLevel.Information;
container
    .ConfigureSerilog(c => c.MinimumLevel.Debug().WriteTo.Xyz(...),
                      expectedExceptions, ignoredMethods, level);
``` 

It is optional whether to pass the parameters `expectedExceptions`, `ignoredMethods` and `level` or not.

### Type Registration

To enable logging for a certain type use one of the extension methods:

```cs
var container = new UnityContainer();
container
    .RegisterLoggedType<IMyType, MyType>();
    .RegisterLoggedInstance<IMyType>(new MyType());
```

## Logging

All logs that is made through the `ILogger` that is registered in the container are enriched with the properties that are mentioned in [CommonProperties.cs](src/Unity.Interception.Serilog/CommonProperties.cs).

### Trace Logs

Except from the common properties, the following is also logged for trace logs:

* `SourceContext`: Namespace and type name
* `EventId`: Method name.
* `Arguments`: A list of argument names and values.
* `Result`: The result, if the method produces such a thing.
* `Duration`: The execution time in ms.
* `LogType`: Is set to `Trace`. For example, this might be used to distinguish trace logs from event logs.

Due to performance reasons, arguments and results are not serialized as objects, but captured with `ToString()`.

The properties `Exception` and `ExceptionType` are included for logs for thrown exceptions. 

### Event Logs

There is no built in support for formatting event logs like the trace logs. You will have to write your own helper for that. For example:

```cs
public static void Information<T>(
    this ILogger logger, string eventId,
    string messageTemplate, params object[] propertyValues)
{
    messageTemplate = "[{SourceContext}.{EventId}] " + messageTemplate;
    logger
        .ForContext<T>()
        .ForContext("EventId", eventId)
        .ForContext("LogType", "Event")
        .Information(messageTemplate, propertyValues);
}
```

### Serilog.Sinks.MSSqlServer Example

There is a `TraceLog` helper class that is useful together with the SQL Server sink. For example, you could configure Serilog like so:

```cs
var columnOptions = new ColumnOptions { AdditionalDataColumns = TraceLog.DataColumns };
columnOptions.Properties.ExcludeAdditionalProperties = true;
var container = new UnityContainer()
    .ConfigureSerilog(c => c.WriteTo.MSSqlServer(
        "Server=.;Database=SerilogTest;Trusted_Connection=True;", "Log",
        autoCreateSqlTable: true, columnOptions: columnOptions));
```

## Links

* [Serilog Wiki](https://github.com/serilog/serilog/wiki/Getting-Started)
* [Patterns & Practices - Unity.Interception](https://msdn.microsoft.com/en-us/library/dn178466.aspx)
