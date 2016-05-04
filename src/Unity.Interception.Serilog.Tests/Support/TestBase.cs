using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;

namespace Unity.Interception.Serilog.Tests.Support
{
    public abstract class TestBase : IDisposable
    {
        private ContainerBuilder _builder;

        protected Dictionary<string, List<LogEntry>> Log => _builder.Log;

        protected ContainerBuilder GivenThereExistsAContainer()
        {
            _builder = new ContainerBuilder();
            return _builder;
        }

        protected IDummy WhenDummyIsResolvedAnd()
        {
            return _builder.Container.Resolve<IDummy>();
        }

        public void Dispose()
        {
            _builder?.Dispose();
        }
    }
}