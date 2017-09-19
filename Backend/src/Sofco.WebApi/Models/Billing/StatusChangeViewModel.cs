using Sofco.Model.Enums;

namespace Sofco.WebApi.Models.Billing
{
    public class StatusChangeViewModel
    {
        public int UserId { get; set; }
        public string Comment { get; set; }
        public SolfacStatus Status { get; set; }
    }
}
