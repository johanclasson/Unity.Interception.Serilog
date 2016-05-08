
//TODO: Only Exception or Logging - LogLevel on options

//TODO: Does timespan work with kibana?
//TODO: Consider good practices at http://blachniet.com/blog/serilog-good-habits/
/*
Make message smaller, and include stuff "in the context", ie: (ILogger).ForContext("JobId", job.Id).Information
Set property SourceContext to namespace.interface, and <{EventID:l}>" to method name

public static ILogger With(this ILogger logger, string propertyName, object value)
{
    return logger.ForContext(propertyName, value, destructureObjects=true);
}
 */

 
 
 
 //TODO: Cache? Configure cache with attributes?
