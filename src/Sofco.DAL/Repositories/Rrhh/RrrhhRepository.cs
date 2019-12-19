using System;
using System.Collections.Generic;
using System.Linq;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Rrhh;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Rrhh;

namespace Sofco.DAL.Repositories.Rrhh
{
    public class RrhhRepository : IRrhhRepository
    {
        private readonly SofcoContext context;

        public RrhhRepository(SofcoContext context)
        {
            this.context = context;
        }

        public IList<SocialCharge> GetSocialCharges(int year, int month)
        {
            return context.SocialCharges
                .Include(x => x.Items)
                .Include(x => x.Employee)
                .Where(x => x.Year == year && x.Month == month)
                .ToList();
        }

        public void Add(List<SocialCharge> listToAdd)
        {
            context.AddRange(listToAdd);
        }

        public void Update(List<SocialCharge> listToUpdate)
        {
            context.UpdateRange(listToUpdate);
        }

        public bool ExistData(int yearId, int monthId)
        {
            return context.SocialCharges.Any(x => x.Year == yearId && x.Month == monthId);
        }

        public IList<Employee> GetEmployeesWithBestAllocation(DateTime today)
        {
            return context.Allocations.Include(x => x.Employee)
                .Include(x => x.Analytic)
                .Where(x => x.StartDate.Date == today.Date)
                .OrderByDescending(x => x.Percentage)
                .Select(x => new Employee
                {
                    Id = x.EmployeeId,
                    ManagerId = x.Employee.ManagerId,
                    Allocations = new List<Allocation>()
                    {
                        new Allocation
                        {
                            Id =  x.Id,
                            AnalyticId = x.AnalyticId,
                            EmployeeId = x.EmployeeId,
                            Percentage = x.Percentage,
                            Analytic = new Analytic
                            {
                                Id = x.AnalyticId,
                                ManagerId = x.Analytic.ManagerId
                            }
                        }
                    }
                })
                .ToList();
        }
    }
}
