using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.Admin;

namespace Sofco.Core.DAL.Admin
{
    public interface ITaskRepository : IBaseRepository<Task>
    {
        bool ExistById(int taskId);
        void UpdateCategory(Task task);
        IList<Task> GetAllActives();
        Task GetById(int id);
        bool DescriptionExist(string modelDescription);
        IList<int> GetAllIds();
    }
}
