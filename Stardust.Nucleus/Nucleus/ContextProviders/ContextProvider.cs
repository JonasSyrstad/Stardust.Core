//
// contextprovider.cs
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using Stardust.Core;
using Stardust.Particles;

namespace Stardust.Nucleus.ContextProviders
{

    public class ManagedScopedProvider : ScopeProviderBase, IDisposable
    {
        private ConcurrentDictionary<Type, object> scopeContainer = new ConcurrentDictionary<Type, object>();
        private List<IDisposable> toDispose = new List<IDisposable>();
        private bool _isDisposing;

        protected override object DoResolve(Type type)
        {
            object instance;
            if (scopeContainer.TryGetValue(type, out instance)) return instance;
            return null;
        }

        protected override void DoBind(Type type, object toInstance)
        {
            if (!scopeContainer.TryAdd(type, toInstance))
                scopeContainer.TryAdd(type, toInstance);
            if (toInstance is IDisposable disposable)
                toDispose.Add(disposable);
        }

        public override void InvalidateBinding(Type type)
        {
            var d = toDispose.SingleOrDefault(type.IsInstanceOfType);
            d.TryDispose();
            object old;
            if (!scopeContainer.TryRemove(type, out old))
                scopeContainer.TryRemove(type, out old);
        }

        public override void KillEmAll()
        {
            if (_isDisposing) return;
            _isDisposing = true;
            foreach (var disposable in toDispose)
            {
                disposable.Dispose();
            }
            toDispose.Clear();
            scopeContainer.Clear();
        }

        private void ReleaseUnmanagedResources()
        {
            KillEmAll();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~ManagedScopedProvider()
        {
            ReleaseUnmanagedResources();
        }
    }
    /// <summary>
    /// The default implementation for the Context <see cref="Scope"/> provider, if not hosted in an request/response application it falls back to thread 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ContextProvider : ThreadProvider
    {


        /// <summary>
        /// adds an instance to the scope
        /// </summary>
        /// <param name="type">the type of object to add to the scope</param>
        /// <param name="toInstance">An instance or an instance of a derived class of the given <paramref name="type"/></param>
        protected override void DoBind(Type type, object toInstance)
        {
            var state = GetRequestState();
            if (state.IsInstance())
                state.Store(type, toInstance);
            else
            {
                base.DoBind(type, toInstance);
            }
        }

        /// <summary>
        /// removes the binding to the type if any.
        /// </summary>
        /// <param name="type">The type to remove from the scope</param>
        public override void InvalidateBinding(Type type)
        {
            var state = GetRequestState();
            if (state.IsInstance())
                state.Remove(type);
            else
                base.InvalidateBinding(type);
        }

        /// <summary>
        /// Invalidates all bindings within the scope
        /// </summary>
        public override void KillEmAll()
        {
            var state = GetRequestState();
            if (state.IsInstance())
                state.RemoveAll();
            else
                base.KillEmAll();
        }


        protected override object DoResolve(Type type)
        {
            var state = GetRequestState();
            if (state.IsInstance())
                return state.Get(type);
            return base.DoResolve(type);
        }

        private static IRequestState GetRequestState()
        {
            return contextResolver.GetRequestState();
            //IRequestState state = null;
            //if (ThreadSynchronizationContext.CurrentContext.IsInstance())
            //{
            //    state = new WcfRequestState();
            //}
            //else if (HttpContext.Current.IsInstance())
            //{
            //    state = new AspNetRequestState();
            //}
            //return state;
        }
        private static IContextTypeResolver contextResolver = new DefaultContextTypeResolver();

    }

    internal class DefaultContextTypeResolver : IContextTypeResolver
    {
        public IRequestState GetRequestState()
        {
            return new WcfRequestState();
        }

        internal class WcfRequestState : IRequestState
        {
            public object Get(Type key)
            {
                throw new NotImplementedException();
            }

            public void Remove(Type key)
            {
                throw new NotImplementedException();
            }

            public void RemoveAll()
            {
                throw new NotImplementedException();
            }

            public void Store(Type key, object instance)
            {
                throw new NotImplementedException();
            }
        }
    }

    public interface IContextTypeResolver
    {
        IRequestState GetRequestState();
    }
}