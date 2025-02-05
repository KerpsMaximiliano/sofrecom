﻿using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Domain.DTO;
using Sofco.Domain.Models.Billing;

namespace Sofco.Core.DAL.Billing
{
    public interface IInvoiceRepository : IBaseRepository<Invoice>
    {
        IList<Invoice> GetByProject(string projectId);
        Invoice GetById(int id);
        bool Exist(int invoiceId);
        void UpdateStatus(Invoice invoice);
        //Invoice GetExcel(int invoiceId);
        //Invoice GetPdf(int invoiceId);
        //void UpdateExcel(Invoice invoice);
        //void UpdatePdf(Invoice invoice);
        void UpdateStatusAndApprove(Invoice invoice);
        IList<Invoice> GetOptions(string projectId);
        ICollection<Invoice> SearchByParams(InvoiceParams parameters);
        bool InvoiceNumberExist(string parametersInvoiceNumber);
        ICollection<Invoice> SearchByParamsAndUser(InvoiceParams parameters, string userMail);
        void AddHistory(InvoiceHistory history);
        ICollection<InvoiceHistory> GetHistories(int id);
        void UpdateSolfacId(Invoice invoiceToModif);
        ICollection<Invoice> GetBySolfac(int id);
        void UpdatePdfFileName(Invoice invoiceToModif);
        IList<Invoice> GetByIds(IList<int> invoiceIds);
        void UpdateExcelId(Invoice invoice);
        void UpdatePdfId(Invoice invoice);
        bool HasFile(int invoiceId);
    }
}
