using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Options;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Models.Admin;
using Sofco.Core.Services.Billing.PurchaseOrder;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.Billing.PurchaseOrder
{
    public class PurchaseOrderApprovalDelegateService : IPurchaseOrderApprovalDelegateService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ISessionManager sessionManager;
        private readonly IMapper mapper;
        private readonly EmailConfig emailConfig;

        public PurchaseOrderApprovalDelegateService(IUnitOfWork unitOfWork,IMapper mapper, ISessionManager sessionManager, IOptions<EmailConfig> emailOptions)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionManager = sessionManager;
            emailConfig = emailOptions.Value;
        }

        public Response<List<UserSelectListItem>> GetComplianceUsers()
        {
            var result = unitOfWork.UserRepository.GetByGroup(emailConfig.ComplianceCode)
                .Where(s => s.Email != sessionManager.GetUserEmail())
                .ToList();

            return new Response<List<UserSelectListItem>>
            {
                Data = Translate(result)
            };
        }

        public Response<List<UserSelectListItem>> GetDafUsers()
        {
            var result = unitOfWork.UserRepository.GetByGroup(emailConfig.DafCode)
                .Where(s => s.Email != sessionManager.GetUserEmail())
                .ToList();

            return new Response<List<UserSelectListItem>>
            {
                Data = Translate(result)
            };
        }
     
        private List<UserSelectListItem> Translate(List<User> users)
        {
            return mapper.Map<List<User>, List<UserSelectListItem>>(users);
        }
    }
}
