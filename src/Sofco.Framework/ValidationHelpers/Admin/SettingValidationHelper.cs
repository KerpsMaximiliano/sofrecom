using Sofco.Model.Models.Admin;
using Sofco.Model.Utils;

namespace Sofco.Framework.ValidationHelpers.Admin
{
    public class SettingValidationHelper
    {
        public static Response<Setting> ValidateAllocationManagementMonths(Setting setting)
        {
            var response = new Response<Setting>();

            var value = setting.Value;

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

        public static Response<Setting> ValidateLicenseCertificatePendingDayOfMonth(Setting setting)
        {
            var response = new Response<Setting>();

            var value = setting.Value;

            int months;

            if (!int.TryParse(value, out months))
            {
                response.AddError(Resources.Rrhh.License.WrongCertificateMonthQuantity);

                return response;
            }

            if (months >= 1 && months <= 28) return response;

            response.AddError(Resources.Rrhh.License.WrongCertificateMonthQuantity);

            return response;
        }
    }
}
