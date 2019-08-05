using Sofco.Domain.Utils;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IEmployeeUpdateService
    {
        Response<string> UpdateEmployees();

        Response UpdateSalaryAndPrepaids();
    }
}