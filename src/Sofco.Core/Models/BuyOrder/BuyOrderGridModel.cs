using Newtonsoft.Json;
using Sofco.Domain.RequestNoteStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sofco.Core.Models.BuyOrder
{
    
    public class BuyOrderGridModel
    {
        public BuyOrderGridModel() { }
        public BuyOrderGridModel(Domain.Models.RequestNote.BuyOrder order, List<string> permissions, int userId)
        {
            Id = order.Id;
            
            Number = order.BuyOrderNumber;
            CreationUserId = order.CreationUserId;
            CreationUserDescription = order.CreationUser?.UserName;
            CreationDate = order.CreationDate;
            RequestNoteId = order.RequestNoteId;
            RequestNoteDescription = order.RequestNote?.Description;
            StatusId = order.StatusId;
            StatusDescription = order.Status?.Name;
            HasReadPermissions = ValidateReadPermissions(permissions, userId);
            HasEditPermissions = ValidateEditPermissions(permissions, userId);
            
        }
        private bool ValidateReadPermissions(List<string> permissions, int userId)
        {
            return CreationUserId == userId || permissions.Any(p => p == "OC_READONLY");
        }
        private bool ValidateEditPermissions(List<string> permissions, int userId)
        {
            var hasPermission = false;
            switch ((RequestNoteStatus)StatusId)
            {
                case RequestNoteStatus.Borrador:
                    hasPermission = CreationUserId == userId;
                    break;
                case RequestNoteStatus.PendienteRevisiónAbastecimiento:
                    hasPermission = permissions.Any(p => p == "NP_REVISION_ABAS");
                    break;
                case RequestNoteStatus.PendienteAprobaciónGerentesAnalítica:
                    hasPermission = permissions.Any(p => p == "NP_APROBACION_GERE") && AnalyticsManagers.Any(a=> a == userId);
                    break;
                case RequestNoteStatus.PendienteAprobaciónAbastecimiento:
                    hasPermission = permissions.Any(p => p == "NP_APROBACION_ABAS");
                    break;
                case RequestNoteStatus.PendienteAprobaciónDAF:
                    hasPermission = permissions.Any(p => p == "NP_APROBACION_DAF");
                    break;
                case RequestNoteStatus.Aprobada:
                    hasPermission = permissions.Any(p => p == "NP_ENVIO_PROV_ABAS");
                    break;
                case RequestNoteStatus.SolicitadaAProveedor:
                    hasPermission = permissions.Any(p => p == "NP_RECEP_ABAS");
                    break;
                case RequestNoteStatus.RecibidoConforme:
                    hasPermission = permissions.Any(p => p == "NP_CONFORME_ABAS");
                    break;
                case RequestNoteStatus.FacturaPendienteAprobaciónGerente:
                    hasPermission = permissions.Any(p => p == "NP_FAC_APROB_GERENTE") && AnalyticsManagers.Any(a => a == userId);
                    break;
                case RequestNoteStatus.PendienteProcesarGAF:
                    hasPermission = permissions.Any(p => p == "NP_PROCESAR_GAF");
                    break;
                case RequestNoteStatus.Rechazada:
                    break;
                case RequestNoteStatus.Cerrada:
                    break;
                default:
                    break;
            }
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

        [JsonIgnore]
        public List<int> AnalyticsManagers { get; set; }
    }
   
}
