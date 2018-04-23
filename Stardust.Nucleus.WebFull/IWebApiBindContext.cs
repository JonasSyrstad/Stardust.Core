using System;
using Stardust.Interstellar.Rest.Extensions;

namespace Stardust.Nucleus.Web
{
    public interface IWebApiBindContext<in T>
    {
        IScopeContext To<TService>() where TService : T;
        IScopeContext ToConstructor(Func<IServiceLocator, object> creator);
    }
}