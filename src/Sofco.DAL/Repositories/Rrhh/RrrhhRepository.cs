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
            var employees = context.Employees
                .Include(x => x.Allocations)
                    .ThenInclude(x => x.Analytic)
                .Where(x => x.Allocations.Any(s => s.StartDate.Date == today.Date))
                .ToList();

            var result = new List<Employee>();

            foreach (var employee in employees)
            {
                var bestAllocation = employee.Allocations.OrderByDescending(x => x.Percentage).FirstOrDefault();

                if(bestAllocation == null) continue;

                result.Add(new Employee
                {
                    Id = employee.Id,
                    ManagerId = employee.ManagerId,
                    Allocations = new List<Allocation>()
                    {
                        new Allocation
                        {
                            Id =  bestAllocation.Id,
                            AnalyticId = bestAllocation.AnalyticId,
                            EmployeeId = bestAllocation.EmployeeId,
                            Percentage = bestAllocation.Percentage,
                            Analytic = new Analytic
                            {
                                Id = bestAllocation.AnalyticId,
                                ManagerId = bestAllocation.Analytic.ManagerId
                            }
                        }
                    }
                });
            }

            return result;
        }
    }
}
