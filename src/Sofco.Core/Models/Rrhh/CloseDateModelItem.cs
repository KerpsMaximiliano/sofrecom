using System.Collections.Generic;

namespace Sofco.Core.Models.Rrhh
{
    public class CloseDateModel
    {
        public IList<CloseDateModelItem> Items { get; set; }

        public int CloseMonthValue { get; set; }
    }

    public class CloseDateModelItem
    {
        public int Id { get; set; }

        public int Day { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public string Description { get; set; }
    }
}
