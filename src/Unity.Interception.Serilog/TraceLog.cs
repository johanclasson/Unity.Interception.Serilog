using System.Collections.Generic;
using System.Data;

namespace Unity.Interception.Serilog
{
    public static class TraceLog
    {
        public static ICollection<DataColumn> DataColumns => new[]
        {
            new DataColumn {DataType = typeof(string), ColumnName = "SourceContext", MaxLength = 250},
            new DataColumn {DataType = typeof(string), ColumnName = "EventId", MaxLength = 250},
            new DataColumn {DataType = typeof(int), ColumnName = "ManagedThreadId"},
            new DataColumn {DataType = typeof(string), ColumnName = "MachineName", MaxLength = 125},
            new DataColumn {DataType = typeof(int), ColumnName = "ProcessId"},
            new DataColumn {DataType = typeof(string), ColumnName = "ProcessName", MaxLength = 250},
            new DataColumn {DataType = typeof(string), ColumnName = "ThreadName", MaxLength = 250},
            new DataColumn {DataType = typeof(string), ColumnName = "AppDomainName", MaxLength = 250},
            new DataColumn {DataType = typeof(string), ColumnName = "Result"},
            new DataColumn {DataType = typeof(double), ColumnName = "Duration"},
            new DataColumn {DataType = typeof(string), ColumnName = "ExceptionType", MaxLength = 250},
            new DataColumn {DataType = typeof(string), ColumnName = "LogType", MaxLength = 10}
        };
    }
}