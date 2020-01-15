using System.Collections.Generic;
using Sofco.Core.Models.Recruitment;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Recruitment
{
    public interface IRecruitmentReportService
    {
        Response<IList<RecruitmentReportResponse>> Search(RecruitmentReportParameters parameter);
    }
}
