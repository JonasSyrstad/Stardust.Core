using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Web;
using Stardust.Interstellar.Rest.Common;

namespace Stardust.Nucleus.Web
{
    public static class HttpContextExtensions
    {
        public static IDependencyResolver CurrentResolver(this HttpContext context)
        {
            return context.Items["resolver"] as IDependencyResolver;
        }

        public static IDependencyResolver GetOrCreateDependencyScope(this HttpContext context)
        {
            var resolver = context.CurrentResolver();
            if (resolver == null)
            {
                resolver = Resolver.CreateScopedResolver();
                context.Items.Add("resolver", resolver);
            }
            return resolver;
        }
    }
    public class CustomResolver : IServiceParameterResolver, IWebMethodConverter
    {
        public IEnumerable<ParameterWrapper> ResolveParameters(MethodInfo methodInfo)
        {
            return null;
        }

        public List<HttpMethod> GetHttpMethods(MethodInfo method)
        {
            return null;
        }
    }
}