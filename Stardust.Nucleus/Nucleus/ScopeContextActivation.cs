using System;
using System.Data;
using Stardust.Nucleus.ContainerIntegration;
using Stardust.Nucleus.Internals;
using Stardust.Nucleus.ObjectActivator;
using Stardust.Particles;

namespace Stardust.Nucleus
{

    public static class ScopeContextActivation
    {
        internal static object Activate(this IScopeContext context)
        {
            return ((IScopeContextInternal)context).Activate(Scope.Context);
        }

        internal static T Activate<T>(this IScopeContextInternal context, Action<T> initializeMethod, Scope? scope = null)
        {
            if (context.IsNull() || context.IsNull) return default(T);
            if (context.BoundType.IsGenericTypeDefinition)
                context = CreateConcreteType<T>(context);
            var container = ContainerFactory.Current(context.ActivationScopeContext);
            var instance = (T)container.Resolve(context.BoundType, GetActivationScope(context, scope));

            if (!instance.IsNull()) return instance;
            instance = ActivateAndInitialize(context, initializeMethod);
            container.Bind(instance.GetType(), instance, GetActivationScope(context, scope));
            return instance;
        }

        internal static T Activate<T>(this IScopeContextInternal context, Scope scope)
        {
            return context.Activate<T>(null, GetActivationScope(context, scope));
        }

        internal static object Activate(this IScopeContextInternal context, Scope scope)
        {
            if (context.IsNull() || context.IsNull) return null;

            var instance = ContainerFactory.Current(context.ActivationScopeContext).Resolve(context.BoundType, GetActivationScope(context, scope));

            if (!instance.IsNull()) return instance;
            instance = ActivateAndInitialize(context, null);
            ContainerFactory.Current(context.ActivationScopeContext).Bind(instance.GetType(), instance, GetActivationScope(context, scope));
            return instance;
        }


        internal static T Activate<T>(this IScopeContextInternal context, Scope scope, Action<T> initializeMethod)
        {
            return context.Activate(initializeMethod, scope);
        }


        private static T ActivateAndInitialize<T>(IScopeContextInternal context, Action<T> initializeMethod)
        {
            var instance = (T)(context.CreatorMethod.IsInstance() ? context.CreatorMethod(context.ActivationScopeContext?.Resolver) : CreateInstance(context));
            if (initializeMethod.IsInstance())
                initializeMethod(instance);
            return instance;
        }

        private static object ActivateAndInitialize(IScopeContextInternal context, Action<object> initializeMethod)
        {
            var instance = (context.CreatorMethod.IsInstance() ? context.CreatorMethod(context.ActivationScopeContext?.Resolver) : CreateInstance(context));
            if (initializeMethod.IsInstance())
                initializeMethod(instance);
            return instance;
        }


        //private static object CreateInstance<T>(IScopeContextInternal context)
        //{
        //    if (context.BoundType.IsGenericTypeDefinition)
        //        context = CreateConcreteType(context, context.BoundType);
        //    return context.Activate();
        //}

        private static object CreateInstance(IScopeContextInternal context)
        {
            if (context.BoundType.IsGenericTypeDefinition)
                context = CreateConcreteType(context, context.BoundType);
            return ActivatorFactory.Activator.Activate(context.BoundType, context.ActivationScopeContext.Resolver);
        }

        private static IScopeContextInternal CreateConcreteType(IScopeContextInternal context, Type serviceType)
        {
            context =
                new ScopeContext(context.BoundType.MakeConcreteType(serviceType.GetGenericArguments()), null).SetScope(
                    context.ActivationScope.GetValueOrDefault(ScopeContext.GetDefaultScope()));
            return context;
        }

        private static IScopeContextInternal CreateConcreteType<T>(IScopeContextInternal context)
        {
            context =
                new ScopeContext(context.BoundType.MakeConcreteType(typeof(T).GetGenericArguments()), null).SetScope(
                    context.ActivationScope.GetValueOrDefault(ScopeContext.GetDefaultScope()));
            return context;
        }

        private static Scope GetActivationScope(this IScopeContextInternal context)
        {
            return context.ActivationScope ?? Scope.PerRequest;
        }
        private static Scope GetActivationScope(this IScopeContextInternal context, Scope? scope)
        {
            if ((context.AllowOverride || !context.ActivationScope.HasValue) && scope.HasValue)
                return scope.Value;
            return context.GetActivationScope();
        }



        public static object Activate(this Type self, Scope scope, IDependencyResolver resolver)
        {
            if (self.IsNull()) throw new NoNullAllowedException("The type to create was not set.");
            var container = ContainerFactory.Current(new ContextProviders.ScopeContext(resolver));
            var item = container.Resolve(self, scope);
            if (item.IsNull())
            {
                item = ActivatorFactory.Activator.Activate(self, resolver);
                container.Bind(self, item, scope);
            }
            return item;
        }

        internal static T Activate<T>(this Type self, Scope scope, Action<T> initializeMethod, IDependencyResolver resolver)
        {
            if (self.IsNull()) throw new NoNullAllowedException("The type to create was not set.");
            var item = (T)ContainerFactory.Current(null).Resolve(self, scope);
            if (item.IsNull())
            {
                item = ActivatorFactory.Activator.Activate<T>(self, resolver);
                if (initializeMethod.IsInstance()) initializeMethod(item);
                ContainerFactory.Current(null).Bind(self, item, scope);
            }
            return item;
        }

        internal static T Activate<T>(this IResolveContext<T> self)
        {
            if (self.IsNull()) throw new NoNullAllowedException("The type to create was not set.");
            if (self.TypeContext == null) return default(T);
            var context = ((IScopeContextInternal)self.TypeContext);
            return context.Activate(context.GetActivationScope(), self.Initializer);
        }
    }
}