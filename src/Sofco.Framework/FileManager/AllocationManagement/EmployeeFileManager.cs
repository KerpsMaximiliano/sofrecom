using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using Sofco.Core.FileManager;
using Sofco.Domain.Models.Reports;

namespace Sofco.Framework.FileManager.AllocationManagement
{
    public class EmployeeFileManager : IEmployeeFileManager
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public EmployeeFileManager(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public ExcelPackage CreateReport(IList<EmployeeView> list)
        {
            var memoryStream = this.GetTemplateStream().Result;

            var excel = new ExcelPackage(memoryStream);

            return Create(excel, list);
        }

        private ExcelPackage Create(ExcelPackage excel, IList<EmployeeView> list)
        {
            var sheet = excel.Workbook.Worksheets.First();

            var i = 2;

            foreach (var employeeView in list)
            { 
                sheet.Cells[$"A{i}"].Value = Convert.ToInt32(employeeView.EmployeeNumber);
                sheet.Cells[$"B{i}"].Value = employeeView.Name;
                sheet.Cells[$"C{i}"].Value = employeeView.Profile;
                sheet.Cells[$"D{i}"].Value = employeeView.Seniority;
                sheet.Cells[$"E{i}"].Value = employeeView.Technology;
                sheet.Cells[$"F{i}"].Value = employeeView.StartDate;
                sheet.Cells[$"G{i}"].Value = employeeView.EndDate;
                sheet.Cells[$"H{i}"].Value = employeeView.Email;
                sheet.Cells[$"I{i}"].Value = employeeView.Cuil;
                sheet.Cells[$"J{i}"].Value = employeeView.DocumentNumber;
                sheet.Cells[$"K{i}"].Value = employeeView.Birthday;
                sheet.Cells[$"L{i}"].Value = employeeView.OfficeAddress;
                sheet.Cells[$"M{i}"].Value = employeeView.Address;
                sheet.Cells[$"N{i}"].Value = employeeView.Location;
                sheet.Cells[$"O{i}"].Value = employeeView.Province;
                sheet.Cells[$"P{i}"].Value = employeeView.Country;
                sheet.Cells[$"Q{i}"].Value = employeeView.PhoneCountryCode;
                sheet.Cells[$"R{i}"].Value = employeeView.PhoneAreaCode;
                sheet.Cells[$"S{i}"].Value = employeeView.PhoneNumber;
                sheet.Cells[$"T{i}"].Value = employeeView.HolidaysByLaw;
                sheet.Cells[$"U{i}"].Value = employeeView.HolidaysPendingByLaw;
                sheet.Cells[$"V{i}"].Value = employeeView.HolidaysPending;
                sheet.Cells[$"W{i}"].Value = employeeView.ExtraHolidaysQuantity;
                sheet.Cells[$"X{i}"].Value = employeeView.HasExtraHolidays ? "Si" : "No";
                sheet.Cells[$"Y{i}"].Value = employeeView.ExamDaysTaken;
                sheet.Cells[$"Z{i}"].Value = employeeView.BillingPercentage;
                sheet.Cells[$"AA{i}"].Value = employeeView.BusinessHours;
                sheet.Cells[$"AB{i}"].Value = employeeView.BusinessHoursDescription;
                sheet.Cells[$"AC{i}"].Value = employeeView.PrepaidHealthCode;
                sheet.Cells[$"AD{i}"].Value = employeeView.EndReason;
                sheet.Cells[$"AE{i}"].Value = employeeView.IsExternal ? "Si" : "No";
                sheet.Cells[$"AF{i}"].Value = employeeView.HasCreditCard ? "Si" : "No";
                sheet.Cells[$"AG{i}"].Value = employeeView.Bank;
                sheet.Cells[$"AH{i}"].Value = employeeView.ManagerName;
                sheet.Cells[$"AI{i}"].Value = employeeView.Id;
                sheet.Cells[$"AJ{i}"].Value = employeeView.Percentage;
                sheet.Cells[$"AK{i}"].Value = employeeView.Title;
                sheet.Cells[$"AL{i}"].Value = employeeView.AnalyticName;
                sheet.Cells[$"AM{i}"].Value = employeeView.ServiceName;

                i++;
            }

            return excel;
        }

        private Task<MemoryStream> GetTemplateStream()
        {
            var fileRoot = new FileInfo($"{hostingEnvironment.ContentRootPath}/wwwroot/excelTemplates/reporte-empleados.xlsx");

            var template = new ExcelPackage(fileRoot, false);

            var memoryStream = new MemoryStream(template.GetAsByteArray());

            template.Dispose();

            return Task.FromResult(memoryStream);
        }
    }
}
