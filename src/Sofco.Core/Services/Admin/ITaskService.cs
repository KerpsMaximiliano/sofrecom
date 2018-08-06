using System.Collections.Generic;
using Sofco.Core.Models.Admin;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Admin
{
    public interface ITaskService
    {
        Response Add(TaskModel model);
        IList<Task> GetAll(bool active);
        Response<TaskModel> GetById(int id);
        Response<Task> Active(int id, bool active);
        Response Update(TaskModel model);
    }
}
