using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Utils;

namespace Sofco.Framework.ValidationHelpers.Billing
{
    public class HitoValidatorHelper
    {
        public static void ValidateAmmounts(HitoParameters hito, Response response)
        {
            if (!hito.Ammount.HasValue || hito.Ammount.GetValueOrDefault() <= 0)
            {
                response.Messages.Add(new Message(Resources.Billing.Project.HitoAmmoutRequired, MessageType.Error));
            }
        }

        public static void ValidateOpportunity(HitoParameters hito, Response response)
        {
            if (string.IsNullOrWhiteSpace(hito.OpportunityId) || hito.OpportunityId.Equals("00000000-0000-0000-0000-000000000000"))
            {
                response.Messages.Add(new Message(Resources.Billing.Project.OpportunityRequired, MessageType.Error));
            }
        }

        public static void ValidateName(HitoParameters hito, Response response)
        {
            if (string.IsNullOrWhiteSpace(hito.Name))
            {
                response.Messages.Add(new Message(Resources.Billing.Project.NameRequired, MessageType.Error));
            }
        }

        public static void ValidateMonth(HitoParameters hito, Response response)
        {
            if (hito.Month <= 0)
            {
                response.Messages.Add(new Message(Resources.Billing.Project.MonthRequired, MessageType.Error));
            }
        }

        public static void ValidateDate(HitoParameters hito, Response response)
        {
            if (!hito.StartDate.HasValue)
            {
                response.AddError(Resources.Billing.Project.DateRequired);
            }
        }

        public static void ValidateCurrency(HitoParameters hito, Response response)
        {
            if (string.IsNullOrWhiteSpace(hito.MoneyId))
            {
                response.AddError(Resources.Common.CurrencyRequired);
            }
        }

        public static void ValidateProject(HitoParameters hito, Response response)
        {
            if (string.IsNullOrWhiteSpace(hito.ProjectId) || hito.ProjectId.Equals("00000000-0000-0000-0000-000000000000"))
            {
                response.AddError(Resources.Billing.Project.Required);
            }
        }

        public static void ValidateDates(Project project, Response response, HitoParameters hito)
        {
            if (project == null)
            {
                response.AddError(Resources.Billing.Project.NotFound);
            }
            else
            {
                if (!(hito.StartDate.GetValueOrDefault().Date >= project.StartDate.Date &&
                    hito.StartDate.GetValueOrDefault().Date <= project.EndDate.Date))
                {
                    response.AddError(Resources.Billing.Project.HitoDatesOutRange);
                }
            }
        }
    }
}
