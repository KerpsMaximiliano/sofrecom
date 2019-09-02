using System.Collections.Generic;

namespace Sofco.Core.Models.Common
{
    public class GenericOptionModel
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public bool Active { get; set; }

        public Dictionary<string, string> Parameters { get; set; }
    }
}
