using System;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Routing;

namespace Stardust.Nucleus.Web
{
    public class StardustControllerActivator : IControllerActivator
    {
        public IController Create(RequestContext requestContext, Type controllerType)
        {
            IDependencyResolver resolver;
            if (!requestContext.HttpContext.Items.Contains("resolver"))
                resolver = Resolver.CreateScopedResolver();
            else
                resolver = (IDependencyResolver)requestContext.HttpContext.Items["resolver"];
            requestContext.HttpContext.Items.Add("resolver", resolver);
            var controller = (IController)resolver.GetInstance(controllerType, Scope.Context);
            return controller;
        }
    }
    public class StardustApiControllerActivator : IHttpControllerActivator
    {
        IHttpController IHttpControllerActivator.Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            return Create(request, controllerDescriptor, controllerType);
        }
        private IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var resolver = HttpContext.Current.Items["resolver"] as IDependencyResolver ?? Resolver.CreateScopedResolver();
            HttpContext.Current.Items.Add("resolver", resolver);
            var controller = (IHttpController)controllerType.Activate(Scope.Context, resolver);
            return controller;

        }

    }
}