using Sofco.Core.DAL.AllocationManagement;
using Sofco.Model.Enums;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Utils;

namespace Sofco.Framework.ValidationHelpers.AllocationManagement
{
    public static class EmployeeValidationHelper
    {
        public static void Exist(Response<Allocation> response, IEmployeeRepository employeeRepository, int employeeId)
        {
            var exist = employeeRepository.Exist(employeeId);

            if (!exist)
            {
                response.Messages.Add(new Message(Resources.es.AllocationManagement.Employee.NotFound, MessageType.Error));
            }
        }

        public static Employee Find(Response response, IEmployeeRepository employeeRepository, int id)
        {
            var employee = employeeRepository.GetById(id);

            if (employee == null)
            {
                response.Messages.Add(new Message(Resources.es.AllocationManagement.Employee.NotFound, MessageType.Error));
            }

            return employee;
        }
    }
}
