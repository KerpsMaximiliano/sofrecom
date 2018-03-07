using Sofco.Model.Models.Admin;
using Sofco.Model.Utils;

namespace Sofco.Framework.ValidationHelpers.Admin
{
    public class SettingValidationHelper
    {
        public static Response<GlobalSetting> ValidateAllocationManagementMonths(GlobalSetting globalSetting)
        {
            var response = new Response<GlobalSetting>();

            var value = globalSetting.Value;

            int months;

            if (!int.TryParse(value, out months))
            {
                response.AddError(Resources.AllocationManagement.Analytic.WrongMonthQuantity);

                return response;
            }

            if (months >= 1 && months <= 36) return response;

            response.AddError(Resources.AllocationManagement.Analytic.WrongMonthQuantity);

            return response;
        }
    }
}
