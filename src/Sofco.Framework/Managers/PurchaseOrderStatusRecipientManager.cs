using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Managers;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Billing;

namespace Sofco.Framework.Managers
{
    public class PurchaseOrderStatusRecipientManager : IPurchaseOrderStatusRecipientManager
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly EmailConfig emailConfig;

        private readonly AppSetting appSetting;

        private readonly IUserData userData;

        public PurchaseOrderStatusRecipientManager(IUnitOfWork unitOfWork, IOptions<EmailConfig> emailConfigOptions, IUserData userData, IOptions<AppSetting> appSettingOptions)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            emailConfig = emailConfigOptions.Value;
            appSetting = appSettingOptions.Value;
        }

        public List<string> GetRecipientsCompliance()
        {
            var mails = GetComplianceMails();

            return mails.Distinct().ToList();
        }

        public List<string> GetRecipientsCommercial(PurchaseOrder purchaseOrder)
        {
            return GetCommercialMails(purchaseOrder);
        }

        public List<string> GetRejectCompliance()
        {
            var mails = GetCdgMails();

            mails.AddRange(GetComplianceMails());

            return mails;
        }

        public List<string> GetRecipientsOperation(PurchaseOrder purchaseOrder)
        {
            var mails = GetOperationMails(purchaseOrder);

            return mails;
        }

        public List<string> GetRejectCommercial(PurchaseOrder purchaseOrder)
        {
            var mails = GetCdgMails();

            mails.AddRange(GetComplianceMails());

            mails.AddRange(GetCommercialMails(purchaseOrder));

            return mails;
        }

        public List<string> GetRecipientsDaf()
        {
            var mails = GetDafMails();

            return mails;
        }

        public List<string> GetRejectOperation(PurchaseOrder purchaseOrder)
        {
            var mails = GetCdgMails();

            mails.AddRange(GetComplianceMails());

            mails.AddRange(GetCommercialMails(purchaseOrder));

            mails.AddRange(GetOperationMails(purchaseOrder));

            return mails;
        }

        public List<string> GetRecipientsFinalApproval(PurchaseOrder purchaseOrder)
        {
            var mails = new List<string>();

            mails.AddRange(GetCdgMails());

            mails.AddRange(GetComplianceMails());

            mails.AddRange(GetCommercialMails(purchaseOrder));

            mails.AddRange(GetOperationMails(purchaseOrder));

            //mails.AddRange(GetDafMails());

            var analytics = unitOfWork.PurchaseOrderRepository.GetByAnalyticsWithManagers(purchaseOrder.Id);

            foreach (var analytic in analytics)
            {
                if (analytic.CommercialManager != null) mails.Add(analytic.CommercialManager.Email);
                if (analytic.Manager != null) mails.Add(analytic.Manager.Email);
            }

            var humanResourceManager = unitOfWork.UserRepository.Get(emailConfig.HumanResourceMangerId);
            var humanResourceProjectLeader = unitOfWork.UserRepository.Get(emailConfig.HumanResourceProjectLeaderId);

            if (humanResourceManager != null) mails.Add(humanResourceManager.Email);
            if (humanResourceProjectLeader != null) mails.Add(humanResourceProjectLeader.Email);

            return mails;
        }

        public List<string> GetRejectDaf(PurchaseOrder purchaseOrder)
        {
            var mails = GetCdgMails();

            mails.AddRange(GetComplianceMails());

            mails.AddRange(GetCommercialMails(purchaseOrder));

            mails.AddRange(GetOperationMails(purchaseOrder));

            mails.AddRange(GetDafMails());

            return mails;
        }

        private List<string> GetCommercialMails(PurchaseOrder purchaseOrder)
        {
            var area = unitOfWork.AreaRepository.GetWithResponsable(purchaseOrder.AreaId.GetValueOrDefault());

            var responsableUser = area.ResponsableUser;

            var mails = new List<string> { responsableUser.Email };

            var userIds = unitOfWork.UserDelegateRepository.GetByTypeAndSourceId(UserDelegateType.PurchaseOrderApprovalCommercial,
                    responsableUser.Id)
                .Select(s => s.UserId);

            mails.AddRange(userIds.Select(userId => userData.GetById(userId))
                .Select(delegated => delegated.Email));

            return mails;
        }

        private List<string> GetCdgMails()
        {
            var mails = new List<string>();

            var cdgUsers = unitOfWork.UserRepository.GetByGroup(emailConfig.CdgCode);

            mails.AddRange(cdgUsers.Select(s => s.Email).ToList());

            return mails;
        }

        private List<string> GetOperationMails(PurchaseOrder purchaseOrder)
        {
            var mails = new List<string>();

            var analytics = unitOfWork.PurchaseOrderRepository.GetByAnalyticsWithSectors(purchaseOrder.Id);

            var users = analytics.Select(analytic => analytic.Sector?.ResponsableUser).ToList();

            mails.AddRange(users.Select(s => s.Email).ToList());

            foreach (var user in users)
            {
                var userIds = unitOfWork.UserDelegateRepository.GetByTypeAndSourceId(UserDelegateType.PurchaseOrderApprovalOperation,
                        user.Id)
                    .Select(s => s.UserId);

                mails.AddRange(userIds.Select(userId => userData.GetById(userId))
                    .Select(delegated => delegated.Email));
            }

            return mails;
        }

        private List<string> GetDafMails()
        {
            var mails = new List<string>();

            var users = unitOfWork.UserRepository.GetByGroup(appSetting.DafPurchaseOrderGroupCode);

            mails.AddRange(users.Select(s => s.Email).ToList());

            foreach (var user in users)
            {
                var userIds = unitOfWork.UserDelegateRepository.GetByTypeAndSourceId(UserDelegateType.PurchaseOrderApprovalDaf,
                        user.Id)
                    .Select(s => s.UserId);

                mails.AddRange(userIds.Select(userId => userData.GetById(userId))
                    .Select(delegated => delegated.Email));
            }

            return mails;
        }

        private List<string> GetComplianceMails()
        {
            var mails = new List<string>();

            var users = unitOfWork.UserRepository.GetByGroup(emailConfig.ComplianceCode);

            mails.AddRange(users.Select(s => s.Email).ToList());

            foreach (var user in users)
            {
                var userIds = unitOfWork.UserDelegateRepository.GetByTypeAndSourceId(UserDelegateType.PurchaseOrderApprovalCompliance,
                        user.Id)
                    .Select(s => s.UserId);

                mails.AddRange(userIds.Select(userId => userData.GetById(userId))
                    .Select(delegated => delegated.Email));
            }

            return mails;
        }
    }
}
