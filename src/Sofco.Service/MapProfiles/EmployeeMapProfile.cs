using AutoMapper;
using Newtonsoft.Json;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Domain.Rh.Rhpro;
using Sofco.Domain.Rh.Tiger;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;
using Sofco.Framework.Helpers;
using Sofco.Repository.Rh.Settings;

namespace Sofco.Service.MapProfiles
{
    public class EmployeeMapProfile : Profile
    {
        private const int InitialBillingPercentage = 100;

        private const string ArCountry = "Argentina";

        private const char ListSeparator = ';';

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
                .ForMember(d => d.PrepaidHealthCode, s => s.MapFrom(x => x.Ospla))
                .ForMember(d => d.Salary, s =>  s.MapFrom(x => CryptographyHelper.Encrypt(x.Nuse1.ToString())))
                .ForMember(d => d.PrepaidAmount, s => s.MapFrom(x => CryptographyHelper.Encrypt(x.Nuset.ToString())))
                .ForMember(d => d.BeneficiariesCount, s => s.MapFrom(x => x.Ayude))
                .ForMember(d => d.Email, s => s.MapFrom(x => x.Email.Trim()));

            CreateMap<RhproEmployeeLicense, EmployeeLicense>()
                .ForMember(d => d.EmployeeNumber, s => s.MapFrom(x => x.Empleado))
                .ForMember(d => d.StartDate, s => s.MapFrom(x => x.Elfechadesde))
                .ForMember(d => d.EndDate, s => s.MapFrom(x => x.Elfechahasta))
                .ForMember(d => d.LicenseTypeNumber, s => s.MapFrom(x => x.Tdnro));

            CreateMap<Employee, EmployeeHistory>()
                .ForMember(d => d.Id, s => s.Ignore())
                .ForMember(d => d.EmployeeData, s => s.MapFrom(x => JsonConvert.SerializeObject(x)));

            CreateMap<Employee, EmployeeModel>()
                .ForMember(x => x.Manager, s => s.MapFrom(x => x.Manager.Name));

            CreateMap<EmployeeHistory, EmployeeHistoryModel>()
                .AfterMap((src, dest) =>
                {
                    if (!string.IsNullOrWhiteSpace(src.EmployeeData))
                    {
                        var data = JsonConvert.DeserializeObject<Employee>(src.EmployeeData);

                        dest.Profile = data.Profile;
                        dest.Seniority = data.Seniority;
                        dest.Technology = data.Technology;
                    }
                });

            CreateMap<Employee, EmployeeProfileModel>()
                .ForMember(d => d.Percentage, s => s.MapFrom(x => x.BillingPercentage));

            CreateMap<Allocation, EmployeeAllocationModel>();

            CreateMap<Employee, Option>()
                .ForMember(d => d.Text, s => s.ResolveUsing(x => $"{x.EmployeeNumber} - {x.Name}"));

            CreateMap<EmployeeEndNotificationModel, EmployeeEndNotification>()
                .ForMember(d => d.Recipients, s => s.MapFrom(x => string.Join(ListSeparator.ToString(), x.Recipients)));

            CreateMap<EmployeeEndNotification, EmployeeEndNotificationModel>()
                .ForMember(d => d.Recipients, s => s.MapFrom(x => x.Recipients.Split(ListSeparator)))
                .ForMember(d => d.CreatedDate, s => s.MapFrom(x => x.Created));
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
