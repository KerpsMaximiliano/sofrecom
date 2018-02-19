using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Sofco.Common.Helpers;

namespace Sofco.Repository.Rh.Extensions
{
    public static class ModelBuilderExtenions
    {
        public static void AddEntityConfigurationsFromAssembly(this ModelBuilder modelBuilder, Assembly assembly)
        {
            var mappingTypes = AssemblyHelper.GetMappingTypes(assembly, typeof(IEntityMappingConfiguration<>));

            foreach (var config in mappingTypes.Select(Activator.CreateInstance).Cast<IEntityMappingConfiguration>())
            {
                config.Map(modelBuilder);
            }
        }
    }
}
