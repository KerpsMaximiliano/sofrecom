using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.RequestNote;
using Sofco.Core.Services.RequestNote;
using Sofco.Domain.Models.RequestNote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sofco.Service.Implementations.RequestNote
{
    public class RequestNoteHistoryService : IRequestNoteHistoryService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<RequestNoteService> logger;

        public RequestNoteHistoryService(IUnitOfWork unitOfWork, ILogMailer<RequestNoteService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public IList<History> GetByRequestNoteId(int id)
        {
            var historias = this.unitOfWork.RequestNoteHistoryRepository.GetByRequestNoteId(id);
            return historias.Select(h => new History()
            {
                Comment = h.Comment,
                StatusFromId = h.StatusFromId,
                CreatedDate = h.CreatedDate,
                StatusToId = h.StatusToId,
                UserName = h.UserName,
                StatusFromDescription = h.StatusFrom?.Name,
                StatusToDescription = h.StatusTo?.Name
            }).ToList();
        }

    }
}
