﻿using System.Collections.Generic;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.WorkTimeManagement
{
    public interface IHolidayService
    {
        Response<List<Holiday>> Get(int year);

        Response<Holiday> Post(Holiday model);

        Response<List<Holiday>> ImportExternalData(int year);

        Response Delete(int holidayId);
    }
}