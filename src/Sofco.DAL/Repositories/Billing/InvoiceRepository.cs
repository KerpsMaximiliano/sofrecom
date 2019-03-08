using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Billing;

namespace Sofco.DAL.Repositories.Billing
{
    public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(SofcoContext context) : base(context)
        {
        }

        public IList<Invoice> GetByProject(string projectId)
        {
            IQueryable<Invoice> query = context.Invoices.Include(x => x.ExcelFileData).Include(x => x.PDfFileData);

            //query = GetProperties(query);

            return query.Where(x => x.ProjectId == projectId).ToList();
        }

        public Invoice GetById(int id)
        {
            IQueryable<Invoice> query = context.Invoices
                .Include(x => x.User)
                .Include(x => x.PDfFileData)
                .Include(x => x.ExcelFileData)
                .Include(x => x.Solfac);

            //query = GetProperties(query);

            return query.SingleOrDefault(x => x.Id == id);
        }

        public bool Exist(int invoiceId)
        {
            return context.Invoices.Any(x => x.Id == invoiceId);
        }

        public void UpdateStatus(Invoice invoice)
        {
            context.Entry(invoice).Property("InvoiceStatus").IsModified = true;
        }

        //public Invoice GetExcel(int invoiceId)
        //{
        //    return context.Invoices.Select(x => new Invoice
        //        {
        //            Id = x.Id,
        //            ExcelFile = x.ExcelFile,
        //            ExcelFileName = x.ExcelFileName
        //        })
        //        .SingleOrDefault(x => x.Id == invoiceId);
        //}

        //public Invoice GetPdf(int invoiceId)
        //{
        //    return context.Invoices.Select(x => new Invoice
        //        {
        //            Id = x.Id,
        //            PdfFile = x.PdfFile,
        //            PdfFileName = x.PdfFileName
        //        })
        //        .SingleOrDefault(x => x.Id == invoiceId);
        //}

        public void UpdateExcelId(Invoice invoice)
        {
            context.Entry(invoice).Property("ExcelFileId").IsModified = true;
        }

        public void UpdatePdfId(Invoice invoice)
        {
            context.Entry(invoice).Property("PdfFileId").IsModified = true;
        }

        //public void UpdateExcel(Invoice invoice)
        //{
        //    context.Entry(invoice).Property("ExcelFile").IsModified = true;
        //    context.Entry(invoice).Property("ExcelFileName").IsModified = true;
        //    context.Entry(invoice).Property("ExcelFileCreatedDate").IsModified = true;
        //}

        //public void UpdatePdf(Invoice invoice)
        //{
        //    context.Entry(invoice).Property("PdfFile").IsModified = true;
        //    context.Entry(invoice).Property("PdfFileName").IsModified = true;
        //    context.Entry(invoice).Property("PdfFileCreatedDate").IsModified = true;
        //}

        public void UpdateStatusAndApprove(Invoice invoice)
        {
            context.Entry(invoice).Property("InvoiceNumber").IsModified = true;
            UpdateStatus(invoice);
        }

        public IList<Invoice> GetOptions(string projectId)
        {
            return context.Invoices
                .Include(x => x.Solfac)
                .Include(x => x.PDfFileData)
                .Where(x => x.Solfac == null && x.InvoiceStatus == InvoiceStatus.Approved && x.ProjectId == projectId)
                .ToList();
        }

        public bool InvoiceNumberExist(string invoiceNumber)
        {
            return context.Invoices.Any(x => x.InvoiceNumber == invoiceNumber);
        }

        //private IQueryable<Invoice> GetProperties(IQueryable<Invoice> dbset)
        //{
        //    return dbset.Select(x => new Invoice
        //    {
        //        Id = x.Id,
        //        AccountName = x.AccountName,
        //        Project = x.Project,
        //        Address = x.Address,
        //        City = x.City,
        //        Cuit = x.Cuit,
        //        Country = x.Country,
        //        Province = x.Province,
        //        Analytic = x.Analytic,
        //        Zipcode = x.Zipcode,
        //        CreatedDate = x.CreatedDate,
        //        Service = x.Service,
        //        ProjectId = x.ProjectId,
        //        ExcelFileName = x.ExcelFileName,
        //        PdfFileName = x.PdfFileName,
        //        ExcelFileCreatedDate = x.ExcelFileCreatedDate,
        //        PdfFileCreatedDate = x.PdfFileCreatedDate,
        //        InvoiceStatus = x.InvoiceStatus,
        //        InvoiceNumber = x.InvoiceNumber,
        //        UserId = x.UserId,
        //        User = x.User,
        //        CustomerId = x.CustomerId,
        //        ServiceId = x.ServiceId,
        //        Solfac = x.Solfac
        //    });
        //}

        public ICollection<Invoice> SearchByParams(InvoiceParams parameters)
        {
            IQueryable<Invoice> query = context.Invoices;

            query = ApplyFilters(parameters, query).Include(x => x.User).Include(x => x.ExcelFileData).Include(x => x.PDfFileData);

            return query.ToList();
        }

        private static IQueryable<Invoice> ApplyFilters(InvoiceParams parameters, IQueryable<Invoice> query)
        {
            if(parameters.DateSince.HasValue && parameters.DateTo.HasValue)
                query = query.Where(x => x.CreatedDate.Date >= parameters.DateSince.GetValueOrDefault().Date && x.CreatedDate.Date <= parameters.DateTo.GetValueOrDefault().Date);

            if (!string.IsNullOrWhiteSpace(parameters.CustomerId) && !parameters.CustomerId.Equals("0"))
                query = query.Where(x => x.AccountId == parameters.CustomerId);

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
            IQueryable<Invoice> query = context.Invoices.Include(x => x.User).Include(x => x.ExcelFileData).Include(x => x.PDfFileData);

            query = query.Where(x => x.User.Email == userMail);

            query = ApplyFilters(parameters, query);

            return query.ToList();
        }

        public void AddHistory(InvoiceHistory history)
        {
            context.InvoiceHistories.Add(history);
        }

        public ICollection<InvoiceHistory> GetHistories(int id)
        {
            return context.InvoiceHistories.Where(x => x.InvoiceId == id).Include(x => x.User).ToList().AsReadOnly();
        }

        public void UpdateSolfacId(Invoice invoiceToModif)
        {
            context.Entry(invoiceToModif).Property("SolfacId").IsModified = true;
        }

        public ICollection<Invoice> GetBySolfac(int id)
        {
            return context.Invoices
                .Include(x => x.ExcelFileData)
                .Include(x => x.PDfFileData)
                .Where(x => x.SolfacId == id)
                .ToList();
        }

        public void UpdatePdfFileName(Invoice invoiceToModif)
        {
            context.Entry(invoiceToModif).Property("PdfFileName").IsModified = true;
        }

        public IList<Invoice> GetByIds(IList<int> invoiceIds)
        {
            return context.Invoices.Where(x => invoiceIds.Contains(x.Id)).Include(x => x.ExcelFileData).ToList();
        }
    }
}