﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Billing;
using Sofco.Core.Models.Admin;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Billing;

namespace Sofco.DAL.Repositories.Billing
{
    public class SolfacRepository : BaseRepository<Solfac>, ISolfacRepository
    {
        public SolfacRepository(SofcoContext context) : base(context)
        {
        }

        public IList<Solfac> GetAllWithDocuments()
        {
            return context.Solfacs.Include(x => x.DocumentType).ToList();
        }

        public IList<Hito> GetHitosByProject(string projectId)
        {
            return context.Hitos.Include(x => x.Solfac).Where(x => x.ProjectId.Equals(projectId)).ToList();
        }

        public IList<Solfac> GetByProject(string projectId)
        {
            return context.Solfacs.Where(x => x.ProjectId.Contains(projectId))
                .Include(x => x.DocumentType)
                .Include(x => x.PurchaseOrder)
                .ToList();
        }

        public Solfac GetByIdWithUser(int id)
        {
            return context.Solfacs
                .Include(x => x.UserApplicant)
                .Include(x => x.PurchaseOrder)
                    .ThenInclude(x => x.AmmountDetails)
                        .ThenInclude(x => x.Currency)
                .Include(x => x.Hitos)
                    .ThenInclude(x => x.Details)
                .SingleOrDefault(x => x.Id == id);
        }

        public ICollection<string> GetHitosIdsBySolfacId(int solfacId)
        {
            return context.Hitos
                .Where(x => x.SolfacId == solfacId)
                .Select(x => x.ExternalHitoId)
                .ToList();
        }

        public Solfac GetById(int id)
        {
            return context.Solfacs
                .Include(x => x.DocumentType)
                .Include(x => x.Currency)
                .Include(x => x.UserApplicant)
                .Include(x => x.ImputationNumber)
                .Include(x => x.PurchaseOrder)
                .Include(x => x.Hitos).ThenInclude(x => x.Details)
                .SingleOrDefault(x => x.Id == id);
        }

        public ICollection<SolfacHistory> GetHistories(int solfacId)
        {
            return context.SolfacHistories.Where(x => x.SolfacId == solfacId).Include(x => x.User).ToList().AsReadOnly();
        }

        public void AddHistory(SolfacHistory history)
        {
            context.SolfacHistories.Add(history);
        }

        public void SaveAttachment(SolfacAttachment attachment)
        {
            context.SolfacAttachments.Add(attachment);
        }

        public ICollection<SolfacAttachment> GetFiles(int solfacId)
        {
            return context.SolfacAttachments
                .Where(x => x.SolfacId == solfacId)
                .Select(x => new SolfacAttachment
                {
                    Id = x.Id,
                    Name = x.Name,
                    CreationDate = x.CreationDate
                })
                .ToList();
        }

        public SolfacAttachment GetFileById(int fileId)
        {
            return context.SolfacAttachments.SingleOrDefault(x => x.Id == fileId);
        }

        public void DeleteFile(SolfacAttachment file)
        {
            context.SolfacAttachments.Remove(file);
        }

        public void UpdateStatus(Solfac solfac)
        {
            context.Entry(solfac).Property("Status").IsModified = true;
        }

        public void UpdateStatusAndCashed(Solfac solfac)
        {
            UpdateCash(solfac);
            UpdateStatus(solfac);
        }

        public void UpdateStatusAndInvoice(Solfac solfac)
        {
            UpdateInvoice(solfac);
            UpdateStatus(solfac);
        }

        public IList<Solfac> SearchByParamsAndUser(SolfacParams parameters, UserLiteModel user)
        {
            IQueryable<Solfac> query = context.Solfacs.Include(x => x.DocumentType).Include(x => x.UserApplicant);

            if(string.IsNullOrWhiteSpace(user.ManagerId))
                query = query.Where(x => x.UserApplicant.Email == user.Email);
            else
                query = query.Where(x => x.ManagerId == user.ManagerId);

            query = ApplyFilters(parameters, query);

            return query.ToList();
        }

        public IList<Solfac> SearchByParams(SolfacParams parameters)
        {
            IQueryable<Solfac> query = context.Solfacs.Include(x => x.DocumentType).Include(x => x.UserApplicant);

            query = ApplyFilters(parameters, query);

            return query.ToList();
        }

