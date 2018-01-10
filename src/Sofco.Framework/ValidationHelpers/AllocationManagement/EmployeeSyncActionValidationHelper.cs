using Sofco.Core.DAL;
using Sofco.Model.Enums;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Utils;

namespace Sofco.Framework.ValidationHelpers.AllocationManagement
{
    public class EmployeeSyncActionValidationHelper
    {
        public static void Exist(int id, Response<EmployeeSyncAction> response, IUnitOfWork unitOfWork)
        {
            var entity = unitOfWork.EmployeeSyncActionRepository.GetSingle(x => x.Id == id);

            if (entity == null)
            {
                response.AddError(Resources.AllocationManagement.EmployeeSyncAction.NotFound);
            }
            else
            {
                response.Data = entity;
            }
        }

        public static void ValidateNewStatus(Response<EmployeeSyncAction> response)
        {
            if (response.Data == null) return;

            if (response.Data.Status == EmployeeSyncActionStatus.Delete)
            {
                response.AddError(Resources.AllocationManagement.Employee.WrongStatus);
            }
        }
    }
}
