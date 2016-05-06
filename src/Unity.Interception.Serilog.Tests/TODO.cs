//TODO: Cache? Configure cache with attributes?
//TODO: Allow configuring logging through registered helper in container. That would allow to configure stuff without the need to reference a dll.
//TODO: Make a IUnityContainer.ConfigureSerilog extension method. Easier to use and a nice place to inject configuration.
//TODO: Only Exception or Logging

//TODO: Ignore logging errors for certain exceptions - Try filtering on "fields.Method" and "level"
/*
Consider:
container.Configure(LoggerConfiguration config, IEnumerable<Type> excludeErrorsFrom) {
    config.Filter.ByExcluding(e => Matching.WithProperty<string>("ExceptionType", t => excludeErrorsFrom.Select(...).Any(s => t == s)(e) && ...)
}
Exception type behöver finnas som fält då!
 */
//TODO: Enrich logs with thread id - https://github.com/serilog/serilog/wiki/Configuration-Basics - Properties that are not mentioned in the message is not logged!
/*
 * Or just include it with all logs to begin with... Together with Computername. Any more?
 */
