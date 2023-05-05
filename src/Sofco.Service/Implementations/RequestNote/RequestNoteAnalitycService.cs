using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.Models.RequestNote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sofco.Service.Implementations.RequestNote
{
    public class RequestNoteAnalitycService : IRequestNoteAnalitycService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<RequestNoteAnalitycService> logger;

        public RequestNoteAnalitycService(IUnitOfWork unitOfWork, ILogMailer<RequestNoteAnalitycService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public void ChangeStatus(int id, string status)
        {
            Domain.Models.RequestNote.RequestNoteAnalytic requestNoteAnalityc = this.unitOfWork.RequestNoteAnalitycRepository.GetById(id);

            requestNoteAnalityc.Status = status;

            this.unitOfWork.RequestNoteAnalitycRepository.UpdateAnalityc(requestNoteAnalityc);
            this.unitOfWork.RequestNoteAnalitycRepository.Save();
        }

        public void ChangeStatusByRequestNodeId(int requestNoteId, string status)
        {
            List<RequestNoteAnalytic> requestNoteAnalytics = GetByRequestNoteId(requestNoteId);

            foreach(RequestNoteAnalytic requestNoteAnalytic in requestNoteAnalytics)
            {
                ChangeStatus(requestNoteAnalytic.Id, status);
            }
        }

        public List<ManagerAnalyticStatusDto> GetApprovedManageners(int requestNoteId)
        {
            List<RequestNoteAnalytic> requestNoteAnalytics = this.unitOfWork.RequestNoteAnalitycRepository.GetManagersByRequestNoteId(requestNoteId);
            return requestNoteAnalytics.GroupBy(r => r.Analytic.Manager.Name)
                                                          .Select(g => new ManagerAnalyticStatusDto
                                                          {
                                                              Name = g.Key,
                                                              Status = g.FirstOrDefault()?.Status
                                                          })
                                                          .OrderByDescending(r => r.Status)
                                                          .ToList();
        }

        public List<RequestNoteAnalytic> GetByRequestNoteId(int requestNoteId)
        {
            return this.unitOfWork.RequestNoteAnalitycRepository.GetByRequestNoteId(requestNoteId);
        }
    }
}
