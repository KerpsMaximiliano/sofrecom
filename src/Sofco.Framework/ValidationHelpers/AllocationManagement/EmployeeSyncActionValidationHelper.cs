using Sofco.Core.DAL;
using Sofco.Core.Models.AllocationManagement;
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

        public static void ValidateDeleteStatus(Response<EmployeeSyncAction> response)
        {
            if (response.Data == null) return;

            if (response.Data.Status == EmployeeSyncActionStatus.New)
            {
                response.AddError(Resources.AllocationManagement.Employee.WrongStatus);
            }
        }

        public static void ValidateEndReasonType(Response<EmployeeSyncAction> response, NewsDeleteModel model)
        {
            if (!model.Type.HasValue || model.Type <= 0)
            {
                response.AddError(Resources.AllocationManagement.EmployeeSyncAction.EndReasonTypeRequired);
            }

            if (model.Type == 17 && string.IsNullOrWhiteSpace(model.Comments))
            {
                response.AddError(Resources.AllocationManagement.EmployeeSyncAction.CommentsRequired);
            }
        }
    }
}
