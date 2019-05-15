using System;
using System.Collections.Generic;
using Sofco.Domain.Crm;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;

namespace Sofco.Service.Crm.Interfaces
{
    public interface ICrmInvoicingMilestoneService
    {
        List<CrmHito> GetToExpire(int daysToExpire);

        List<CrmProjectHito> GetByProjectId(Guid projectId);

        void UpdateAmmountAndStatus(HitoParameters data, Response response);

        void UpdateStatus(List<string> hitoIds, HitoStatus hitoStatus);

        void UpdateStatusAndPurchaseOrder(List<string> hitoIds, HitoStatus hitoStatus, string purchaseOrderNumber);

        void UpdateStatusAndInvoiceDateAndNumber(List<string> hitoIds, HitoStatus hitoStatus, DateTime invoicingDate, string invoiceCode);

        void UpdateStatusAndBillingDate(List<string> hitoIds, HitoStatus hitoStatus, DateTime billingDate);

        string Create(HitoParameters data, Response response);

        void Close(Response response, string id, string status);

        void UpdateAmmount(HitoAmmountParameter hito, Response response);

        void Delete(string hitoId, Response response);

        void UpdateAmmountAndName(HitoAmmountParameter hito, Response response);

        CrmProjectHito GetById(string id);
    }
}