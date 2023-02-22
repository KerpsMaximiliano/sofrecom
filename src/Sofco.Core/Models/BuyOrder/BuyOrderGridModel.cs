using Newtonsoft.Json;
using Sofco.Common.Settings;
using Sofco.Domain.RequestNoteStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sofco.Core.Models.BuyOrder
{
    
    public class BuyOrderGridModel
    {
        private readonly AppSetting _settings;
        public BuyOrderGridModel() { }
        public BuyOrderGridModel(Domain.Models.RequestNote.BuyOrder order, List<string> permissions, int userId, AppSetting settings)
        {
            Id = order.Id;
            _settings = settings;
            Number = order.BuyOrderNumber;
            CreationUserId = order.CreationUserId;
            CreationUserDescription = order.CreationUser?.UserName;
            CreationDate = order.CreationDate;
            RequestNoteId = order.RequestNoteId;
            RequestNoteDescription = order.RequestNote?.Description;
            StatusId = order.StatusId;
            StatusDescription = order.Status?.Name;
            ProviderId = order.ProviderId;
            ProviderDescription = order.Provider?.Name;
            HasReadPermissions = ValidateReadPermissions(permissions, userId);
            HasEditPermissions = ValidateEditPermissions(permissions, userId);
            
        }
        private bool ValidateReadPermissions(List<string> permissions, int userId)
        {
            return permissions.Any(p => p == "OC_READONLY");
        }
        private bool ValidateEditPermissions(List<string> permissions, int userId)
        {
            var hasPermission = false;
            if (StatusId == _settings.WorkflowStatusBOPendienteAprobacionDAF)
                hasPermission = permissions.Any(p => p == "OC_PEND_APR_DAF");
            else if (StatusId == _settings.WorkflowStatusBOPendienteRecepcionMerc)
                hasPermission = permissions.Any(p => p == "OC_PEND_REC_MERC");
            else if (StatusId == _settings.WorkflowStatusBOPendienteRecepcionFact)
                hasPermission = permissions.Any(p => p == "OC_PEND_REC_FACT");
            return hasPermission;
        }
        public int? Id { get; set; }

        public bool HasEditPermissions { get; set; }
        public bool HasReadPermissions { get; set; }
        public string Number { get; set; }
        
        public int CreationUserId { get; set; }
        public string CreationUserDescription { get; set; }
        public DateTime CreationDate { get; set; }

        public int StatusId { get; set; }
        public string StatusDescription { get; set; }

        public int RequestNoteId { get; set; }
        public string RequestNoteDescription { get; set; }

        public int ProviderId { get; set; }
        public string ProviderDescription { get; set; }

    }
   
}
