using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Billing;
using Sofco.DAL.Repositories.Common;
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
            IQueryable<Invoice> query = _context.Invoices;
                
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
                InvoiceNumber = x.InvoiceNumber
            });
        }
    }
}
