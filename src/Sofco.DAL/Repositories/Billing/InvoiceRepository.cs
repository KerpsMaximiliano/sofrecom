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
    public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(SofcoContext context) : base(context)
        {
        }

        public IList<Invoice> GetByProject(string projectId)
        {
            IQueryable<Invoice> query = _context.Invoices;

            query = GetProperties(query);

            return query.Where(x => x.ProjectId == projectId).ToList();
        }

        public Invoice GetById(int id)
        {
            IQueryable<Invoice> query = _context.Invoices.Include(x => x.User);
                
            query = GetProperties(query);

            return query.SingleOrDefault(x => x.Id == id);
        }

        public bool Exist(int invoiceId)
        {
            return _context.Invoices.Any(x => x.Id == invoiceId);
        }

        public void UpdateStatus(Invoice invoice)
        {
            _context.Entry(invoice).Property("InvoiceStatus").IsModified = true;
        }

        public Invoice GetExcel(int invoiceId)
        {
            return _context.Invoices.Select(x => new Invoice
                {
                    Id = x.Id,
                    ExcelFile = x.ExcelFile,
                    ExcelFileName = x.ExcelFileName
                })
                .SingleOrDefault(x => x.Id == invoiceId);
        }

        public Invoice GetPdf(int invoiceId)
        {
            return _context.Invoices.Select(x => new Invoice
                {
                    Id = x.Id,
                    PdfFile = x.PdfFile,
                    PdfFileName = x.PdfFileName
                })
                .SingleOrDefault(x => x.Id == invoiceId);
        }

        public void UpdateExcel(Invoice invoice)
        {
            _context.Entry(invoice).Property("ExcelFile").IsModified = true;
            _context.Entry(invoice).Property("ExcelFileName").IsModified = true;
            _context.Entry(invoice).Property("ExcelFileCreatedDate").IsModified = true;
        }

        public void UpdatePdf(Invoice invoice)
        {
            _context.Entry(invoice).Property("PdfFile").IsModified = true;
            _context.Entry(invoice).Property("PdfFileName").IsModified = true;
            _context.Entry(invoice).Property("PdfFileCreatedDate").IsModified = true;
        }

        public void UpdateStatusAndApprove(Invoice invoice)
        {
            _context.Entry(invoice).Property("InvoiceNumber").IsModified = true;
            UpdateStatus(invoice);
        }

        public IList<Invoice> GetOptions(string projectId)
        {
            return _context.Invoices
                .Include(x => x.Solfac)
                .Where(x => x.Solfac == null && x.InvoiceStatus == InvoiceStatus.Approved && x.ProjectId == projectId).ToList();
        }

        public bool InvoiceNumberExist(string invoiceNumber)
        {
            return _context.Invoices.Any(x => x.InvoiceNumber == invoiceNumber);
        }

        private IQueryable<Invoice> GetProperties(IQueryable<Invoice> dbset)
        {
            return dbset.Select(x => new Invoice
            {
                Id = x.Id,
                AccountName = x.AccountName,
                Project = x.Project,
                Address = x.Address,
                City = x.City,
                Cuit = x.Cuit,
                Country = x.Country,
                Province = x.Province,
                Analytic = x.Analytic,
                Zipcode = x.Zipcode,
                CreatedDate = x.CreatedDate,
                Service = x.Service,
                ProjectId = x.ProjectId,
                ExcelFileName = x.ExcelFileName,
                PdfFileName = x.PdfFileName,
                ExcelFileCreatedDate = x.ExcelFileCreatedDate,
                PdfFileCreatedDate = x.PdfFileCreatedDate,
                InvoiceStatus = x.InvoiceStatus,
                InvoiceNumber = x.InvoiceNumber,
                UserId = x.UserId,
                User = x.User,
                CustomerId = x.CustomerId,
                ServiceId = x.ServiceId
            });
        }

        public ICollection<Invoice> SearchByParams(InvoiceParams parameters)
        {
            IQueryable<Invoice> query = _context.Invoices;

            query = ApplyFilters(parameters, query).Include(x => x.User);

            return query.Select(x => new Invoice
                        {
                            Id = x.Id,
                            InvoiceNumber = x.InvoiceNumber,
                            AccountName = x.AccountName,
                            Service = x.Service,
                            Project = x.Project,
                            ProjectId = x.ProjectId,
                            User = x.User,
                            CreatedDate = x.CreatedDate,
                            InvoiceStatus = x.InvoiceStatus
                        })
                        .ToList();
        }

        private static IQueryable<Invoice> ApplyFilters(InvoiceParams parameters, IQueryable<Invoice> query)
        {
            query = query.Where(x => x.CreatedDate.Date >= parameters.DateSince.Date && x.CreatedDate.Date <= parameters.DateTo.Date);

            if (!string.IsNullOrWhiteSpace(parameters.CustomerId) && !parameters.CustomerId.Equals("0"))
                query = query.Where(x => x.CustomerId == parameters.CustomerId);

            if (!string.IsNullOrWhiteSpace(parameters.ServiceId) && !parameters.ServiceId.Equals("0"))
                query = query.Where(x => x.ServiceId == parameters.ServiceId);

            if (!string.IsNullOrWhiteSpace(parameters.ProjectId) && !parameters.ProjectId.Equals("0"))
                query = query.Where(x => x.ProjectId == parameters.ProjectId);

            if (!string.IsNullOrWhiteSpace(parameters.InvoiceNumber))
                query = query.Where(x => x.InvoiceNumber.ToLowerInvariant().Equals(parameters.InvoiceNumber.ToLowerInvariant()));

            if (parameters.userApplicantId > 0)
                query = query.Where(x => x.UserId == parameters.userApplicantId);

            if (parameters.Status != null)
                query = query.Where(x => x.InvoiceStatus == parameters.Status);

            return query;
        }

        public ICollection<Invoice> SearchByParamsAndUser(InvoiceParams parameters, string userMail)
        {
            IQueryable<Invoice> query = _context.Invoices.Include(x => x.User);

            query = query.Where(x => x.User.Email == userMail);

            query = ApplyFilters(parameters, query);

            return query.Select(x => new Invoice
                        {
                            Id = x.Id,
                            InvoiceNumber = x.InvoiceNumber,
                            AccountName = x.AccountName,
                            Service = x.Service,
                            Project = x.Project,
                            ProjectId = x.ProjectId,
                            User = x.User,
                            CreatedDate = x.CreatedDate,
                            InvoiceStatus = x.InvoiceStatus
                        })
                        .ToList();
        }

        public void AddHistory(InvoiceHistory history)
        {
            _context.InvoiceHistories.Add(history);
        }

        public ICollection<InvoiceHistory> GetHistories(int id)
        {
            return _context.InvoiceHistories.Where(x => x.InvoiceId == id).Include(x => x.User).ToList().AsReadOnly();
        }
    }
}