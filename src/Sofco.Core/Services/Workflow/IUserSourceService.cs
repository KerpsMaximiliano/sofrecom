using Sofco.Domain.Models.Workflow;

namespace Sofco.Core.Services.Workflow
{
    public interface IUserSourceService
    {
        UserSource Get(string code);

        UserSource Get(string code, int sourceId);
    }
}
