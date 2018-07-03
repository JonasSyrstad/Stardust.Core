using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using Stardust.Core;
using Stardust.Interstellar.Rest.Client;
using Stardust.Interstellar.Rest.Common;
using Stardust.Interstellar.Rest.Extensions;
using Stardust.Interstellar.Rest.Service;
using Stardust.Particles;
using IDependencyResolver = Stardust.Nucleus.IDependencyResolver;
using IServiceLocator = Stardust.Interstellar.Rest.Extensions.IServiceLocator;

namespace Stardust.Nucleus.Web
{


    //public abstract class BlueprintBase : BlueprintBase<Logger>
    //{

    //}

    public abstract class BlueprintBase<TLogger> : IBlueprint where TLogger : ILogging
    {
        public void Bind(IConfigurator configuration)
        {


            LoggingExtentions.SetLogger(typeof(TLogger));
            BindWebActivator(configuration);
            ConfigureIoc(configuration);
            if (BlueprintExtensions.HasRegisteredServices)
                ServiceFactory.FinalizeRegistration();

        }

        protected abstract void ConfigureIoc(IConfigurator configuration);

        private void BindWebActivator(IConfigurator configuration)
        {
            var c = configuration as IInternalConfigurator;
            var di = c?.Items["resolver"] as IDependencyResolver;
            configuration.Bind<IProxyFactory>().To<ProxyFactoryImplementation>().SetRequestResponseScope();
            configuration.Bind<IServiceLocator>().To<ServiceLocator>().SetRequestResponseScope();
            configuration.Bind<IServiceProvider>().To<ServiceLocator>().SetRequestResponseScope();
            configuration.UnBind<IHttpActionInvoker>().AllAndBind().To<Invoker>().SetTransientScope();
            configuration.Bind<IControllerFactory>().To<StardustControllerFactory>().SetSingletonScope();
            configuration.Bind<IHttpControllerActivator>().To<StardustControllerActivator>().SetSingletonScope();
            configuration.Bind<IActionInvoker>().To<ControllerInvoker>().SetRequestResponseScope();
            configuration.Bind<IControllerActivator>().To<StardustControllerActivator>().SetSingletonScope();
            configuration.Bind<IServiceParameterResolver>().To<CustomResolver>().SetSingletonScope();
            configuration.Bind(typeof(ILogging)).To(LoggingType).SetRequestResponseScope();
            GlobalConfiguration.Configuration.DependencyResolver = new StardustDependencyResolver();
            DependencyResolver.SetResolver(new StardustDependencyResolver());
            configuration.Bind<IProxyFactory>().To<ProxyFactoryImplementation>().SetRequestResponseScope();
            configuration.Bind<HttpContextAccessor>().To<HttpContextAccessor>().SetRequestResponseScope();
        }

        public Type LoggingType => typeof(TLogger);
    }

    public class HttpContextAccessor
    {
        private HttpContext _current;

        internal void SetContext(HttpContext context)
        {
            _current = context;
            if (_current == null & HttpContext.Current != null)
                _current = HttpContext.Current;
        }

        public HttpContext Current
        {
            get
            {
                if (_current == null & HttpContext.Current != null)
                    _current = HttpContext.Current;
                return _current;
            }
            set => _current = value;
        }
    }

}
