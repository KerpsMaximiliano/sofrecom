using System.Collections.Generic;
using Sofco.Core.Models.Recruitment;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Recruitment
{
    public interface IApplicantService
    {
        Response Add(ApplicantAddModel model);
        Response<IList<ApplicantResultModel>> Search(ApplicantSearchParameters parameter);
        Response Update(int id, ApplicantAddModel model);
        Response<ApplicantDetailModel> Get(int id);
        Response<IList<ApplicantCallHistory>> GetApplicantHistory(int applicantId);
        Response Register(int id, RegisterModel model);
        Response ChangeStatus(int id, ApplicantChangeStatusModel parameter);
        Response<IList<ApplicantFileModel>> GetFiles(int id);
    }
}
