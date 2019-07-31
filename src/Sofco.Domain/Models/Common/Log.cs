using System;

namespace Sofco.Domain.Models.Common
{
    public class Log : BaseEntity
    {
        public string Username { get; set; }

        public DateTime Created { get; set; }

        public string Comment { get; set; }
    }
}
