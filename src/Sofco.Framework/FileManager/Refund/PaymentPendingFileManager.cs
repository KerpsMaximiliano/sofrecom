using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using Sofco.Core.FileManager;
using Sofco.Core.Models.AdvancementAndRefund.Common;

namespace Sofco.Framework.FileManager.Refund
{
    public class PaymentPendingFileManager : IPaymentPendingFileManager
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public PaymentPendingFileManager(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public ExcelPackage CreateExcel(IList<PaymentPendingModel> data)
        {
            var memoryStream = this.GetTemplateStream().Result;

            var excel = new ExcelPackage(memoryStream);

            return Create(excel, data);
        }

        private ExcelPackage Create(ExcelPackage excel, IList<PaymentPendingModel> data)
        {
            var sheet = excel.Workbook.Worksheets.First();

            var index = 3;

            foreach (var model in data)
            {
                sheet.Cells[$"A{index}"].Value = model.Bank;
                sheet.Cells[$"B{index}"].Value = model.UserApplicantDesc;
                sheet.Cells[$"C{index}"].Value = model.CurrencyDesc;
                sheet.Cells[$"D{index}"].Value = model.Ammount;
                sheet.Cells[$"E{index}"].Value = model.AmmountPesos;

                index++;
            }

            return excel;
        }

        private Task<MemoryStream> GetTemplateStream()
        {
            var fileRoot = new FileInfo($"{_hostingEnvironment.ContentRootPath}/wwwroot/excelTemplates/pendientes-control.xlsx");

            var template = new ExcelPackage(fileRoot, false);

            var memoryStream = new MemoryStream(template.GetAsByteArray());

            template.Dispose();

            return Task.FromResult(memoryStream);
        }
    }
}
