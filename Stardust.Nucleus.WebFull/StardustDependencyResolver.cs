using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http.Dependencies;
using Stardust.Particles;

namespace Stardust.Nucleus.Web
{
    internal class StardustDependencyResolver : System.Web.Http.Dependencies.IDependencyResolver, System.Web.Mvc.IDependencyResolver
    {


        public IDependencyScope BeginScope()
        {
            var resolver = Resolver.CreateScopedResolver();
            if (HttpContext.Current != null && !(HttpContext.Current.Items["resolver"] is IDependencyResolver))
                HttpContext.Current.Items.Add("resolver", resolver);
            resolver.GetService<ILogging>()?.DebugMessage("Starting dependency scope");
            return new StardustDependencyScope(resolver);
        }

        public object GetService(Type serviceType)
        {
            IDependencyResolver resolver;
            if (HttpContext.Current != null && HttpContext.Current.Items.Contains("resolver"))
            {
                resolver = HttpContext.Current.Items["resolver"] as IDependencyResolver ??
                           Resolver.CreateScopedResolver();
            }
            else resolver = Resolver.CreateScopedResolver();
            if (HttpContext.Current != null && !(HttpContext.Current.Items["resolver"] is IDependencyResolver))
                HttpContext.Current.Items.Add("resolver", resolver);
            resolver.GetService<ILogging>()?.DebugMessage($"Resolving service {serviceType.FullName}");
            return resolver.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            IDependencyResolver resolver;
            if (HttpContext.Current.Items.Contains("resolver"))
            {
                resolver = HttpContext.Current.Items["resolver"] as IDependencyResolver ??
                           Resolver.CreateScopedResolver();
            }
            else resolver = Resolver.CreateScopedResolver();
            if (!(HttpContext.Current.Items["resolver"] is IDependencyResolver))
                HttpContext.Current.Items.Add("resolver", resolver);
            resolver.GetService<ILogging>()?.DebugMessage($"Resolving services {serviceType.FullName}");
            return resolver.GetServices(serviceType);
        }


        public void Dispose()
        {
        }

    }
}