using Newtonsoft.Json;
using Sofco.Common.Settings;
using Sofco.Domain.RequestNoteStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sofco.Core.Models.RequestNote
{
    
    public class RequestNoteGridModel
    {

        private readonly AppSetting _settings;
        public RequestNoteGridModel() { }
        public RequestNoteGridModel(Domain.Models.RequestNote.RequestNote note, List<string> permissions, int userId, AppSetting settings)
        {
            _settings = settings;
            Id = note.Id;
            
            Description = note.Description;
            CreationUserId = note.CreationUserId;
            CreationUserDescription = note.CreationUser?.UserName;
            CreationUserName = note.CreationUser.Name;
            CreationDate = note.CreationDate;
            AnalyticsManagers = note.Analytics != null 
                ?  note.Analytics.Where(a=> a.Analytic?.ManagerId != null).Select(a => a.Analytic.ManagerId.Value).ToList() 
                : new List<int>();
            StatusId = note.StatusId;
            StatusDescription = note.Status?.Name;
            HasReadPermissions = ValidateReadPermissions(permissions, userId);
            HasEditPermissions = ValidateEditPermissions(permissions, userId);
            
        }
        private bool ValidateReadPermissions(List<string> permissions, int userId)
        {
            return CreationUserId == userId || permissions.Any(p => p == "NP_READONLY");
        }
        private bool ValidateEditPermissions(List<string> permissions, int userId)
        {
            var hasPermission = CreationUserId == userId; //Si es suya, siempre la puede ver
            if (hasPermission && StatusId == _settings.WorkflowStatusNPBorrador)
                return hasPermission;

            if (StatusId == _settings.WorkflowStatusNPPendienteAprobacionGerente) //case RequestNoteStatus.PendienteAprobaciónGerentesAnalítica:
                hasPermission = hasPermission || (permissions.Any(p => p == "NP_APROBACION_GERE") && AnalyticsManagers.Any(a => a == userId));
            else if (StatusId == _settings.WorkflowStatusNPPendienteAprobacionDAF) // case RequestNoteStatus.PendienteAprobaciónDAF:
                hasPermission = permissions.Any(p => p == "NP_APROBACION_DAF");
            else if (StatusId == _settings.WorkflowStatusNPPendienteAprobacionCompras) // case RequestNoteStatus.PendienteAprobaciónDAF:
                hasPermission = permissions.Any(p => p == "NP_APROBACION_COMPRA");
            else if (StatusId == _settings.WorkflowStatusNPPendienteAprobacionSAP) // case RequestNoteStatus.PendienteAprobaciónDAF:
                hasPermission = permissions.Any(p => p == "NP_PEND_GENE_SAP");
            else if (new List<int>() {
                    _settings.WorkflowStatusNPPendienteRecepcionMerc,
                    _settings.WorkflowStatusNPRecepcionParcial
            }.Contains(StatusId))
                hasPermission = permissions.Any(p => p == "NP_CERRAR");

            return hasPermission;
        }
        public int? Id { get; set; }

        public bool HasEditPermissions { get; set; }
        public bool HasReadPermissions { get; set; }
        public string Description { get; set; }
        
        public int CreationUserId { get; set; }
        public string CreationUserDescription { get; set; }

        public string CreationUserName { get; set; }
        public DateTime CreationDate { get; set; }

        public int StatusId { get; set; }
        public string StatusDescription { get; set; }

        [JsonIgnore]
        public List<int> AnalyticsManagers { get; set; }
    }
   
}
