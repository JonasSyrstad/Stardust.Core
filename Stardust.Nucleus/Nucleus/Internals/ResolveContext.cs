//
// resolvecontext.cs
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
using Stardust.Nucleus.ContainerIntegration;

namespace Stardust.Nucleus.Internals
{
    public interface IResolveContext<T>
    {
        ///// <summary>
        ///// Creates an instance of the resolved type as its base type or interface
        ///// </summary>
        //T Activate();

        IResolveContext<T> SetInitializer(Action<T> initializer);
        IScopeContext TypeContext { get; }

        Action<T> Initializer { get; }
    }

    internal sealed class ResolveContext<T> : IResolveContext<T>
    {
        public ContextProviders.IScopeContext Context { get; }
        public Action<T> Initializer { get; private set; }

        internal ResolveContext(IScopeContextInternal typeContext, ContextProviders.IScopeContext scopeContext)
        {


            Context = scopeContext;
            TypeContext = new Internals.InternalScopeContext(typeContext, scopeContext);

        }

        public IScopeContext TypeContext { get; private set; }


        public IResolveContext<T> SetInitializer(Action<T> initializer)
        {
            Initializer = initializer;
            return this;
        }

    }

    internal class InternalScopeContext : IScopeContextInternal
    {
        private readonly IScopeContextInternal _typeContext;

        public InternalScopeContext(IScopeContextInternal typeContext, ContextProviders.IScopeContext scopeContext)
        {
            _typeContext = typeContext;
            ActivationScopeContext = scopeContext;

        }

        public IScopeContext SetRequestResponseScope()
        {
            return this;
        }

        public IScopeContext SetTransientScope()
        {
            return this;
        }

        public IScopeContext SetSessionScope()
        {
            return this;
        }

        public IScopeContext SetThreadScope()
        {
            return this;
        }

        public IScopeContext SetSingletonScope()
        {
            return this;
        }

        public Scope? ActivationScope => _typeContext?.ActivationScope;
        public bool AllowOverride { get => _typeContext?.AllowOverride ?? false; set { } }
        public bool IsNull => _typeContext?.IsNull ?? true;
        public string ImplementationKey => _typeContext?.ImplementationKey;

        public void SetAllowOverride()
        {

        }

        public void DisableOverride()
        {

        }

        public Func<IDependencyResolver, object> CreatorMethod => _typeContext?.CreatorMethod;
        public Type BoundType => _typeContext?.BoundType;

        public ScopeContext SetScope(Scope scope)
        {
            return _typeContext as ScopeContext;
        }

        public ContextProviders.IScopeContext ActivationScopeContext { get; }
    }
}