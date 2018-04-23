using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.Http.Dependencies;

namespace Stardust.Nucleus.Web
{
    internal class StardustDependencyResolver : System.Web.Http.Dependencies.IDependencyResolver, System.Web.Mvc.IDependencyResolver
    {
        private IDependencyResolver _resolver;

        public StardustDependencyResolver()
        {
            _resolver = Resolver.CreateScopedResolver();
        }
        public IDependencyScope BeginScope()
        {
            return new StardustDependencyScope(Resolver.CreateScopedResolver());
        }

        public object GetService(Type serviceType)
        {
            return _resolver.GetService(serviceType, "default");
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _resolver.GetServices(serviceType);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
                GC.SuppressFinalize(this);

        }

        ~StardustDependencyResolver()
        {
            Dispose(false);
        }
    }
}