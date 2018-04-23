using System;
using Stardust.Interstellar.Rest.Extensions;
using Stardust.Interstellar.Rest.Service;

namespace Stardust.Nucleus.Web
{
    internal class BindContext<T> : IWebApiBindContext<T>
    {

        private readonly IBindContext<T> _bind;
        private readonly IDependencyResolver _resolver;

        public BindContext(IBindContext<T> bind, IDependencyResolver resolver)
        {
            _bind = bind;
            _resolver = resolver;
        }

        public IScopeContext To<TService>() where TService : T
        {
            ServiceFactory.CreateServiceImplementation<T>(new ServiceLocator(_resolver));
            BlueprintExtensions.HasRegisteredServices = true;
            return _bind.To<TService>();
        }

        public IScopeContext ToConstructor(Func<IServiceLocator, object> creator)
        {
            ServiceFactory.CreateServiceImplementation<T>(new ServiceLocator(_resolver));
            BlueprintExtensions.HasRegisteredServices = true;
            return _bind.ToConstructor(s => creator(new ServiceLocator(s)));
        }
    }
}