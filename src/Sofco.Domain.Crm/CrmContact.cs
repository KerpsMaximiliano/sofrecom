namespace Sofco.Domain.Crm
{
    public class CrmContact
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public int StatusId { get; set; }

        public string Status { get; set; }

        public string AccountId { get; set; }
    }
}
