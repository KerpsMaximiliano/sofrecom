using Sofco.Core.DAL.AllocationManagement;
using Sofco.Model.Enums;
using Sofco.Model.Models.TimeManagement;
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
    }
}
