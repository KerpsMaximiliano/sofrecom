using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace Sofco.Common.Helpers
{
    public class AssemblyHelper
    {
        public static IEnumerable<Assembly> GetReferencingAssemblies(string assemblyName)
        {
            var assemblies = new List<Assembly>();
            var dependencies = DependencyContext.Default.RuntimeLibraries;
            foreach (var library in dependencies)
            {
                if (IsCandidateLibrary(library, assemblyName))
                {
                    var assembly = Assembly.Load(new AssemblyName(library.Name));
                    assemblies.Add(assembly);
                }
            }
            return assemblies;
        }

        private static bool IsCandidateLibrary(RuntimeLibrary library, string assemblyName)
        {
            return library.Name == (assemblyName)
                || library.Dependencies.Any(d => d.Name.StartsWith(assemblyName));
        }

        public static IEnumerable<Type> GetMappingTypes(Assembly assembly, Type mappingInterface)
        {
            return assembly
                .GetTypes()
                .Where(x =>
                    !x.GetTypeInfo().IsAbstract 
                    && x.GetInterfaces()
                    .Any(y => y.GetTypeInfo().IsGenericType 
                    && y.GetGenericTypeDefinition() == mappingInterface));
        }

        public static List<Type> GetTypesByType(Type assemblyOriginType, Type type)
        {
            return assemblyOriginType.GetTypeInfo()
                .Assembly
                .GetTypes()
                .Where(x => type.IsAssignableFrom(x))
                .ToList();
        }
    }
}
