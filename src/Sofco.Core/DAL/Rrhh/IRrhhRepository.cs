using System;
using System.Collections.Generic;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Rrhh;

namespace Sofco.Core.DAL.Rrhh
{
    public interface IRrhhRepository
    {
        IList<SocialCharge> GetSocialCharges(int year, int month);
        void Add(List<SocialCharge> listToAdd);
        void Update(List<SocialCharge> listToUpdate);
        bool ExistData(int yearId, int monthId);
        IList<Employee> GetEmployeesWithBestAllocation(DateTime today);
        void Update(SocialCharge toUpdate);
        IList<SocialCharge> GetSocialCharges(int pYear, int pMonth, IList<int> employeesIds);
        IList<SocialCharge> GetSocialCharges(DateTime startDate, DateTime endDate);
    }
}
