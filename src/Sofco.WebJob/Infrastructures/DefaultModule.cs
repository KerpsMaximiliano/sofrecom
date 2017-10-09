using System.Linq;
using Autofac;
using Sofco.Common.Helpers;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Infrastructures
{
    public class DefaultModule : Module
    {
        private const string ServiceAssemblyEndName = "Service";

        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = AssemblyHelper.GetReferencingAssemblies(nameof(Sofco)).ToArray();

            builder.RegisterAssemblyTypes(assemblies)
                .AssignableTo<IJob>()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(s => s.Name.EndsWith(ServiceAssemblyEndName))
                .AsImplementedInterfaces();
        }
    }
}
