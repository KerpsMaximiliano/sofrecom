﻿using System.Collections.Generic;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IWorkTimeApprovalService
    {
        Response<List<UserApproverModel>> Save(List<UserApproverModel> userApprovers);

        Response Delete(int workTimeApprovalId);
    }
}