using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using Sofco.Core.FileManager;
using Sofco.Domain.Models.Billing;

namespace Sofco.Framework.FileManager.Billing
{
    public class InvoiceFileManager : IInvoiceFileManager
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public InvoiceFileManager(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public ExcelPackage CreateInvoiceExcel(Invoice invoice)
        {
            var memoryStream = this.GetTemplateStream().Result;

            var excel = new ExcelPackage(memoryStream);

            return Create(excel, invoice);
        }

        private ExcelPackage Create(ExcelPackage excel, Invoice invoice)
        {
            var sheet = excel.Workbook.Worksheets.First();

            sheet.Cells["D10"].Value = invoice.AccountName;
            sheet.Cells["D11"].Value = invoice.Address;
            sheet.Cells["D12"].Value = invoice.City;
            sheet.Cells["D13"].Value = invoice.Cuit;

            return excel;
        }

        private Task<MemoryStream> GetTemplateStream()
        {
            var fileRoot = new FileInfo($"{_hostingEnvironment.ContentRootPath}/wwwroot/excelTemplates/remito.xlsx");

            var template = new ExcelPackage(fileRoot, false);

            var memoryStream = new MemoryStream(template.GetAsByteArray());

            template.Dispose();

            return Task.FromResult(memoryStream);
        }
    }
}
