using AutoMapper;
using Newtonsoft.Json;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Domain.Rh.Rhpro;
using Sofco.Domain.Rh.Tiger;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Repository.Rh.Settings;

namespace Sofco.Service.MapProfiles
{
    public class EmployeeMapProfile : Profile
    {
        private const int InitialBillingPercentage = 100;

        private const string ArCountry = "Argentina";

        public EmployeeMapProfile()
        {
            CreateMap<TigerEmployee, Employee>()
                .ForMember(d => d.EmployeeNumber, s => s.MapFrom(x => x.Legaj))
                .ForMember(d => d.Name, s => s.MapFrom(x => x.Nomb.Trim()))
                .ForMember(d => d.Birthday, s => s.MapFrom(x => x.Fenac))
                .ForMember(d => d.StartDate, s => s.MapFrom(x => x.Feiem))
                .ForMember(d => d.EndDate,
                    s => s.MapFrom(x => x.Febaj == RhSetting.TigerDateTimeMinValue ? null : x.Febaj))
                .ForMember(d => d.Profile, s => s.MapFrom(x => x.Dtitu.Trim()))
                .ForMember(d => d.Technology, s => s.MapFrom(x => x.Didio.Trim()))
                .ForMember(d => d.Seniority, s => s.MapFrom(x => x.Dgrup.Trim()))
                .ForMember(d => d.BillingPercentage, s => s.ResolveUsing(x => InitialBillingPercentage))
                .ForMember(d => d.Address, s => s.ResolveUsing(MapAddress))
                .ForMember(d => d.Location, s => s.MapFrom(x => x.Loca.Trim()))
                .ForMember(d => d.Province, s => s.MapFrom(x => x.Dprov.Trim()))
                .ForMember(d => d.Country, s => s.ResolveUsing(x => ArCountry))
                .ForMember(d => d.HealthInsuranceCode, s => s.MapFrom(x => x.Obsoc))
                .ForMember(d => d.PrepaidHealthCode, s => s.MapFrom(x => x.Ospla));

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

            CreateMap<Employee, EmployeeModel>();

            CreateMap<EmployeeHistory, EmployeeHistoryModel>()
                .AfterMap((src, dest) =>
                {
                    var data = JsonConvert.DeserializeObject<Employee>(src.EmployeeData);

                    dest.Profile = data.Profile;
                    dest.Seniority = data.Seniority;
                    dest.Technology = data.Technology;
                });

            CreateMap<Employee, EmployeeProfileModel>()
                .ForMember(d => d.Manager, s => s.ResolveUsing(x => "Diego O. Miguel"))
                .ForMember(d => d.Office, s => s.ResolveUsing(x => "Reconquista"))
                .ForMember(d => d.Percentage, s => s.MapFrom(x => x.BillingPercentage));
        }

        private string MapAddress(TigerEmployee item)
        {
            var streetNumber = string.Format(" {0}", item.Nro).TrimEnd();

            var floor = item.Piso > 0 ? string.Format(" {0}º", item.Piso) : string.Empty;

            var department = string.IsNullOrEmpty(item.Depto) ? string.Empty : string.Format(" {0}", item.Depto.Trim());

            return $"{item.Calle.Trim()}{streetNumber}{floor}{department}".Trim();
        }
    }
}
