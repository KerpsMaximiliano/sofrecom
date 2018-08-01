using System;
using AutoMapper;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Domain.Nolaborables;

namespace Sofco.Service.MapProfiles
{
    public class HolidayMapProfile : Profile
    {
        public HolidayMapProfile()
        {
            CreateMap<Feriado, Holiday>()
                .ForMember(s => s.Id, x => x.Ignore())
                .ForMember(s => s.Name, x => x.MapFrom(_ => _.Motivo))
                .ForMember(s => s.DataSource, x => x.ResolveUsing(_ => HolidayDataSource.External))
                .ForMember(s => s.Date, x => x.ResolveUsing(f =>
                {
                    var month = int.Parse(f.Mes);
                    var day = int.Parse(f.Dia);
                    var date = new DateTime(f.Year, month, day, 0, 0, 0, DateTimeKind.Utc);
                    return date;
                }));
        }
    }
}
