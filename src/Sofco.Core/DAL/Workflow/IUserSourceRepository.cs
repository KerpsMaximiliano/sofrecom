using Sofco.Domain.Models.Workflow;

namespace Sofco.Core.DAL.Workflow
{
    public interface IUserSourceRepository
    {
        UserSource Get(string code, int sourceId);
        void Add(UserSource userSource);
    }
}
