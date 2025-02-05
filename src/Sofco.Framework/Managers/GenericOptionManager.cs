﻿using System;
using System.Collections.Generic;
using Sofco.Core.Managers;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.Recruitment;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Managers
{
    public class GenericOptionManager : IGenericOptionManager
    {
        public void SetParameters(Option domain, Dictionary<string, string> parameters)
        {
            switch (domain)
            {
                case ReasonCause reasonCause:
                {
                    SetReasonCause(reasonCause, parameters);
                    break;
                }

                case CostType costType:
                {
                    SetCostType(costType, parameters);
                    break;
                }
            }
        }

        private void SetReasonCause(ReasonCause reasonCause, Dictionary<string, string> parameters)
        {
            if (parameters != null && parameters.ContainsKey("type"))
            {
                reasonCause.Type = (ReasonCauseType)Convert.ToInt32(parameters["type"]);
            }
        }

        private void SetCostType(CostType costType, Dictionary<string, string> parameters)
        {
            if (parameters != null && parameters.ContainsKey("category"))
            {
                costType.Category = Convert.ToInt32(parameters["category"]);
            }
        }
    }
}
