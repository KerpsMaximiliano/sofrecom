using Sofco.Core.DAL;
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
    }
}
