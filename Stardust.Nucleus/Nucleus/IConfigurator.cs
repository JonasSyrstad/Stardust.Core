using System;
using System.Collections.Generic;

namespace Stardust.Nucleus
{
    public interface IConfigurator
    {
        IBindContext<T> Bind<T>();
        IBindContext Bind(Type type);
        IBindContext BindAsGeneric(Type genericUnboundType);
        void RemoveAll();
        IUnbindContext<T> UnBind<T>();

    }

    public interface IInternalConfigurator : IConfigurator
    {
        Dictionary<string, object> Items { get; }
    }
}