//
// unbindcontext.cs
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
using Stardust.Nucleus.TypeResolver;
using Stardust.Particles;

namespace Stardust.Nucleus.Internals
{
    internal sealed class UnbindContext<T> : IUnbindContext<T>
    {
        private IConfigurationKernel ConfigurationKernel;
        private IDependencyResolver _context;

        internal UnbindContext(IConfigurationKernel configurationKernel, IDependencyResolver context)
        {
            ConfigurationKernel = configurationKernel;
            _context = context;
        }

        /// <summary>
        /// Removes the binding if existing. 
        /// Does not throw an exception if there is not binding since the intention is achieved
        /// </summary>
        public void From<TImplementation>() where TImplementation : T
        {
            From<TImplementation>(TypeLocatorNames.DefaultName);
        }

        public void From<TImplementation>(string identifier) where TImplementation : T
        {
            var existingBinding = ConfigurationKernel.Resolve(typeof(T), identifier, _context);
            if (existingBinding.IsInstance())
            {
                ConfigurationKernel.Unbind(typeof(T), new ScopeContext(typeof(TImplementation), new ContextProviders.ScopeContext(_context)), identifier);
            }
        }

        public void From<TImplementation>(Enum identifier) where TImplementation : T
        {
            From<TImplementation>(identifier.ToString());
        }

        public void All()
        {
            ConfigurationKernel.UnbindAll(typeof(T));
        }

        public IBindContext<T> AllAndBind()
        {
            All();
            return new BindContext<T>(ConfigurationKernel, _context);
        }

        public void From(Enum typeEnum)
        {
            From(typeEnum.ToString());
        }

        public void From(string named)
        {
            ConfigurationKernel.Unbind(typeof(T), named);
        }

        public IBindContext<T> FromAndRebind(Enum typeEnum)
        {
            From(typeEnum.ToString());
            return new BindContext<T>(ConfigurationKernel, _context);
        }

        public IBindContext<T> FromAndRebind(string named)
        {
            From(named);
            return new BindContext<T>(ConfigurationKernel, _context);
        }
    }
}