using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.RequestNote;

namespace Sofco.Framework.Workflow.OnSuccess
{
    public class UploadFilesRequestNote : IOnTransitionSuccessState
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly AppSetting appSetting;

        public UploadFilesRequestNote(IUnitOfWork unitOfWork, AppSetting appSetting)
        {
            this.unitOfWork = unitOfWork;
            this.appSetting = appSetting;
        }

        public void Process(WorkflowEntity entity, WorkflowChangeStatusParameters parameters)
        {
            var req = (RequestNote) entity;
            if (parameters is WorkflowChangeRequestNoteParameters)
            {
                if (((WorkflowChangeRequestNoteParameters)parameters).RequestNote?.Attachments == null)
                    ((WorkflowChangeRequestNoteParameters)parameters).RequestNote.Attachments = new List<Core.Models.RequestNote.File>();
                foreach (var a in ((WorkflowChangeRequestNoteParameters)parameters).RequestNote.Attachments)
                {
                    var an = req.Attachments.SingleOrDefault(p => p.FileId == a.FileId);
                    if (an == null)
                    {
                        an = new RequestNoteFile() { FileId = a.FileId.Value, Type = a.Type };
                        req.Attachments.Add(an);
                    }
                }
                unitOfWork.RequestNoteRepository.UpdateRequestNote(req);
                unitOfWork.RequestNoteRepository.Save();
            }
        }
    }
}
