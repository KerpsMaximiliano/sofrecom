﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Managers;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;

namespace Sofco.Framework.Managers
{
    public class PurchaseOrderStatusRecipientManager : IPurchaseOrderStatusRecipientManager
    {
        private const string MailDelimiter = ";";

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

        public string GetRecipientsCompliance()
        {
            var mails = GetComplianceMails();

            return string.Join(MailDelimiter, mails.Distinct());
        }

        public string GetRecipientsCommercial(PurchaseOrder purchaseOrder)
        {
            var mails = GetCommercialMails(purchaseOrder);

            return string.Join(MailDelimiter, mails.Distinct());
        }

        public string GetRejectCompliance()
        {
            var mails = GetCdgMails();

            mails.AddRange(GetComplianceMails());

            return string.Join(MailDelimiter, mails.Distinct());
        }

        public string GetRecipientsOperation(PurchaseOrder purchaseOrder)
        {
            var mails = GetOperationMails(purchaseOrder);

            return string.Join(MailDelimiter, mails.Distinct());
        }

        public string GetRejectCommercial(PurchaseOrder purchaseOrder)
        {
            var mails = GetCdgMails();

            mails.AddRange(GetComplianceMails());

            mails.AddRange(GetCommercialMails(purchaseOrder));

            return string.Join(MailDelimiter, mails.Distinct());
        }

        public string GetRecipientsDaf()
        {
            var mails = GetDafMails();

            return string.Join(MailDelimiter, mails.Distinct());
        }

        public string GetRejectOperation(PurchaseOrder purchaseOrder)
        {
            var mails = GetCdgMails();

            mails.AddRange(GetComplianceMails());

            mails.AddRange(GetCommercialMails(purchaseOrder));

            mails.AddRange(GetOperationMails(purchaseOrder));

            return string.Join(MailDelimiter, mails.Distinct());
        }

        public string GetRecipientsFinalApproval(PurchaseOrder purchaseOrder)
        {
            var mails = new List<string>();

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

            var cdgUsers = unitOfWork.UserRepository.GetByGroup(emailConfig.CdgCode);

            mails.AddRange(cdgUsers.Select(s => s.Email).ToList());

            return string.Join(MailDelimiter, mails.Distinct());
        }

        public string GetRejectDaf(PurchaseOrder purchaseOrder)
        {
            var mails = GetCdgMails();

            mails.AddRange(GetComplianceMails());

            mails.AddRange(GetCommercialMails(purchaseOrder));

            mails.AddRange(GetOperationMails(purchaseOrder));

            mails.AddRange(GetDafMails());

            return string.Join(MailDelimiter, mails.Distinct());
        }

        private List<string> GetCommercialMails(PurchaseOrder purchaseOrder)
        {
            var area = unitOfWork.AreaRepository.GetWithResponsable(purchaseOrder.AreaId.GetValueOrDefault());

            var responsableUser = area.ResponsableUser;

            var mails = new List<string> { responsableUser.Email };

            var userIds = unitOfWork.UserDelegateRepository.GetByTypeAndSourceId(UserDelegateType.PurchaseOrderCommercial,
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
                var userIds = unitOfWork.UserDelegateRepository.GetByTypeAndSourceId(UserDelegateType.PurchaseOrderOperation,
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
                var userIds = unitOfWork.UserDelegateRepository.GetByTypeAndSourceId(UserDelegateType.PurchaseOrderDaf,
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
                var userIds = unitOfWork.UserDelegateRepository.GetByTypeAndSourceId(UserDelegateType.PurchaseOrderCompliance,
                        user.Id)
                    .Select(s => s.UserId);

                mails.AddRange(userIds.Select(userId => userData.GetById(userId))
                    .Select(delegated => delegated.Email));
            }

            return mails;
        }
    }
}
