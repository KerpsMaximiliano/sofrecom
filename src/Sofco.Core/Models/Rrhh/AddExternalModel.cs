namespace Sofco.Core.Models.Rrhh
{
    public class AddExternalModel
    {
        public int UserId { get; set; }

        public int ManagerId { get; set; }

        public int CountryCode { get; set; }

        public int AreaCode { get; set; }

        public string Phone { get; set; }

        public int Hours { get; set; }
    }
}
