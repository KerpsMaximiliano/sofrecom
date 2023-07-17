using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Admin;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.Admin;

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

        public IList<int> GetAllIds()
        {
            return context.Tasks.Where(x => x.Active).Select(x => x.Id).ToList();
        }

        public IList<Task> GetAllActivesWithCategories()
        {
            return context.Tasks.Include(x => x.Category).Where(x => x.Active).ToList().AsReadOnly();
        }

        public new IList<Task> GetAll()
        {
            return context.Tasks.Include(x => x.Category).ToList().AsReadOnly();
        }

        public IList<Task> GetByCategory(string descripcion)
        {
            return context.Tasks.Where(x => x.Category.Description==descripcion).ToList().AsReadOnly();
        }
    }
}
