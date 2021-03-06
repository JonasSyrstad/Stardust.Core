﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Stardust.Nucleus.ContainerIntegration;
using Stardust.Nucleus.Extensions;
using Stardust.Nucleus.Internals;
using Stardust.Particles;

namespace Stardust.Nucleus.TypeResolver
{
    internal class TypeResolverFromAssemblyScanning : IAssemblyScanningTypeResolver
    {
        public IEnumerable<IScopeContext> LocateInLoadedAssemblies(Type type, IDependencyResolver resolver)
        {
            return (DoAssemblyScanning(type, resolver)).ToList();
        }

        private IEnumerable<IScopeContext> DoAssemblyScanning(Type type, IDependencyResolver resolver)
        {
            if (ConfigurationManagerHelper.GetValueOnKey("stardust.allowAssemblyScanning") != "true") return new List<IScopeContext>();
            return from assembly in AppDomain.CurrentDomain.GetAssemblies()
                   from definedType in assembly.DefinedTypes
                   where !definedType.Implements(type)
                   select new BindContext(ConfigurationKernel, type, resolver).To(definedType, GetBindingName(definedType));
        }

        private string GetBindingName(TypeInfo definedType)
        {
            var attrib = definedType.GetAttribute<ImplementationKeyAttribute>();
            return attrib != null ? attrib.Name : TypeLocatorNames.DefaultName;
        }

        public IConfigurationKernel ConfigurationKernel { get; set; }
    }
}
