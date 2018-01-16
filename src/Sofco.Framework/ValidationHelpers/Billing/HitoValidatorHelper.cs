using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Utils;

namespace Sofco.Framework.ValidationHelpers.Billing
{
    public class HitoValidatorHelper
    {
        public static void ValidateAmmounts(HitoSplittedParams hito, Response response)
        {
            if (!hito.Ammount.HasValue || hito.Ammount.GetValueOrDefault() <= 0)
            {
                response.Messages.Add(new Message(Resources.Billing.Project.HitoAmmoutRequired, MessageType.Error));
            }
        }

        public static void ValidateOpportunity(HitoSplittedParams hito, Response response)
        {
            if (string.IsNullOrWhiteSpace(hito.OpportunityId) || hito.OpportunityId.Equals("00000000-0000-0000-0000-000000000000"))
            {
                response.Messages.Add(new Message(Resources.Billing.Project.OpportunityRequired, MessageType.Error));
            }
        }

        public static void ValidateName(HitoSplittedParams hito, Response response)
        {
            if (string.IsNullOrWhiteSpace(hito.Name))
            {
                response.Messages.Add(new Message(Resources.Billing.Project.NameRequired, MessageType.Error));
            }
        }

        public static void ValidateMonth(HitoSplittedParams hito, Response response)
        {
            if (hito.Month <= 0)
            {
                response.Messages.Add(new Message(Resources.Billing.Project.MonthRequired, MessageType.Error));
            }
        }
    }
}
