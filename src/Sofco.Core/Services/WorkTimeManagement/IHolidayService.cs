using System.Collections.Generic;
using Sofco.Model.Models.WorkTimeManagement;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.WorkTimeManagement
{
    public interface IHolidayService
    {
        Response<List<Holiday>> Get(int year);

        Response<Holiday> Post(Holiday model);

        Response<List<Holiday>> ImportExternalData(int year);
    }
}