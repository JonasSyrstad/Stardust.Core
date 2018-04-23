//
// ContainerFactory.cs
// This file is part of Stardust
//
// Author: Jonas Syrstad (jsyrstad2+StardustCore@gmail.com), http://no.linkedin.com/in/jonassyrstad/) 
// Copyright (c) 2014 Jonas Syrstad. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.Web;
using Stardust.Nucleus.ContainerIntegration;
using Stardust.Nucleus.ContextProviders;
using Stardust.Nucleus.ObjectActivator;
using Stardust.Particles;

namespace Stardust.Nucleus
{
    /// <summary>
    /// This class serves as the main entry point for the OLM container within Stardust. It provides public methods that you may use within your application to keep instances of a type within the given scope
    /// </summary>
    public static class ContainerFactory
    {
        private static IContainer ContainerSingleton;
        private static readonly object LockObject = new object();

        /// <summary>
        /// The current OLM container
        /// </summary>
        /// <param name="context"></param>
        private static IContainer GetContainer(ContextProviders.IScopeContext context)
        {
            if (ContainerSingleton.IsInstance()) return new ScopedContainer( ContainerSingleton, context);
            lock (LockObject)
            {
                var container = new AdvancedActivator<IContainer>().CreateInstance(typeof(Container));
                if (ContainerSingleton != null) return ContainerSingleton;
                ContainerSingleton = container;
            }

            return new ScopedContainer(ContainerSingleton, context);
        }

        /// <summary>
        /// Moves an instance of an object from one OLM scope to another
        /// </summary>
        /// <param name="instance">The instance to promote</param>
        /// <param name="from">Source scope</param>
        /// <param name="to">Destination scope</param>
        public static void PromoteObject(this object instance, Scope from, Scope to)
        {
            GetContainer(null).InvalidateBinding(instance.GetType(), @from);
            GetContainer(null).Bind(instance.GetType(), instance, to);
        }

        /// <summary>
        /// Drops the container and creates a new on next pass
        /// </summary>
        public static void ClearContainer()
        {
            lock (LockObject)
            {
                if (ContainerSingleton.IsInstance())
                    ContainerSingleton.KillAllInstances();
                ContainerSingleton = null;
            }
        }

        /// <summary>
        /// The current OLM container
        /// </summary>
        public static IContainer Current(ContextProviders.IScopeContext context)
        {
             return GetContainer(context); 
        }

        /// <summary>
        /// Gets the current principal from the correct context. This is not the same form asp.net/mvc and WCF
        /// </summary>
        /// <remarks>If not found in any of the contexts(http/wcf) the Thread.CurrentPrincipal is used</remarks>
        //public static IPrincipal CurrentPrincipal
        //{
        //    get
        //    {
        //        if (HttpContext.Current.IsInstance() && HttpContext.Current.User.IsInstance())
        //            return HttpContext.Current.User;
        //        if (Thread.CurrentPrincipal.IsInstance())
        //            return Thread.CurrentPrincipal;
        //        throw new SecurityException("No principal found in OperationContext, HttpContext or current thread.");
        //    }
        //}
    }

    internal class ScopedContainer : IContainer
    {
        private readonly IContainer _containerSingleton;
        private readonly ContextProviders.IScopeContext _context;

        public ScopedContainer(IContainer containerSingleton,ContextProviders.IScopeContext context)
        {
            _containerSingleton = containerSingleton;
            _context = context;
        }

        public void Bind(Type type, object toInstance, Scope scope)
        {
            var provider = GetProvider(scope);
                provider.Bind(type,toInstance);
        }

        public void InvalidateBinding(Type type, Scope scope)
        {
            if (scope == Scope.Context && _context != null)
                _context.Provider.InvalidateBinding(type);
            else
                _containerSingleton.InvalidateBinding(type,scope);
        }

        public void KillAllInstances()
        {
           _context?.Provider?.KillEmAll();
            _containerSingleton?.KillAllInstances();
        }

        public object Resolve(Type type, Scope scope, Func<IDependencyResolver,object> constructor)
        {
            var provider = GetProvider(scope);

            var instance = provider.Resolve(type);
            if (instance != null)
            {
                instance = constructor(_context.Resolver);
                provider.Bind(type, instance);
               
            }
            return instance;
        }

        public IScopeProvider GetProvider(Scope scope)
        {
            IScopeProvider provider;
            if (scope == Scope.Context && _context != null)
            {
                provider = _context.Provider;
            }
            else
            {
                provider = _containerSingleton.GetProvider(scope);
            }
            return provider;
        }

        public object Resolve(Type type, Scope scope)
        {
            var provider = GetProvider(scope);

            var instance = provider.Resolve(type);
            return instance;
        }
        
        public IExtendedScopeProvider ExtendScope(Scope scopeToExtend)
        {
            return _containerSingleton.ExtendScope(scopeToExtend);
        }
    }
}