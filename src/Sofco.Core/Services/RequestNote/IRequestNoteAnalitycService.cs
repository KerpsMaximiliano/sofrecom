using Sofco.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Services.RequestNote
{
    public interface IRequestNoteAnalitycService
    {
        void ChangeStatusByRequestNodeId(int requestNoteId, string status);
        List<Domain.Models.RequestNote.RequestNoteAnalytic> GetByRequestNoteId(int requestNoteId);
        void ChangeStatus(int id, string status);
        List<ManagerAnalyticStatusDto> GetApprovedManageners(int requestNoteId);
    }
}
