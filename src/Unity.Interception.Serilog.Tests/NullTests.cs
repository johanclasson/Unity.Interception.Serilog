using System;
using FluentAssertions;
using Microsoft.Practices.Unity;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class NullTests
    {
        [Fact]
        public void NullMembersShouldThrow()
        {
            var container = new UnityContainer();
            Action[] actions = {
                () => container.RegisterLoggedType<IDummy, Dummy>((InjectionMember[])null),
                () => container.RegisterLoggedType<IDummy, Dummy>("", (InjectionMember[])null),
                () => container.RegisterLoggedType<IDummy, Dummy>(new ContainerControlledLifetimeManager(), null),
                () => container.RegisterLoggedType<IDummy, Dummy>("", new ContainerControlledLifetimeManager(), null)
            };
            foreach (var action in actions)
            {
                action.ShouldThrow<ArgumentNullException>();
            }
        }

        [Fact]
        public void NullContainerShouldThrow()
        {
            Action[] actions = {
                () => UnityContainerExtensions.RegisterLoggedType<IDummy, Dummy>(null),
                () => UnityContainerExtensions.RegisterLoggedType<IDummy, Dummy>(null, ""),
                () => UnityContainerExtensions.RegisterLoggedType<IDummy, Dummy>(null, new ContainerControlledLifetimeManager()),
                () => UnityContainerExtensions.RegisterLoggedType<IDummy, Dummy>(null, "", new ContainerControlledLifetimeManager())
            };
            foreach (var action in actions)
            {
                action.ShouldThrow<ArgumentNullException>();
            }
        }
    }
}