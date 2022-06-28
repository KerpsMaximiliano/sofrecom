using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Providers;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.RequestNote
{
    public class RequestNote : WorkflowEntity
    {
        public string Description { get; set; }
        public int ProviderAreaId { get; set; }
        public ProvidersArea ProviderArea { get; set; }
        public bool RequiresEmployeeClient { get; set; }
        public bool ConsideredInBudget { get; set; }
        public int EvalpropNumber { get; set; }
        public string Comments { get; set; }

        public bool TravelSection { get; set; }
        //public int RequestNoteTravelId { get; set; }
        //public RequestNoteTravel RequestNoteTravel { get; set; }
        public IList<RequestNoteTravel> Travels { get; set; }

        public bool TrainingSection { get; set; }
        //public int RequestNoteTrainingId { get; set; }
        public IList<RequestNoteTraining> Trainings { get; set; }

        public decimal? PurchaseOrderAmmount { get; set; }
        public int? PurchaseOrderNumber { get; set; }
        public DateTime CreationDate { get; set; }
        public int CreationUserId { get; set; }
        public Admin.User CreationUser { get; set; }
        public int WorkflowId { get; set; }
        public Workflow.Workflow Workflow { get; set; }
        public IList<RequestNoteProductService> ProductsServices { get; set; }
        public IList<RequestNoteFile> Attachments { get; set; }
        //public int AnalyticId { get; set; }
        public IList<RequestNoteAnalytic> Analytics { get; set; }
        public IList<RequestNoteProvider> Providers { get; set; }
        [NotMapped]
        public string UsersAlreadyApproved { get; set; }

        public IList<RequestNoteHistory> Histories { get; set; }
    }
}
