using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class DoNotLogCanceledMethodsTests : TestBase
    {
        public DoNotLogCanceledMethodsTests()
        {
            GivenThereExistsAContainer()
                .WithConfiguredSerilog()
                .WithADummyTypeRegistered();
        }

        [Fact]
        public async Task ThenNoInformationMessageShouldBeLogged()
        {
            var ts = new CancellationTokenSource();
            CancellationToken ct = ts.Token;
            IDummy dummy = WhenDummyIsResolvedAnd();
            // ReSharper disable once ConvertToLocalFunction
            Func<Task> func = async () => await dummy.ReturnStuffAsync(1, "b");
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(func, ct);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            ts.Cancel();
            // ReSharper disable once MethodSupportsCancellation
            await Task.Delay(500);

            Log.Count.Should().Be(0);
        }
    }
}