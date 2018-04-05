using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Admin;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.Admin;

namespace Sofco.DAL.Repositories.Admin
{
    public class TaskRepository : BaseRepository<Task>, ITaskRepository
    {
        public TaskRepository(SofcoContext context) : base(context)
        {
        }

        public bool ExistById(int taskId)
        {
            return context.Tasks.Any(x => x.Id == taskId);
        }

        public void UpdateCategory(Task task)
        {
            context.Entry(task).Property("CategoryId").IsModified = true;
        }

        public IList<Task> GetAllActives()
        {
            return context.Tasks.Where(x => x.Active).ToList().AsReadOnly();
        }

        public Task GetById(int id)
        {
            return context.Tasks.SingleOrDefault(x => x.Id == id);
        }

        public bool DescriptionExist(string description)
        {
            return context.Tasks.Any(x => x.Description == description);
        }
    }
}
