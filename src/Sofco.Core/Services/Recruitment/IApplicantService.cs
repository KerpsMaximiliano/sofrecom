using Sofco.Core.Models.Recruitment;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Recruitment
{
    public interface IApplicantService
    {
        Response Add(ApplicantAddModel model);
    }
}
