using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Sofco.Core.DAL;
using Sofco.Core.FileManager;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Billing;

namespace Sofco.Framework.FileManager.Billing
{
    public class PurchaseOrderFileManager : IPurchaseOrderFileManager
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IUnitOfWork unitOfWork;

        private IList<Project> Projects { get; set; }

        public PurchaseOrderFileManager(IHostingEnvironment hostingEnvironment, IUnitOfWork unitOfWork)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.unitOfWork = unitOfWork;
        }

        public ExcelPackage CreateReport(IList<PurchaseOrder> purchaseOrders)
        {
            Projects = unitOfWork.ProjectRepository.GetAll();

            var memoryStream = this.GetTemplateStream().Result;

            var excel = new ExcelPackage(memoryStream);

            return Create(excel, purchaseOrders);
        }

        private ExcelPackage Create(ExcelPackage excel, IList<PurchaseOrder> purchaseOrders)
        {
            var sheet = excel.Workbook.Worksheets.First();

            var row = 1;

            foreach (var purchaseOrder in purchaseOrders)
            {
                SetPurchaseOrderHeader(sheet, row);
                row++;

                sheet.SetValue(row, 1, purchaseOrder.Number);
                sheet.SetValue(row, 2, purchaseOrder.ClientExternalName);
                sheet.SetValue(row, 3, purchaseOrder.Title);
                sheet.SetValue(row, 4, GetStatusDescription(purchaseOrder.Status));
                sheet.SetValue(row, 5, purchaseOrder.ReceptionDate.ToString("dd/MM/yyyy"));
                sheet.SetValue(row, 6, purchaseOrder.StartDate.ToString("dd/MM/yyyy"));
                sheet.SetValue(row, 7, purchaseOrder.EndDate.ToString("dd/MM/yyyy"));
                sheet.SetValue(row, 8, 0);
                sheet.SetValue(row, 9, 0);
                sheet.SetValue(row, 10, 0);

                foreach (var ammountDetail in purchaseOrder.AmmountDetails)
                {
                    if (ammountDetail.CurrencyId == 1)
                        sheet.SetValue(row, 8, ammountDetail.Ammount);

                    if (ammountDetail.CurrencyId == 2)
                        sheet.SetValue(row, 9, ammountDetail.Ammount);

                    if (ammountDetail.CurrencyId == 3)
                        sheet.SetValue(row, 10, ammountDetail.Ammount);
                }

                sheet.SetValue(row, 11, purchaseOrder.Margin);
                sheet.SetValue(row, 12, purchaseOrder.Area?.Text);
                sheet.SetValue(row, 13, purchaseOrder.FicheDeSignature);
                sheet.SetValue(row, 14, purchaseOrder.PaymentForm);
                sheet.SetValue(row, 15, purchaseOrder.Description);
                sheet.SetValue(row, 16, purchaseOrder.Comments);

                SetBorderRight(sheet, row);

                var proposals = new string[] { };

                if (purchaseOrder.PurchaseOrderAnalytics.Any())
                {
                    row++;
                    sheet.SetValue(row, 1, purchaseOrder.Number);
                    SetAnalyticHeader(sheet, row);
                    SetBorderRight(sheet, row);
                }
                else
                {
                    row++;
                    continue;
                }

                var proposalAdded = false;

                foreach (var ocAnalytic in purchaseOrder.PurchaseOrderAnalytics)
                {
                    if (!proposalAdded) row++;
             
                    proposalAdded = false;

                    if (ocAnalytic.Analytic != null)
                    {
                        sheet.SetValue(row, 1, purchaseOrder.Number);
                        sheet.SetValue(row, 2, ocAnalytic.Analytic.Name);
                        sheet.SetValue(row, 3, ocAnalytic.Analytic.Service);
                        sheet.SetValue(row, 4, ocAnalytic.Analytic.Manager?.Name);
                        sheet.SetValue(row, 5, ocAnalytic.Analytic.CommercialManager?.Name);

                        if (!string.IsNullOrWhiteSpace(purchaseOrder.Proposal))
                            proposals = purchaseOrder.Proposal.Split(';');

                        var projects = Projects.Where(x => x.ServiceId.Equals(ocAnalytic.Analytic.ServiceId));

                        var proposalInitRow = row;
                        var proposalEndRow = row;

                        SetBorderRight(sheet, row);

                        foreach (var project in projects)
                        {
                            if (proposals.Contains(project.OpportunityNumber))
                            {
                                SetBorderRight(sheet, row);

                                proposalAdded = true;

                                sheet.SetValue(row, 1, purchaseOrder.Number);
                                sheet.SetValue(row, 6, project.OpportunityNumber);
                                sheet.SetValue(row, 7, project.OpportunityName);
                                sheet.SetValue(row, 8, project.Incomes);

                                proposalEndRow = row;

                                row++;
                            }
                        }

                        MergeRows(sheet, proposalInitRow, proposalEndRow);
                    }
                }

                if (!proposalAdded)
                {
                    row++;
                }
                //else
                //{
                //    row++;
                //}

                //SetBorderBottom(sheet, row);
            }

            return excel;
        }

        private void SetBorderRight(ExcelWorksheet sheet, int row)
        {
            sheet.Cells[$"P{row}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"P{row}"].Style.Border.Right.Color.SetColor(Color.Black);
        }

        private void SetBorderBottom(ExcelWorksheet sheet, int row)
        {
            row--;
            sheet.Cells[$"A{row}:P{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[$"A{row}:P{row}"].Style.Fill.BackgroundColor.SetColor(Color.White);
            sheet.Cells[$"A{row}:P{row}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"A{row}:P{row}"].Style.Border.Top.Color.SetColor(Color.Black);
        }

        private void MergeRows(ExcelWorksheet sheet, int proposalInitRow, int proposalEndRow)
        {
            sheet.Cells[$"B{proposalInitRow}:B{proposalEndRow}"].Merge = true;
            sheet.Cells[$"C{proposalInitRow}:C{proposalEndRow}"].Merge = true;
            sheet.Cells[$"D{proposalInitRow}:D{proposalEndRow}"].Merge = true;
            sheet.Cells[$"E{proposalInitRow}:E{proposalEndRow}"].Merge = true;
        }

        private void SetAnalyticHeader(ExcelWorksheet sheet, int row)
        {
            sheet.Cells[$"B{row}:H{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[$"B{row}:H{row}"].Style.Fill.BackgroundColor.SetColor(Color.CadetBlue);

            sheet.Cells[$"B{row}"].Value = "Analitica";
            sheet.Cells[$"C{row}"].Value = "Servicio";
            sheet.Cells[$"D{row}"].Value = "Gerente Proyecto";
            sheet.Cells[$"E{row}"].Value = "Ejecutivo Cuenta";
            sheet.Cells[$"F{row}"].Value = "N° Propuesta";
            sheet.Cells[$"G{row}"].Value = "Propuesta";
            sheet.Cells[$"H{row}"].Value = "Ingresos Estimados";
        }

        private void SetPurchaseOrderHeader(ExcelWorksheet sheet, int row)
        {
            sheet.Cells[$"A{row}:P{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[$"A{row}:P{row}"].Style.Fill.BackgroundColor.SetColor(Color.CadetBlue);

            sheet.Cells[$"A{row}"].Value = "Numero";
            sheet.Cells[$"B{row}"].Value = "Cliente";
            sheet.Cells[$"C{row}"].Value = "Titulo";
            sheet.Cells[$"D{row}"].Value = "Estado";
            sheet.Cells[$"E{row}"].Value = "Fecha Recepcion";
            sheet.Cells[$"F{row}"].Value = "Fecha desde";
            sheet.Cells[$"G{row}"].Value = "Fecha hasta";
            sheet.Cells[$"H{row}"].Value = "Monto $";
            sheet.Cells[$"I{row}"].Value = "Monto USD";
            sheet.Cells[$"J{row}"].Value = "Monto Euro";
            sheet.Cells[$"K{row}"].Value = "Margen";
            sheet.Cells[$"L{row}"].Value = "Area";
            sheet.Cells[$"M{row}"].Value = "Fiche Signature";
            sheet.Cells[$"N{row}"].Value = "Forma de pago";
            sheet.Cells[$"O{row}"].Value = "Descripcion";
            sheet.Cells[$"P{row}"].Value = "Comentarios";

            sheet.Cells[$"P{row}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"P{row}"].Style.Border.Right.Color.SetColor(Color.Black);
        }

        private Task<MemoryStream> GetTemplateStream()
        {
            var fileRoot = new FileInfo($"{hostingEnvironment.ContentRootPath}/wwwroot/excelTemplates/purchase-order-report.xlsx");

            var template = new ExcelPackage(fileRoot, false);

            var memoryStream = new MemoryStream(template.GetAsByteArray());

            template.Dispose();

            return Task.FromResult(memoryStream);
        }

        private string GetStatusDescription(PurchaseOrderStatus status)
        {
            switch (status)
            {
                case PurchaseOrderStatus.Closed: return "Cerrada";
                case PurchaseOrderStatus.ComercialPending: return "Pend. Aprobación Comercial";
                case PurchaseOrderStatus.CompliancePending: return "Pend. Aprobación Compliance";
                case PurchaseOrderStatus.Consumed: return "Consumida";
                case PurchaseOrderStatus.DafPending: return "Pend. Aprobación DAF";
                case PurchaseOrderStatus.Draft: return "Borrador";
                case PurchaseOrderStatus.OperativePending: return "Pend. Aprobación Operativa";
                case PurchaseOrderStatus.Reject: return "Rechazada";
                case PurchaseOrderStatus.Valid: return "Vigente";
                default: return string.Empty;
            }
        }
    }
}
