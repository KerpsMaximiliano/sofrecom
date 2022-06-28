using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Models.RequestNote
{
    public class RequestNoteModel
    {
        public string Description { get; set; }
        public List<ProductsService> ProductsServices { get; set; }
        public int ProviderAreaId { get; set; }
        public List<Analytic> Analytics { get; set; }
        public bool RequiresEmployeeClient { get; set; }
        public List<Provider> Providers { get; set; }
        public bool ConsideredInBudget { get; set; }
        public int EvalpropNumber { get; set; }
        public string Comments { get; set; }
        public bool TravelSection { get; set; }
        public bool TrainingSection { get; set; }
        public Training Training { get; set; }
        public Travel Travel { get; set; }
        public int CreationUserId { get; set; }
        public int UserApplicantId { get; set; }
        public int WorkflowId { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Analytic
    {
        public int AnalyticId { get; set; }
        public int Asigned { get; set; }
    }

    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
    }

    public class ProductsService
    {
        public string ProductService { get; set; }
        public int Quantity { get; set; }
    }

    public class Provider
    {
        public int ProviderId { get; set; }
        public int? FileId { get; set; }
    }

    public class Training
    {
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
        public string Duration { get; set; }
        public int Ammount { get; set; }
        public List<Employee> Participants { get; set; }
    }

    public class Travel
    {
        public List<Employee> Passengers { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Destination { get; set; }
        public string Transportation { get; set; }
        public string Accommodation { get; set; }
        public string Details { get; set; }
    }


}
