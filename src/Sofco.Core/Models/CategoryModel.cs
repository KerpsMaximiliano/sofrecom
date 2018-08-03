using System;
using System.Collections.Generic;
using Sofco.Core.Models.Admin;
using Sofco.Domain.Models.Admin;

namespace Sofco.Core.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public IList<TaskModel> Tasks { get; set; }
    }

    public class CategoryListItem
    {
        public CategoryListItem(Category category)
        {
            this.Id = category.Id;
            this.Description = category.Description;
            this.StartDate = category.StartDate;
            this.EndDate = category.EndDate;
            this.Active = category.Active;
        }

        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Active { get; set; }
    }
}
