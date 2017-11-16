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
        public static void ValidateAmmounts(IList<HitoSplittedParams> hitos, Response response)
        {
            if (hitos.Any(x => x.Ammount <= 0))
            {
                response.Messages.Add(new Message(Resources.es.Billing.Project.HitoAmmoutRequired, MessageType.Error));
            }
        }

        public static void ValidateHitosQuantity(IList<HitoSplittedParams> hitos, Response response)
        {
            if (hitos.Count < 2)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Project.Almost2HitosRequired, MessageType.Error));
            }
        }

        public static void ValidateOpportunity(IList<HitoSplittedParams> hitos, Response response)
        {
            if (hitos.Any(x => x.OpportunityId.Equals("00000000-0000-0000-0000-000000000000") || string.IsNullOrWhiteSpace(x.OpportunityId)))
            {
                response.Messages.Add(new Message(Resources.es.Billing.Project.OpportunityRequired, MessageType.Error));
            }
        }
    }
}
