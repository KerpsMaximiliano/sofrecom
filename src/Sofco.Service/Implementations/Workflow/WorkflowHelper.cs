using System;
using System.Collections.Generic;
using System.Text;
using Sofco.Common.Settings;
using Sofco.Domain.Enums;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.Service.Implementations.Workflow
{
    public class WorkflowHelper
    {
        public static void CheckEspecialUsers(WorkflowEntity entity, int actualStateId, int nextStateId, AppSetting appSetting)
        {
            if (appSetting.GeneralDirectorUserId == entity.UserApplicantId)
            {
                if (entity is Refund refund)
                {
                    if (actualStateId == appSetting.WorkflowStatusPendingDirectorId &&
                        nextStateId == appSetting.WorkflowStatusPendingGeneralDirectorId)
                    {
                        entity.StatusId = appSetting.WorkflowStatusDafId;
                    }
                }

                if (entity is Advancement advancement)
                {
                    if (advancement.Type == AdvancementType.Viaticum &&
                        actualStateId == appSetting.WorkflowStatusDraft &&
                        nextStateId == appSetting.WorkflowStatusPendingManagerId)
                    {
                        entity.StatusId = appSetting.WorkflowStatusDafId;
                    }
                }
            }

            if (appSetting.FinancialDirectorUserId == entity.UserApplicantId)
            {
                if (entity is Refund refund)
                {
                    if (nextStateId == appSetting.WorkflowStatusDafId)
                    {
                        entity.StatusId = appSetting.WorkflowStatusGafId;
                    }
                }

                if (entity is Advancement advancement)
                {
                    if (advancement.Type == AdvancementType.Viaticum &&
                        actualStateId == appSetting.WorkflowStatusPendingManagerId &&
                        nextStateId == appSetting.WorkflowStatusPendingDirectorId)
                    {
                        entity.StatusId = appSetting.WorkflowStatusGafId;
                    }

                    if (advancement.Type == AdvancementType.Salary &&
                        nextStateId == appSetting.WorkflowStatusDafId)
                    {
                        entity.StatusId = appSetting.WorkflowStatusGafId;
                    }
                }
            }
        }
    }
}
