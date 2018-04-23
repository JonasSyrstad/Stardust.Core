using Stardust.Interstellar.Rest.Client;

namespace Stardust.Nucleus.Web
{
    public static class BlueprintExtensions
    {
        public static IScopeContext ToServiceProxy<T>(this IBindContext<T> bindContext, string baseUrl)
        {
            return bindContext.ToConstructor(ProxyFactory.CreateProxy<T>(), s => new ServiceLocator(s).CreateRestClient<T>(baseUrl));
        }

        public static IScopeContext ToServiceProxy<T>(this IBindContext<T> bindContext, string baseUrl, string identifier)
        {
            return bindContext.ToConstructor(ProxyFactory.CreateProxy<T>(), s => new ServiceLocator(s).CreateRestClient<T>(baseUrl), identifier);
        }

        public static IWebApiBindContext<T> BindWebApiController<T>(this IConfigurator configuration)
        {
            var resolver = (configuration as IInternalConfigurator)?.Items["resolver"] as IDependencyResolver;
            return new BindContext<T>(configuration.Bind<T>(), resolver);
        }

        internal static bool HasRegisteredServices;
    }
}