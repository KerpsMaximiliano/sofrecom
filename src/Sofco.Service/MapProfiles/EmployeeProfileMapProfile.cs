using AutoMapper;
using Sofco.Core.Models.Admin;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Service.MapProfiles
{
    public class EmployeeProfileMapProfile : Profile
    {
        public EmployeeProfileMapProfile()
        {
            CreateMap<EmployeeProfile, EmployeeProfileModel>();
        }
    }
}
