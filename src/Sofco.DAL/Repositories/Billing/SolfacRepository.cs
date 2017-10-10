using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;

namespace Sofco.DAL.Repositories.Billing
{
    public class SolfacRepository : BaseRepository<Solfac>, ISolfacRepository
    {
        public SolfacRepository(SofcoContext context) : base(context)
        {
        }

        public IList<Solfac> GetAllWithDocuments()
        {
            return _context.Solfacs.Include(x => x.DocumentType).ToList();
        }

        public IList<Hito> GetHitosByProject(string projectId)
        {
            return _context.Hitos.Where(x => x.ExternalProjectId.Equals(projectId)).ToList();
        }

        public IList<Solfac> GetByProject(string projectId)
        {
            return _context.Solfacs.Where(x => x.ProjectId == projectId).Include(x => x.DocumentType).ToList();
        }

        public Solfac GetByIdWithUser(int id)
        {
            return _context.Solfacs
                .Include(x => x.UserApplicant)
                .SingleOrDefault(x => x.Id == id);
        }

        public ICollection<string> GetHitosIdsBySolfacId(int solfacId)
        {
            return _context.Hitos
                .Where(x => x.SolfacId == solfacId)
                .Select(x => x.ExternalHitoId)
                .ToList();
        }

        public Solfac GetById(int id)
        {
            return _context.Solfacs
                .Include(x => x.DocumentType)
                .Include(x => x.Currency)
                .Include(x => x.UserApplicant)
                .Include(x => x.ImputationNumber)
                .Include(x => x.Hitos)
                .Include(x => x.Invoice)
                .SingleOrDefault(x => x.Id == id);
        }

        public ICollection<SolfacHistory> GetHistories(int solfacId)
        {
            return _context.SolfacHistories.Where(x => x.SolfacId == solfacId).Include(x => x.User).ToList().AsReadOnly();
        }

        public void AddHistory(SolfacHistory history)
        {
            _context.SolfacHistories.Add(history);
        }

        public void SaveAttachment(SolfacAttachment attachment)
        {
            _context.SolfacAttachments.Add(attachment);
        }

        public ICollection<SolfacAttachment> GetFiles(int solfacId)
        {
            return _context.SolfacAttachments
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
            return _context.SolfacAttachments.SingleOrDefault(x => x.Id == fileId);
        }

        public void DeleteFile(SolfacAttachment file)
        {
            _context.SolfacAttachments.Remove(file);
        }

        public void UpdateStatus(Solfac solfac)
        {
            _context.Entry(solfac).Property("Status").IsModified = true;
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

        public IList<Solfac> SearchByParamsAndUser(SolfacParams parameters, string userMail)
        {
            IQueryable<Solfac> query = _context.Solfacs.Include(x => x.DocumentType).Include(x => x.UserApplicant);

            query = query.Where(x => x.UserApplicant.Email == userMail);

            query = ApplyFilters(parameters, query);

            return query.ToList();
        }

        public IList<Solfac> SearchByParams(SolfacParams parameters)
        {
            IQueryable<Solfac> query = _context.Solfacs.Include(x => x.DocumentType).Include(x => x.UserApplicant);

            query = ApplyFilters(parameters, query);

            return query.ToList();
        }

        private static IQueryable<Solfac> ApplyFilters(SolfacParams parameters, IQueryable<Solfac> query)
        {
            query = query.Where(x => x.StartDate.Date >= parameters.DateSince.Date && x.StartDate.Date <= parameters.DateTo.Date);

            if (!string.IsNullOrWhiteSpace(parameters.CustomerId) && !parameters.CustomerId.Equals("0"))
                query = query.Where(x => x.CustomerId == parameters.CustomerId);

            if (!string.IsNullOrWhiteSpace(parameters.ServiceId) && !parameters.ServiceId.Equals("0"))
                query = query.Where(x => x.ServiceId == parameters.ServiceId);

            if (!string.IsNullOrWhiteSpace(parameters.ProjectId) && !parameters.ProjectId.Equals("0"))
                query = query.Where(x => x.ProjectId == parameters.ProjectId);

            if (!string.IsNullOrWhiteSpace(parameters.Analytic))
                query = query.Where(x => x.Analytic.ToLowerInvariant().Equals(parameters.Analytic.ToLowerInvariant()));

            if (parameters.UserApplicantId > 0)
                query = query.Where(x => x.UserApplicantId == parameters.UserApplicantId);

            if (parameters.Status != SolfacStatus.None)
                query = query.Where(x => x.Status == parameters.Status);

            return query;
        }

        public void UpdateInvoice(Solfac solfac)
        {
            _context.Entry(solfac).Property("InvoiceCode").IsModified = true;
            _context.Entry(solfac).Property("InvoiceDate").IsModified = true;
        }

        public void UpdateCash(Solfac solfac)
        {
            _context.Entry(solfac).Property("CashedDate").IsModified = true;
        }

        public bool InvoiceCodeExist(string invoiceCode)
        {
            return _context.Solfacs.Any(x => x.InvoiceCode == invoiceCode);
        }
    }
}
