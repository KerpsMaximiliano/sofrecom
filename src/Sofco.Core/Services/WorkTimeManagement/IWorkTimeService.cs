using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.WorkTimeManagement
{
    public interface IWorkTimeService
    {
        Response<WorkTimeModel> Get(DateTime date);

        Response<WorkTime> Save(WorkTimeAddModel model);

        Response<IList<HoursApprovedModel>> GetHoursApproved(WorktimeHoursApprovedParams model);

        Response<IList<HoursApprovedModel>> GetHoursPending(WorktimeHoursPendingParams model);

        Response Approve(int id, IList<BankHoursSplitted> bankHours);

        IEnumerable<Option> GetAnalytics();

        Response Reject(int id, string comments, bool massive);

        Response ApproveAll(List<HoursToApproveModel> hourIds);

        Response Send();

        Response<IList<WorkTimeSearchItemResult>> Search(SearchParams parameters);

        IEnumerable<Option> GetStatus();

        Response RejectAll(WorkTimeRejectParams parameters);

        Response Delete(int id);

        void Import(int analyticId, IFormFile file, Response<IList<WorkTimeImportResult>> response);

        byte[] ExportTemplate(int analyticId);
    }
}
