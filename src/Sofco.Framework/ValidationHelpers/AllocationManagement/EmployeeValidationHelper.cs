using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;

namespace Sofco.Framework.ValidationHelpers.AllocationManagement
{
    public static class EmployeeValidationHelper
    {
        public static void Exist(Response response, IEmployeeRepository employeeRepository, int employeeId)
        {
            var exist = employeeRepository.Exist(employeeId);

            if (!exist)
            {
                response.Messages.Add(new Message(Resources.AllocationManagement.Employee.NotFound, MessageType.Error));
            }
        }

        public static Employee Find(Response response, IEmployeeRepository employeeRepository, int id)
        {
            var employee = employeeRepository.GetById(id);

            if (employee == null)
            {
                response.Messages.Add(new Message(Resources.AllocationManagement.Employee.NotFound, MessageType.Error));
            }

            return employee;
        }

        public static void ValidateBusinessHours(Response response, EmployeeBusinessHoursParams model)
        {
            if (model.BusinessHours < 1 || model.BusinessHours > 8)
            {
                response.Messages.Add(new Message(Resources.AllocationManagement.Employee.BusinessHoursWrong, MessageType.Error));
            }

            if (string.IsNullOrWhiteSpace(model.BusinessHoursDescription))
            {
                response.Messages.Add(new Message(Resources.AllocationManagement.Employee.BusinessHoursEmpty, MessageType.Error));
            }
        }
    }
}
