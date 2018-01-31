using AutoMapper;
using Newtonsoft.Json;
using Sofco.Domain.Rh.Rhpro;
using Sofco.Domain.Rh.Tiger;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Repository.Rh.Settings;

namespace Sofco.Service.MapProfiles
{
    public class EmployeeMapProfile : Profile
    {
        private const int InitialBillingPercentage = 100;

        public EmployeeMapProfile()
        {
            CreateMap<TigerEmployee, Employee>()
                .ForMember(d => d.EmployeeNumber, s => s.MapFrom(x => x.Legaj))
                .ForMember(d => d.Name, s => s.MapFrom(x => x.Nomb))
                .ForMember(d => d.Birthday, s => s.MapFrom(x => x.Fenac))
                .ForMember(d => d.StartDate, s => s.MapFrom(x => x.Feiem))
                .ForMember(d => d.EndDate, s => s.MapFrom(x => x.Febaj == RhSetting.TigerDateTimeMinValue ? null : x.Febaj))
                .ForMember(d => d.Profile, s => s.MapFrom(x => x.Dtitu))
                .ForMember(d => d.Technology, s => s.MapFrom(x => x.Didio))
                .ForMember(d => d.Seniority, s => s.MapFrom(x => x.Dgrup))
                .ForMember(d => d.BillingPercentage, s => s.ResolveUsing(x => InitialBillingPercentage));

            CreateMap<RhproEmployeeLicense, EmployeeLicense>()
                .ForMember(d => d.EmployeeNumber, s => s.MapFrom(x => x.Empleado))
                .ForMember(d => d.StartDate, s => s.MapFrom(x => x.Elfechadesde))
                .ForMember(d => d.EndDate, s => s.MapFrom(x => x.Elfechahasta))
                .ForMember(d => d.LicenseTypeNumber, s => s.MapFrom(x => x.Tdnro));

            CreateMap<RhproLicenseType, LicenseType>()
                .ForMember(d => d.LicenseTypeNumber, s => s.MapFrom(x => x.Tdnro))
                .ForMember(d => d.Description, s => s.MapFrom(x => x.Tddesc));

            CreateMap<Employee, EmployeeHistory>()
                .ForMember(d => d.Id, s => s.Ignore())
                .ForMember(d => d.EmployeeData, s => s.MapFrom(x => JsonConvert.SerializeObject(x)));
        }
    }
}
