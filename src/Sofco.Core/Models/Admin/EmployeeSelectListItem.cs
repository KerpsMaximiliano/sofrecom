using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Models.Admin
{
    public class EmployeeSelectListItem
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string employeeNumber { get; set; }
        public int? UserId { get; set; }
    }

}
