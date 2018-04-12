using System;
using Sofco.Model.Models.Admin;

namespace Sofco.Core.Models.Admin
{
    public class TaskModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
    }

    public class TaskListItem
    {
        public TaskListItem(Task task)
        {
            this.Id = task.Id;
            this.Description = task.Description;
            this.StartDate = task.StartDate;
            this.EndDate = task.EndDate;
            this.Active = task.Active;

            this.Category = task.Category?.Description;
        }

        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Active { get; set; }

        public string Category { get; set; }
    }
}
