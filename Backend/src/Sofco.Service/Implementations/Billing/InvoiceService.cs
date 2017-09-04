using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.Billing;
using Sofco.Core.Services.Billing;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Billing
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceService(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public IList<Invoice> GetByProject(string projectId)
        {
            return _invoiceRepository.GetByProject(projectId);
        }

        public Response<Invoice> GetById(int id)
        {
            var response = new Response<Invoice>();

            var invoce = _invoiceRepository.GetById(id);

            if (invoce == null)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Invoice.NotFound, MessageType.Error));
                return response;
            }

            response.Data = invoce;

            return response;
        }

        public Response<Invoice> Add(Invoice invoice)
        {
            var response = Validate(invoice);

            if (response.HasErrors()) return response;

            try
            {
                invoice.CreatedDate = DateTime.Now;

                _invoiceRepository.Insert(invoice);
                _invoiceRepository.Save(string.Empty);

                response.Messages.Add(new Message(Resources.es.Billing.Invoice.InvoiceCreated, MessageType.Success));
            }
            catch (Exception e)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        private Response<Invoice> Validate(Invoice invoice)
        {
            var response = new Response<Invoice>();

            if (!invoice.Details.Any())
            {
                response.Messages.Add(new Message(Resources.es.Billing.Invoice.DetailsRequired, MessageType.Error));
            }
            else
            {
                if (invoice.Details.Any(x => x.Description == string.Empty))
                {
                    response.Messages.Add(new Message(Resources.es.Billing.Invoice.DescriptionRequired, MessageType.Error));
                }

                if (invoice.Details.Any(x => x.Quantity == 0))
                {
                    response.Messages.Add(new Message(Resources.es.Billing.Invoice.QuantityRequired, MessageType.Error));
                }
            }

            return response;
        }
    }
}
