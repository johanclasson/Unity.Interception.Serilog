using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Unity.Interception.Serilog
{
    public static class UnityContainerExtensions
    {
        public static IUnityContainer AddNewExtensionIfNotPresent<TExtension>(this IUnityContainer container)
            where TExtension : UnityContainerExtension, new()
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));
            if (container.Configure<TExtension>() == null)
                container.AddNewExtension<TExtension>();
            return container;
        }

        public static IUnityContainer RegisterLoggedType<TFrom, TTo>(this IUnityContainer container,
            params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            container
                .AddNewExtensionIfNotPresent<Microsoft.Practices.Unity.InterceptionExtension.Interception>()
                .RegisterType<TFrom, TTo>(injectionMembers)
                .Configure<Microsoft.Practices.Unity.InterceptionExtension.Interception>()
                .SetInterceptorFor<TFrom>(new InterfaceInterceptor());
            return container;
        }

        public static IUnityContainer RegisterLoggedType<TFrom, TTo>(this IUnityContainer container, string name,
            params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            container
                .AddNewExtensionIfNotPresent<Microsoft.Practices.Unity.InterceptionExtension.Interception>()
                .RegisterType<TFrom, TTo>(name, injectionMembers)
                .Configure<Microsoft.Practices.Unity.InterceptionExtension.Interception>()
                .SetInterceptorFor<TFrom>(new InterfaceInterceptor());
            return container;
        }

        public static IUnityContainer RegisterLoggedType<TFrom, TTo>(this IUnityContainer container,
            LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            container
                .AddNewExtensionIfNotPresent<Microsoft.Practices.Unity.InterceptionExtension.Interception>()
                .RegisterType<TFrom, TTo>(lifetimeManager, injectionMembers)
                .Configure<Microsoft.Practices.Unity.InterceptionExtension.Interception>()
                .SetInterceptorFor<TFrom>(new InterfaceInterceptor());
            return container;
        }

        public static IUnityContainer RegisterLoggedType<TFrom, TTo>(this IUnityContainer container, string name,
            LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            container
                .AddNewExtensionIfNotPresent<Microsoft.Practices.Unity.InterceptionExtension.Interception>()
                .RegisterType<TFrom, TTo>(name, lifetimeManager, injectionMembers)
                .Configure<Microsoft.Practices.Unity.InterceptionExtension.Interception>()
                .SetInterceptorFor<TFrom>(new InterfaceInterceptor());
            return container;
        }

        public static IUnityContainer RegisterLoggedInstance<TInterface>(this IUnityContainer container,
            TInterface instance)
        {
            container
                .AddNewExtensionIfNotPresent<Microsoft.Practices.Unity.InterceptionExtension.Interception>()
                .RegisterInstance(instance)
                .Configure<Microsoft.Practices.Unity.InterceptionExtension.Interception>()
                .SetInterceptorFor<TInterface>(new InterfaceInterceptor());
            return container;
        }

        public static IUnityContainer RegisterLoggedInstance<TInterface>(this IUnityContainer container,
            TInterface instance, LifetimeManager lifetimeManager)
        {
            container
                .AddNewExtensionIfNotPresent<Microsoft.Practices.Unity.InterceptionExtension.Interception>()
                .RegisterInstance(instance, lifetimeManager)
                .Configure<Microsoft.Practices.Unity.InterceptionExtension.Interception>()
                .SetInterceptorFor<TInterface>(new InterfaceInterceptor());
            return container;
        }

        public static IUnityContainer RegisterLoggedInstance<TInterface>(this IUnityContainer container, string name,
            TInterface instance)
        {
            container
                .AddNewExtensionIfNotPresent<Microsoft.Practices.Unity.InterceptionExtension.Interception>()
                .RegisterInstance(name, instance)
                .Configure<Microsoft.Practices.Unity.InterceptionExtension.Interception>()
                .SetInterceptorFor<TInterface>(new InterfaceInterceptor());
            return container;
        }

        public static IUnityContainer RegisterLoggedInstance<TInterface>(this IUnityContainer container, string name,
            TInterface instance, LifetimeManager lifetimeManager)
        {
            container
                .AddNewExtensionIfNotPresent<Microsoft.Practices.Unity.InterceptionExtension.Interception>()
                .RegisterInstance(name, instance, lifetimeManager)
                .Configure<Microsoft.Practices.Unity.InterceptionExtension.Interception>()
                .SetInterceptorFor<TInterface>(new InterfaceInterceptor());
            return container;
        }
    }
}