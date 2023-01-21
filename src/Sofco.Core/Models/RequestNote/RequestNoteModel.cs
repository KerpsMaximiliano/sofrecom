using Newtonsoft.Json;
using Sofco.Common.Settings;
using Sofco.Domain.RequestNoteStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sofco.Core.Models.RequestNote
{
    
    public class RequestNoteModel
    {
        private readonly AppSetting _settings;
        public RequestNoteModel() { }
        public RequestNoteModel(Domain.Models.RequestNote.RequestNote note, List<string> permissions, int userId, AppSetting settings)
        {
            _settings = settings;
            Id = note.Id;
            
            Description = note.Description;
            ProviderAreaId = note.ProviderAreaId;
            RequiresEmployeeClient = note.RequiresEmployeeClient;
            ConsideredInBudget = note.ConsideredInBudget;
            EvalpropNumber = note.EvalpropNumber;
            Comments = note.Comments;
            PurchaseOrderAmmount = note.PurchaseOrderAmmount;
            PurchaseOrderNumber = note.PurchaseOrderNumber;
            TravelSection = note.TravelSection;
            TrainingSection = note.TrainingSection;
            CreationUserId = note.CreationUserId;
            UserApplicantId = note.UserApplicantId;
            WorkflowId = note.WorkflowId;
            ProviderAreaDescription = note.ProviderArea?.Description;
            CreationUserDescription = note.CreationUser?.UserName;
            UserApplicantDescription = note.UserApplicant?.UserName;
            WorkflowDescription = note.Workflow?.Description;
            StatusId = note.StatusId;
            StatusDescription = note.Status?.Name;
            ProductsServices = new List<ProductsService>();
            Analytics = new List<Analytic>();
            Providers = new List<Provider>();
            ProvidersSelected = new List<Provider>();
            if (note.Providers != null)
            {
                ProvidersSelected = note.Providers.Select(p => new Provider()
                {
                    FileId = p.FileId,
                    FileDescription = p.File?.FileName,
                    ProviderId = p.ProviderId,
                    ProviderDescription = p.Provider?.Name,
                    Ammount = p.Price
                }).ToList();
                //ProviderSelectedId = note.Providers.FirstOrDefault(p => p.IsSelected)?.ProviderId;
            }
            if (note.ProvidersSugg != null)
            {
                Providers = note.ProvidersSugg.Select(p => new Provider()
                {
                    FileId = p.FileId,
                    FileDescription = p.File?.FileName,
                    ProviderId = p.ProviderId,
                    ProviderDescription = p.Provider?.Name
                }).ToList();
                //ProviderSelectedId = note.Providers.FirstOrDefault(p => p.IsSelected)?.ProviderId;
            }
            if (note.Attachments != null)
                Attachments = note.Attachments.Select(p => new File()
                {
                    FileId = p.FileId,
                    FileDescription = p.File?.FileName,
                    Type = p.Type,
                    TypeDescription = Enum.GetName(RequestNoteFileTypes.Borrador.GetType(), p.Type).ToString()
                }).ToList();
            if (note.ProductsServices != null)
                ProductsServices = note.ProductsServices.Select(p => new ProductsService()
                {
                    ProductService = p.ProductService,
                    Quantity = p.Quantity
                }).ToList();
            if(note.Analytics != null)
                Analytics = note.Analytics.Select(p=> new Analytic()
                {
                    AnalyticId = p.AnalyticId,
                    Asigned = p.Percentage,
                    Description = p.Analytic?.Name,
                    Status = p.Status,
                    ManagerId = p.Analytic?.ManagerId
                }).ToList();
            if (note.Trainings != null && note.Trainings.Any())
                Training = note.Trainings.Select(t => new Training()
                {
                    Ammount = t.Ammount,
                    Date = t.TrainingDate,
                    Duration = t.Duration,
                    Location = t.Place,
                    Name = t.Topic,
                    Subject = t.Subject,
                    Participants = t.Employees.Select(e=> new Employee()
                    {
                        EmployeeId = e.EmployeeId,
                        Name = e.Employee?.Name,
                        Sector = e.SectorProject
                    }).ToList()
                }).FirstOrDefault();
            if (note.Travels != null && note.Travels.Any())
                Travel = note.Travels.Select(t => new Travel()
                {
                    Accommodation = t.Accommodation,
                    DepartureDate = t.DepartureDate,
                    ReturnDate = t.ReturnDate,
                    Destination = t.Destination,
                    Details = t.ItineraryDetail,
                    Transportation = t.Conveyance,
                    Passengers = t.Employees.Select(e => new Employee()
                    {
                        EmployeeId = e.EmployeeId,
                        Name = e.Employee?.Name
                    }).ToList()
                }).FirstOrDefault();
            HasReadPermissions = ValidateReadPermissions(permissions, userId);
            HasEditPermissions = ValidateEditPermissions(permissions, userId);

        }
        public int? Id { get; set; }

        public bool HasEditPermissions { get; set; }
        public bool HasReadPermissions { get; set; }
        public string Description { get; set; }
        public List<ProductsService> ProductsServices { get; set; }
        public int ProviderAreaId { get; set; }
        public int? ProviderSelectedId { get; set; }
        public string ProviderAreaDescription { get; set; }
        public List<Analytic> Analytics { get; set; }
        public bool RequiresEmployeeClient { get; set; }
        public List<Provider> Providers { get; set; }
        public List<Provider> ProvidersSelected { get; set; }
        public List<File> Attachments { get; set; }
        public bool ConsideredInBudget { get; set; }
        public int EvalpropNumber { get; set; }
        public string Comments { get; set; }
        public decimal? PurchaseOrderAmmount { get; set; }
        public string Remarks { get; set; }
        public int? PurchaseOrderNumber { get; set; }
        public bool TravelSection { get; set; }
        public bool TrainingSection { get; set; }
        public Training Training { get; set; }
        public Travel Travel { get; set; }
        public int CreationUserId { get; set; }
        public string CreationUserDescription { get; set; }
        public int UserApplicantId { get; set; }

        public string UserApplicantDescription { get; set; }
        public int WorkflowId { get; set; }
        public string WorkflowDescription { get; set; }

        public int StatusId { get; set; }
        public string StatusDescription { get; set; }

        private bool ValidateReadPermissions(List<string> permissions, int userId)
        {
            return CreationUserId == userId || permissions.Any(p => p == "NP_READONLY");
        }
        private bool ValidateEditPermissions(List<string> permissions, int userId)
        {
            var hasPermission = false;
            if (StatusId == _settings.WorkflowStatusNPBorrador) //RequestNoteStatus.Borrador:
                hasPermission = CreationUserId == userId;

            else if (StatusId == _settings.WorkflowStatusNPPendienteAprobacionGerente) //case RequestNoteStatus.PendienteAprobaciónGerentesAnalítica:
                hasPermission = permissions.Any(p => p == "NP_APROBACION_GERE") && Analytics != null && Analytics.Any(a => a.ManagerId == userId);
            else if (StatusId == _settings.WorkflowStatusNPPendienteAprobacionDAF) // case RequestNoteStatus.PendienteAprobaciónDAF:
                hasPermission = permissions.Any(p => p == "NP_APROBACION_DAF");
            else if (StatusId == _settings.WorkflowStatusNPPendienteAprobacionCompras)
                hasPermission = permissions.Any(p => p == "NP_APROBACION_COMPRA");
            else if (new List<int>() {
                    _settings.WorkflowStatusNPPendienteAprobacionSAP,
                    _settings.WorkflowStatusNPPendienteRecepcionMerc,
                    _settings.WorkflowStatusNPRecepcionParcial
            }.Contains(StatusId))
                hasPermission = permissions.Any(p => p == "NP_CERRAR");

            return hasPermission;
        }
    }
    public class Analytic
    {
        public int AnalyticId { get; set; }

        public string Description { get; set; }
        public int Asigned { get; set; }

        public string Status { get; set; }

        [JsonIgnore]
        public int? ManagerId { get; set; }
    }

    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Sector { get; set; }
    }

    public class ProductsService
    {
        public string ProductService { get; set; }
        public int Quantity { get; set; }
    }

    public class Provider
    {
        public int ProviderId { get; set; }
        public string ProviderDescription { get; set; }
        public int? FileId { get; set; }
        public string FileDescription { get; set; }
        public decimal? Ammount { get; set; }
    }
    public class File
    {
        public int Type { get; set; }
        public string TypeDescription { get; set; }
        public int? FileId { get; set; }
        public string FileDescription { get; set; }
    }

    public class Training
    {
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
        public string Duration { get; set; }
        public decimal Ammount { get; set; }
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

    public class History
    {
        public DateTime CreatedDate { get; set; }

        public string UserName { get; set; }

        public int StatusFromId { get; set; }
        public string StatusFromDescription { get; set; }

        public int StatusToId { get; set; }
        public string StatusToDescription { get; set; }

        public string Comment { get; set; }
    }


}
