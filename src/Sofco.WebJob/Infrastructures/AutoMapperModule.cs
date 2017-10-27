using Autofac;
using AutoMapper;
using Sofco.Common.Helpers;
using Sofco.Service.MapProfiles;

namespace Sofco.WebJob.Infrastructures
{
    public class AutoMapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var profileTypes = AssemblyHelper.GetTypesByType(typeof(EmployeeMapProfile), typeof(Profile));

            builder.Register(ctx => new MapperConfiguration(cfg =>
            {
                foreach (var profile in profileTypes)
                {
                    cfg.AddProfile(profile);
                }
            }));

            builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper()).As<IMapper>();
        }
    }
}
