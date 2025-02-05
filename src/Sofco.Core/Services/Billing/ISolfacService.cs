﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Sofco.Core.Config;
using Sofco.Core.Models.Billing;
using Sofco.Domain.DTO;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface ISolfacService
    {
        Response<Solfac> CreateSolfac(Solfac solfac, IList<int> invoicesId, IList<int> certificatesId);
        IList<Solfac> Search(SolfacParams parameter);
        IList<Hito> GetHitosByProject(string projectId);
        IList<Solfac> GetByProject(string projectId);
        Response<Solfac> GetById(int id);
        Response ChangeStatus(Solfac solfac, SolfacStatusParams parameters, EmailConfig emailConfig);
        Response ChangeStatus(int solfacId, SolfacStatusParams parameters, EmailConfig emailConfig);
        Response Delete(int id);
        ICollection<SolfacHistory> GetHistories(int id);
        Response Update(Solfac domain, string modelComments);
        Response<SolfacAttachment> SaveFile(int solfacId, byte[] fileAsArrayBytes, string fileFileName);
        ICollection<SolfacAttachment> GetFiles(int solfacId);
        Response<SolfacAttachment> GetFileById(int fileId);
        Response DeleteFile(int id);
        Response UpdateBill(int id, SolfacStatusParams solfacStatusParams);
        Response UpdateCashedDate(int id, SolfacStatusParams parameters);
        Response DeleteInvoice(int id, int invoiceId);
        Response<ICollection<InvoiceFileOptions>> GetInvoices(int id);
        Response<List<Invoice>> AddInvoices(int id, IList<int> invoices);
        Response<Solfac> Validate(Solfac solfac);
        Response DeleteDetail(int id);
        Response<Solfac> Post(Solfac solfac, IList<int> invoicesId, IList<int> certificatesId);
        Response DeleteSolfacCertificate(int id, int certificateId);
        Response<IList<Certificate>> AddCertificates(int id, IList<int> certificates);
        Response AdminUpdate(int id, SolfacAdminUpdate request);
    }
}
