﻿using System;
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Unity.Interception.Serilog
{
    public static class UnityContainerExtensions
    {
        public static IUnityContainer RegisterLoggedType<TFrom, TTo>(this IUnityContainer container,
            params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            InjectionMember[] members = GetMembers(container, injectionMembers);
            return container.RegisterType<TFrom, TTo>(members);
        }

        public static IUnityContainer RegisterLoggedType<TFrom, TTo>(this IUnityContainer container, string name,
            params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            InjectionMember[] members = GetMembers(container, injectionMembers);
            return container.RegisterType<TFrom, TTo>(name, members);
        }

        public static IUnityContainer RegisterLoggedType<TFrom, TTo>(this IUnityContainer container,
            LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            InjectionMember[] members = GetMembers(container, injectionMembers);
            return container.RegisterType<TFrom, TTo>(lifetimeManager, members);
        }

        public static IUnityContainer RegisterLoggedType<TFrom, TTo>(this IUnityContainer container, string name,
            LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            InjectionMember[] members = GetMembers(container, injectionMembers);
            return container.RegisterType<TFrom, TTo>(name, lifetimeManager, members);
        }

        public static IUnityContainer RegisterLoggedInstance<TInterface>(this IUnityContainer container,
            TInterface instance)
        {
            InjectionMember[] members = GetInstanceMembers(container, instance);
            return container.RegisterType<TInterface>(members);
        }

        public static IUnityContainer RegisterLoggedInstance<TInterface>(this IUnityContainer container,
            TInterface instance, LifetimeManager lifetimeManager)
        {
            InjectionMember[] members = GetInstanceMembers(container, instance);
            return container.RegisterType<TInterface>(lifetimeManager, members);
        }

        public static IUnityContainer RegisterLoggedInstance<TInterface>(this IUnityContainer container, string name,
            TInterface instance)
        {
            InjectionMember[] members = GetInstanceMembers(container, instance);
            return container.RegisterType<TInterface>(name, members);
        }

        public static IUnityContainer RegisterLoggedInstance<TInterface>(this IUnityContainer container, string name,
            TInterface instance, LifetimeManager lifetimeManager)
        {
            InjectionMember[] members = GetInstanceMembers(container, instance);
            return container.RegisterType<TInterface>(name, lifetimeManager, members);
        }

        #region Helpers

        public static IUnityContainer AddNewExtensionIfNotPresent<TExtension>(this IUnityContainer container)
            where TExtension : UnityContainerExtension, new()
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));
            if (container.Configure<TExtension>() == null)
                container.AddNewExtension<TExtension>();
            return container;
        }

        private static InjectionMember[] GetMembers(IUnityContainer container, params InjectionMember[] injectionMembers)
        {
            container.AddNewExtensionIfNotPresent<Microsoft.Practices.Unity.InterceptionExtension.Interception>();
            InjectionMember[] loggingMembers =
            {
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<LoggingInterceptionBehavior>()
            };
            return injectionMembers.Concat(loggingMembers).ToArray();
        }

        private static InjectionMember[] GetInstanceMembers<TInterface>(IUnityContainer container, TInterface instance)
        {
            return GetMembers(container, new InjectionFactory(c => instance));
        }

        #endregion
    }
}