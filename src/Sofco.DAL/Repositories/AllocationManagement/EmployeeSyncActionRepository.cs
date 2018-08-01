using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.DAL.Repositories.AllocationManagement
{
    public class EmployeeSyncActionRepository : BaseRepository<EmployeeSyncAction>, IEmployeeSyncActionRepository
    {
        public EmployeeSyncActionRepository(SofcoContext context) : base(context)
        {
        }

        public void Save(List<EmployeeSyncAction> employeeSyncActions)
        {
            var storedItems = GetByEmployeeNumber(employeeSyncActions.Select(s => s.EmployeeNumber).ToArray());

            var storedNumbers = storedItems.Select(s => s.EmployeeNumber).ToList();

            foreach (var item in employeeSyncActions)
            {
                if (storedNumbers.Contains(item.EmployeeNumber))
                {
                    var updateItem = storedItems
                        .First(s => s.EmployeeNumber == item.EmployeeNumber);

                    Update(updateItem, item);
                }
                else
                {
                    Insert(item);
                }
            }

            context.SaveChanges();
        }

        public bool Exist(int id)
        {
            return context.EmployeeSyncActions.Any(x => x.Id == id);
        }

        private List<EmployeeSyncAction> GetByEmployeeNumber(string[] employeeNumbers)
        {
            return context.EmployeeSyncActions
                .Where(s => employeeNumbers.Contains(s.EmployeeNumber))
                .ToList();
        }

        private void Update(EmployeeSyncAction storedData, EmployeeSyncAction data)
        {
            storedData.EmployeeData = data.EmployeeData;
            storedData.Status = data.Status;

            Update(storedData);
        }
    }
}