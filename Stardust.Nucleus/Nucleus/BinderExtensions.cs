using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Stardust.Nucleus.Extensions;
using Stardust.Nucleus.TypeResolver;
using Stardust.Particles;

namespace Stardust.Nucleus
{
    public static class BinderExtensions
    {
        public static void ToAssembly<T>(this IBindContext<T> self, string assemblyName, Action<IScopeContext> scopeHandler = null)
        {
            var assembly = Assembly.Load(assemblyName);
            self.ToAssembly(assembly, scopeHandler);
        }

        public static void ToAssembly<T>(this IBindContext<T> self, Assembly assembly, Action<IScopeContext> scopeHandler = null)
        {
            foreach (var definedType in assembly.DefinedTypes)
            {
                if (!definedType.Implements(typeof(T))) continue;
                var attrib = definedType.GetAttribute<ImplementationKeyAttribute>();
                IScopeContext scope;
                if (attrib != null) scope = self.To(definedType, attrib.Name);
                else scope = self.To(definedType);
                if (scopeHandler != null) scopeHandler(scope);
            }
        }

        public static IConfigurator LoadAssembly(this IConfigurator self, string name)
        {
            Assembly.Load(new AssemblyName(name));
            return self;
        }

        public static IConfigurator LoadAssemblyFromPath(this IConfigurator self, string path)
        {
            Assembly.LoadFrom(path);
            return self;
        }

        /// <summary>
        /// Resolves all types in the loaded assemblies
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IConfigurator ResolveAll(this IConfigurator self, bool onlyInterfacesOrAbstracts = true, Action<IScopeContext> scopeHandler = null)
        {

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                self.ResolveAssembly(onlyInterfacesOrAbstracts, assembly, scopeHandler);
            }
            return self;
        }

        public static IConfigurator ResolveAssembly(this IConfigurator self, bool onlyInterfacesOrAbstracts, string assemblyName, Action<IScopeContext> scopeHandler = null)
        {
            var assembly = Assembly.Load(new AssemblyName(assemblyName));
            return self.ResolveAssembly(onlyInterfacesOrAbstracts, assembly);
        }

        public static IConfigurator ResolveAssembly(this IConfigurator self, bool onlyInterfacesOrAbstracts, Assembly assembly, Action<IScopeContext> scopeHandler = null)
        {
            if (scopeHandler == null) scopeHandler = a => { a.SetTransientScope(); };
            foreach (var definedType in assembly.DefinedTypes)
            {
                if (onlyInterfacesOrAbstracts && !definedType.IsConcreteType())
                {
                    continue;
                }
                var name = definedType.GetAttribute<ImplementationKeyAttribute>() != null ? definedType.GetAttribute<ImplementationKeyAttribute>().Name : TypeLocatorNames.DefaultName;
                var implementations = FindImplementations(definedType);
                foreach (var implementation in implementations)
                {
                    scopeHandler(definedType.IsGenericTypeDefinition ? self.BindAsGeneric(definedType).To(implementation, name) : self.Bind(definedType).To(definedType, name));
                }
            }
            return self;
        }

        private static IEnumerable<TypeInfo> FindImplementations(TypeInfo definedType)
        {
            return from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in a.DefinedTypes
                where t.Implements(definedType) && t.IsConcreteType()
                select t;
        }
    }
}