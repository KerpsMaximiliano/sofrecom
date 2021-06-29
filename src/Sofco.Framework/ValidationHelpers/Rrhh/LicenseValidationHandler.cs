using System;
using System.Linq;
using Sofco.Core.DAL;
using Sofco.Framework.StatusHandlers.License;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Models.Rrhh;
using Sofco.Domain.Utils;

namespace Sofco.Framework.ValidationHelpers.Rrhh
{
    public static class LicenseValidationHandler
    {
        public static void ValidateEmployee(Response response, License domain, IUnitOfWork unitOfWork)
        {
            if (domain.EmployeeId <= 0)
            {
                response.AddError(Resources.Rrhh.License.EmployeeRequired);
            }
            else
            {
                if (!unitOfWork.EmployeeRepository.Exist(domain.EmployeeId))
                {
                    response.AddError(Resources.Rrhh.License.EmployeeNotFound);
                }
            }
        }

        public static void ValidateManager(Response response, License domain, IUnitOfWork unitOfWork)
        {
            if (domain.ManagerId <= 0)
            {
                response.AddError(Resources.Rrhh.License.ManagerRequired);
            }
            else
            {
                if (!unitOfWork.UserRepository.ExistById(domain.ManagerId))
                {
                    response.AddError(Resources.Rrhh.License.ManagerNotFound);
                }
            }
        }

        public static void ValidateDates(Response response, License domain, bool isRrhh)
        {
            if (domain.StartDate == DateTime.MinValue || domain.EndDate == DateTime.MinValue)
            {
                response.AddError(Resources.Rrhh.License.DatesRequired);
            }
            else
            {
                if (domain.StartDate.DayOfWeek == DayOfWeek.Saturday || domain.StartDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    response.AddError(Resources.Rrhh.License.StartDateIsWeekend);
                }

                if (domain.StartDate.Date > domain.EndDate.Date)
                {
                    response.AddError(Resources.Rrhh.License.EndDateLessThanStartDate);
                }

                if (!isRrhh && (domain.StartDate.Date < DateTime.UtcNow.Date || domain.EndDate.Date < DateTime.UtcNow.Date))
                {
                    response.AddError(Resources.Rrhh.License.DatesLessThanToday);
                }
            }
        }

        public static void ValidateSector(Response response, License domain)
        {
            if (domain.SectorId <= 0)
            {
                //  Add Generic Sector
                domain.SectorId = 15;
            }
        }

        public static void ValidateLicenseType(Response response, License domain)
        {
            if (domain.TypeId <= 0)
            {
                response.AddError(Resources.Rrhh.License.TypeRequired);
            }
        }

        public static void ValidateDays(Response response, License domain, IUnitOfWork unitOfWork)
        {
            var licenseValidator = LicenseFactory.GetInstance(domain.TypeId);

            licenseValidator.Validate(response, domain, unitOfWork);
        }

        public static void ValidateWorkTimeOverlap(Response response, License domain, IUnitOfWork unitOfWork)
        {
            var workTimes =
                unitOfWork.WorkTimeRepository.GetByEmployeeId(domain.StartDate, domain.EndDate, domain.EmployeeId);

            if (workTimes.Any())
            {
                response.AddError(Resources.Rrhh.License.WorkTimeDateOverlap);
            }
        }

        public static License Find(int id, Response response, IUnitOfWork unitOfWork)
        {
            var license = unitOfWork.LicenseRepository.GetSingle(x => x.Id == id);

            if (license == null)
            {
                response.AddError(Resources.Rrhh.License.NotFound);
            }

            return license;
        }

        public static License FindFull(int id, Response response, IUnitOfWork unitOfWork)
        {
            var license = unitOfWork.LicenseRepository.GetById(id);

            if (license == null)
            {
                response.AddError(Resources.Rrhh.License.NotFound);
            }

            return license;
        }

        public static void ValidateApplicantNotEqualManager(Response response, License domain, IUnitOfWork unitOfWork)
        {
            var employee = unitOfWork.EmployeeRepository.GetSingle(x => x.Id == domain.EmployeeId);
            var user = unitOfWork.UserRepository.GetByEmail(employee.Email);

            if (domain.ManagerId == user.Id)
            {
                response.AddError(Resources.Rrhh.License.ManagerEqualsEmployee);
            }
        }
        
        public static void ValidateDatesOverlaped(Response response, License domain, IUnitOfWork unitOfWork)
        {
            if (domain.StartDate != DateTime.MinValue && domain.EndDate != DateTime.MinValue && domain.EmployeeId > 0)
            {
                if (unitOfWork.LicenseRepository.AreDatesOverlaped(domain.StartDate.Date, domain.EndDate.Date,
                    domain.EmployeeId))
                {
                    response.AddError(Resources.Rrhh.License.DatesOverlaped);
                }
            }
        }
    }
}
