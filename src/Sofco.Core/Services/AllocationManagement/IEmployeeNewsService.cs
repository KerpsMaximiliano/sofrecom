﻿using System.Collections.Generic;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IEmployeeNewsService
    {
        Response<IList<EmployeeNewsModel>> GetEmployeeNews();

        Response<EmployeeSyncAction> Add(int newsId);

        Response<EmployeeSyncAction> Delete(int newsId, NewsDeleteModel model);

        Response<EmployeeSyncAction> Cancel(int newsId);
    }
}