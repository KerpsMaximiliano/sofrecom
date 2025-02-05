﻿using System.Collections.Generic;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IEmployeeEndNotificationService
    {
        Response<List<EmployeeEndNotificationModel>> Get(EmployeeEndNotificationParameters parameters);
    }
}