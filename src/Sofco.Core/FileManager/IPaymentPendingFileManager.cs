using System.Collections.Generic;
using OfficeOpenXml;
using Sofco.Core.Models.AdvancementAndRefund.Common;
using Sofco.Domain.Utils;

namespace Sofco.Core.FileManager
{
    public interface IPaymentPendingFileManager
    {
        ExcelPackage CreateExcel(IList<PaymentPendingModel> data);
    }
}