        private IQueryable<Solfac> ApplyFilters(SolfacParams parameters, IQueryable<Solfac> query)
        {
            if(parameters.DateSince.HasValue && parameters.DateTo.HasValue)
                query = query.Where(x => x.StartDate.Date >= parameters.DateSince.GetValueOrDefault().Date && x.StartDate.Date <= parameters.DateTo.GetValueOrDefault().Date);

            if (!string.IsNullOrWhiteSpace(parameters.CustomerId) && !parameters.CustomerId.Equals("0"))
                query = query.Where(x => x.AccountId == parameters.CustomerId);

            if (!string.IsNullOrWhiteSpace(parameters.ServiceId) && !parameters.ServiceId.Equals("0"))
                query = query.Where(x => x.ServiceId == parameters.ServiceId);

            if (!string.IsNullOrWhiteSpace(parameters.ProjectId) && !parameters.ProjectId.Equals("0"))
                query = query.Where(x => x.ProjectId.Contains(parameters.ProjectId));

            if (parameters.Analytic.HasValue)
            {
                var analytic = context.Analytics.SingleOrDefault(x => x.Id == parameters.Analytic.Value);

                if (analytic != null)
                {
                    query = query.Where(x => x.Analytic.ToLowerInvariant().Equals(analytic.Title.ToLowerInvariant()));
                }
            }

            if (!string.IsNullOrWhiteSpace(parameters.ManagerId))
                query = query.Where(x => x.ManagerId == parameters.ManagerId);

            if (parameters.Status != SolfacStatus.None)
                query = query.Where(x => x.Status == parameters.Status);

            return query;
        }

        public void UpdateInvoice(Solfac solfac)
        {
            context.Entry(solfac).Property("InvoiceCode").IsModified = true;
            context.Entry(solfac).Property("InvoiceDate").IsModified = true;
        }

        public void UpdateCurrencyExchange(Solfac solfac)
        {
            context.Entry(solfac).Property("CurrencyExchange").IsModified = true;
        }

        public Hito GetHitoByCrmId(string hitoId)
        {
            return context.Hitos.Include(x => x.Solfac).SingleOrDefault(x => x.ExternalHitoId.Equals(hitoId));
        }

        public void UpdateCash(Solfac solfac)
        {
            context.Entry(solfac).Property("CashedDate").IsModified = true;
        }

        public bool InvoiceCodeExist(string invoiceCode, Solfac solfac)
        {
            return context.Solfacs.Any(x => x.InvoiceCode == invoiceCode && x.DocumentTypeId == solfac.DocumentTypeId && x.Id != solfac.Id);
        }

        public IList<Hito> GetHitosByExternalIds(List<Guid> externalIds)
        {
            var externalIdsText = externalIds.Select(s => s.ToString());

            return context.Hitos.Where(x => externalIdsText.Contains(x.ExternalHitoId)).ToList();
        }

        public HitoDetail GetDetail(int id)
        {
            return context.HitoDetails.SingleOrDefault(x => x.Id == id);
        }

        public void DeleteDetail(HitoDetail detail)
        {
            context.HitoDetails.Remove(detail);
        }

        public bool HasAttachments(int solfacId)
        {
            return context.Solfacs.Include(x => x.Attachments).SingleOrDefault(x => x.Id == solfacId).Attachments.Any();
        }

        public bool HasInvoices(int solfacId)
        {
            return context.Solfacs.Include(x => x.Invoices).SingleOrDefault(x => x.Id == solfacId).Invoices.Any();
        }

        public IList<Hito> GetHitosBySolfacId(int solfacId)
        {
            return context.Hitos.Where(x => x.SolfacId == solfacId).ToList();
        }

        public decimal GetTotalAmountById(int solfacId)
        {
            return context.Solfacs
                .Where(s => s.Id == solfacId)
                .Select(s => s.TotalAmount)
                .FirstOrDefault();
        }

        public IEnumerable<Solfac> GetByProjectWithPurchaseOrder(string projectId)
        {
            return context.Solfacs.Where(x => x.ProjectId.Contains(projectId))
                .Include(x => x.PurchaseOrder)
                    .ThenInclude(x => x.AmmountDetails)
                    .ThenInclude(x => x.Currency)
                .ToList();
        }
    }
}
