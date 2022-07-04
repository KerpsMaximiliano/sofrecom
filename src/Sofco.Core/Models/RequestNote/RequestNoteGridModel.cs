using Newtonsoft.Json;
using Sofco.Domain.RequestNoteStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sofco.Core.Models.RequestNote
{
    
    public class RequestNoteGridModel
    {
        public RequestNoteGridModel() { }
        public RequestNoteGridModel(Domain.Models.RequestNote.RequestNote note, List<string> permissions, int userId)
        {
            Id = note.Id;
            
            Description = note.Description;
            CreationUserId = note.CreationUserId;
            CreationUserDescription = note.CreationUser?.UserName;
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
            var hasPermission = false;
            switch ((RequestNoteStates)StatusId)
            {
                case RequestNoteStates.Borrador:
                    hasPermission = CreationUserId == userId;
                    break;
                case RequestNoteStates.PendienteRevisiónAbastecimiento:
                    hasPermission = permissions.Any(p => p == "NP_REVISION_ABAS");
                    break;
                case RequestNoteStates.PendienteAprobaciónGerentesAnalítica:
                    hasPermission = permissions.Any(p => p == "NP_APROBACION_GERENTE") && AnalyticsManagers.Any(a=> a == userId);
                    break;
                case RequestNoteStates.PendienteAprobaciónAbastecimiento:
                    hasPermission = permissions.Any(p => p == "NP_APROBACION_ABAS");
                    break;
                case RequestNoteStates.PendienteAprobaciónDAF:
                    hasPermission = permissions.Any(p => p == "NP_APROBACION_DAF");
                    break;
                case RequestNoteStates.Aprobada:
                    hasPermission = permissions.Any(p => p == "NP_ENVIO_PROV_ABAS");
                    break;
                case RequestNoteStates.SolicitadaAProveedor:
                    hasPermission = permissions.Any(p => p == "NP_RECEP_ABAS");
                    break;
                case RequestNoteStates.RecibidoConforme:
                    hasPermission = permissions.Any(p => p == "NP_CONFORME_ABAS");
                    break;
                case RequestNoteStates.FacturaPendienteAprobaciónGerente:
                    hasPermission = permissions.Any(p => p == "NP_FAC_APROB_GERENTE") && AnalyticsManagers.Any(a => a == userId);
                    break;
                case RequestNoteStates.PendienteProcesarGAF:
                    hasPermission = permissions.Any(p => p == "NP_PROCESAR_GAF");
                    break;
                case RequestNoteStates.Rechazada:
                    break;
                case RequestNoteStates.Cerrada:
                    break;
                default:
                    break;
            }
            return hasPermission;
        }
        public int? Id { get; set; }

        public bool HasEditPermissions { get; set; }
        public bool HasReadPermissions { get; set; }
        public string Description { get; set; }
        
        public int CreationUserId { get; set; }
        public string CreationUserDescription { get; set; }
        public DateTime CreationDate { get; set; }

        public int StatusId { get; set; }
        public string StatusDescription { get; set; }

        [JsonIgnore]
        public List<int> AnalyticsManagers { get; set; }
    }
   
}
