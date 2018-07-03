using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Stardust.Particles;
using IDependencyResolver = Stardust.Nucleus.IDependencyResolver;

namespace Stardust.Nucleus.Web
{
    internal sealed class StardustDependencyScope : IDependencyScope
    {
        private IDependencyResolver ScopeContainer;

        public ILogging Logging { get; }

        public StardustDependencyScope(IDependencyResolver scopeContainer)
        {
            ScopeContainer = scopeContainer;
            Logging = scopeContainer.GetService<ILogging>();
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
                GC.SuppressFinalize(this);
            ScopeContainer.TryDispose();
            ScopeContainer = null;
        }

        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~StardustDependencyScope()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public object GetService(Type serviceType)
        {
            Logging?.DebugMessage($"Resolving service {serviceType.FullName}");
            return ScopeContainer.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            Logging?.DebugMessage($"Resolving services {serviceType.FullName}");
            return ScopeContainer.GetServices(serviceType);
        }
    }
}