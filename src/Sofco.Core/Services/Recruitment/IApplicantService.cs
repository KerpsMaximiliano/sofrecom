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
    }
}
