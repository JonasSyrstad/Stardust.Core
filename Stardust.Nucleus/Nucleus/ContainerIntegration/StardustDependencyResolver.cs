using System;
using System.Collections.Generic;
using System.Linq;
using Stardust.Nucleus.ContextProviders;
using Stardust.Nucleus.Internals;
using Stardust.Nucleus.TypeResolver;

namespace Stardust.Nucleus.ContainerIntegration
{
    internal sealed class StardustDependencyResolver : IDependencyResolver, IServiceProvider
    {
        internal StardustDependencyResolver(Func<IConfigurationKernel> kernel)
        {
            KernelResolver = kernel();
            _scopeContext = new ContextProviders.ScopeContext(this);
        }

        private IConfigurationKernel KernelResolver;
        internal readonly ContextProviders.IScopeContext _scopeContext;
        private bool _isDisposed;

        private IResolveContext<T> FindServiceImplementation<T>(string named)
        {
            return new ResolveContext<T>((IScopeContextInternal)KernelResolver.Resolve(typeof(T), named, this), _scopeContext);
        }

        public object GetInstance(Type type, Scope scope)
        {
            var typeContext = new Internals.ScopeContext(type, _scopeContext);
            typeContext.SetScope(scope);
            return new ResolveContext<object>(typeContext, _scopeContext).Activate();
        }

        public object GetService(Type serviceType)
        {
            return GetService(serviceType, "default");
        }

        public T GetService<T>()
        {
            return GetService<T>("default");
        }

        public T GetService<T>(Action<T> initializer)
        {
            return FindServiceImplementation<T>(TypeLocatorNames.DefaultName).SetInitializer(initializer).Activate();
        }

        public T GetService<T>(string named)
        {
            var result = GetService(typeof(T), named);
            if (result == null) return default(T);
            return (T)result;
        }

        public T GetService<T>(string named, Action<T> initializer)
        {
            return FindServiceImplementation<T>(named).SetInitializer(initializer).Activate();
        }

        public T[] GetServices<T>()
        {
            return KernelResolver.ResolveAll(typeof(T)).ActivateAll<T>();

        }

        public object GetService(Type serviceType, string named)
        {
            return KernelResolver.Resolve(serviceType, named, this)?.Activate();
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            var items = KernelResolver.ResolveAll(serviceType);
            if (items == null) return new List<object>();
            return (from i in items where i != null select i.Activate())?.ToList();
        }

        public IExtendedScopeProvider BeginExtendedScope(IExtendedScopeProvider scope)
        {
            return scope;
        }

        public Dictionary<string, T> GetServicesNamed<T>(string exceptWithName)
        {
            if (exceptWithName == null)
                return (from i in KernelResolver.ResolveAllNamed(typeof(T)) select new { Instance = (T)i.Value.Activate(), Name = i.Key ?? "default" }).ToDictionary(k => k.Name, v => v.Instance);
            return (from i in KernelResolver.ResolveAllNamed(typeof(T)) where i.Key != exceptWithName select new { Instance = (T)i.Value.Activate(), Name = i.Key ?? "default" }).ToDictionary(k => k.Name, v => v.Instance);
        }

        public Dictionary<string, object> GetServicesNamed(Type serviceType)
        {
            return (from i in KernelResolver.ResolveAllNamed(serviceType) select new { Instance = i.Value.Activate(), Name = i.Key ?? "default" }).ToDictionary(k => k.Name, v => v.Instance);
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            _scopeContext?.Dispose();
        }
    }
}