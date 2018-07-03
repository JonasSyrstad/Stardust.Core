using System;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Routing;

namespace Stardust.Nucleus.Web
{
    public class StardustControllerActivator : IControllerActivator, IHttpControllerActivator
    {
        public static Action<IServiceProvider, HttpContextAccessor, Type> OnCreatingController;
        public static Action<IServiceProvider, HttpContextAccessor, object> OnControllerCreated;
        public IController Create(RequestContext requestContext, Type controllerType)
        {

            return CreateInternal<IController>(controllerType, HttpContext.Current);
        }

        private T CreateInternal<T>(Type controllerType, HttpContext requestContextHttpContext)
        {
            IDependencyResolver resolver;
            {
                if (requestContextHttpContext.Items.Contains("resolver"))
                {
                    resolver = requestContextHttpContext.Items["resolver"] as IDependencyResolver ??
                               Resolver.CreateScopedResolver();
                }
                else resolver = Resolver.CreateScopedResolver();
            }
            if (!(requestContextHttpContext.Items["resolver"] is IDependencyResolver))
                HttpContext.Current.Items.Add("resolver", resolver);
            var context = resolver.GetService<HttpContextAccessor>();   
            context.SetContext(requestContextHttpContext);
            OnCreatingController?.Invoke(resolver, context, controllerType);
            var controller = (T)controllerType.Activate(Scope.Context, resolver);
            OnControllerCreated?.Invoke(resolver, context, controller);
            return controller;
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            
            return CreateInternal<IHttpController>(controllerType, HttpContext.Current);
        }
    }
}