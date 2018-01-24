using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Sofco.DAL;
using Sofco.Model.Enums;
using Sofco.Model.Enums.TimeManagement;

namespace Sofco.WebApi.Migrations
{
    [DbContext(typeof(SofcoContext))]
    [Migration("20180123151710_ChangeMaxLengthSolfacProperties")]
    partial class ChangeMaxLengthSolfacProperties
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasDefaultSchema("app")
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Sofco.Model.Models.Admin.Functionality", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(5);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("ModuleId");

                    b.HasKey("Id");

                    b.HasIndex("ModuleId");

                    b.ToTable("Functionalities");
                });

            modelBuilder.Entity("Sofco.Model.Models.Admin.GlobalSetting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("Created");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<DateTime?>("Modified");

                    b.Property<int>("Type");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.HasKey("Id");

                    b.ToTable("GlobalSettings");
                });

            modelBuilder.Entity("Sofco.Model.Models.Admin.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<string>("Code")
                        .HasMaxLength(100);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<DateTime?>("EndDate");

                    b.Property<int?>("RoleId");

                    b.Property<DateTime>("StartDate");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("Sofco.Model.Models.Admin.Module", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(5);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Modules");
                });

            modelBuilder.Entity("Sofco.Model.Models.Admin.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<DateTime?>("EndDate");

                    b.Property<DateTime>("StartDate");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Sofco.Model.Models.Admin.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(150);

                    b.Property<DateTime?>("EndDate");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150);

                    b.Property<DateTime>("StartDate");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(150);

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Sofco.Model.Models.AllocationManagement.Allocation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AnalyticId");

                    b.Property<int>("EmployeeId");

                    b.Property<decimal>("Percentage");

                    b.Property<DateTime>("ReleaseDate");

                    b.Property<DateTime>("StartDate");

                    b.HasKey("Id");

                    b.HasIndex("AnalyticId");

                    b.HasIndex("EmployeeId");

                    b.ToTable("Allocations");
                });

            modelBuilder.Entity("Sofco.Model.Models.AllocationManagement.Analytic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ActivityId");

                    b.Property<string>("AmountEarned")
                        .HasMaxLength(500);

                    b.Property<string>("AmountProject")
                        .HasMaxLength(500);

                    b.Property<bool>("BugsAccess");

                    b.Property<string>("ClientExternalId")
                        .HasMaxLength(150);

                    b.Property<string>("ClientExternalName")
                        .HasMaxLength(150);

                    b.Property<int?>("ClientGroupId");

                    b.Property<string>("ClientProjectTfs")
                        .HasMaxLength(150);

                    b.Property<int?>("CommercialManagerId");

                    b.Property<string>("ContractNumber")
                        .HasMaxLength(50);

                    b.Property<int>("CostCenterId");

                    b.Property<DateTime>("CreationDate");

                    b.Property<int?>("CurrencyId");

                    b.Property<string>("Description")
                        .HasMaxLength(500);

                    b.Property<int>("DirectorId");

                    b.Property<DateTime>("EndDateContract");

                    b.Property<bool?>("EvalProp");

                    b.Property<int?>("ManagerId");

                    b.Property<string>("Name")
                        .HasMaxLength(200);

                    b.Property<int?>("ProductId");

                    b.Property<string>("Proposal")
                        .HasMaxLength(200);

                    b.Property<int?>("PurchaseOrderId");

                    b.Property<string>("Service")
                        .HasMaxLength(50);

                    b.Property<int?>("ServiceTypeId");

                    b.Property<int?>("SoftwareLawId");

                    b.Property<int?>("SolutionId");

                    b.Property<DateTime>("StartDateContract");

                    b.Property<int>("Status");

                    b.Property<int?>("TechnologyId");

                    b.Property<string>("Title")
                        .HasMaxLength(150);

                    b.Property<int>("TitleId");

                    b.Property<string>("UsersQv")
                        .HasMaxLength(500);

                    b.HasKey("Id");

                    b.HasIndex("ActivityId");

                    b.HasIndex("ClientGroupId");

                    b.HasIndex("CommercialManagerId");

                    b.HasIndex("CostCenterId");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("DirectorId");

                    b.HasIndex("ManagerId");

                    b.HasIndex("ProductId");

                    b.HasIndex("PurchaseOrderId");

                    b.HasIndex("ServiceTypeId");

                    b.HasIndex("SoftwareLawId");

                    b.HasIndex("SolutionId");

                    b.HasIndex("TechnologyId");

                    b.ToTable("Analytics");
                });

            modelBuilder.Entity("Sofco.Model.Models.AllocationManagement.CostCenter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<int>("Code")
                        .HasMaxLength(3);

                    b.Property<string>("Description")
                        .HasMaxLength(250);

                    b.Property<string>("Letter")
                        .HasMaxLength(1);

                    b.HasKey("Id");

                    b.ToTable("CostCenters");
                });

            modelBuilder.Entity("Sofco.Model.Models.AllocationManagement.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("BillingPercentage");

                    b.Property<DateTime?>("Birthday");

                    b.Property<DateTime?>("Created");

                    b.Property<string>("CreatedByUser")
                        .HasMaxLength(50);

                    b.Property<string>("EmployeeNumber")
                        .HasMaxLength(50);

                    b.Property<DateTime?>("EndDate");

                    b.Property<DateTime?>("Modified");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<string>("Profile")
                        .HasMaxLength(100);

                    b.Property<string>("Seniority")
                        .HasMaxLength(100);

                    b.Property<DateTime>("StartDate");

                    b.Property<string>("Technology")
                        .HasMaxLength(300);

                    b.HasKey("Id");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("Sofco.Model.Models.AllocationManagement.EmployeeLicense", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("Created");

                    b.Property<string>("EmployeeNumber")
                        .HasMaxLength(50);

                    b.Property<DateTime>("EndDate");

                    b.Property<int>("LicenseTypeNumber");

                    b.Property<DateTime?>("Modified");

                    b.Property<DateTime>("StartDate");

                    b.HasKey("Id");

                    b.ToTable("EmployeeLicenses");
                });

            modelBuilder.Entity("Sofco.Model.Models.AllocationManagement.EmployeeSyncAction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("Created");

                    b.Property<string>("EmployeeData");

                    b.Property<string>("EmployeeNumber")
                        .HasMaxLength(50);

                    b.Property<DateTime?>("EndDate");

                    b.Property<DateTime?>("Modified");

                    b.Property<DateTime?>("StartDate");

                    b.Property<string>("Status")
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.ToTable("EmployeeSyncActions");
                });

            modelBuilder.Entity("Sofco.Model.Models.AllocationManagement.LicenseType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("Created");

                    b.Property<string>("Description")
                        .HasMaxLength(300);

                    b.Property<int>("LicenseTypeNumber");

                    b.Property<DateTime?>("Modified");

                    b.HasKey("Id");

                    b.ToTable("LicenseTypes");
                });

            modelBuilder.Entity("Sofco.Model.Models.Billing.Hito", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("Created")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Currency");

                    b.Property<string>("CurrencyId");

                    b.Property<string>("Description");

                    b.Property<string>("ExternalHitoId");

                    b.Property<string>("ExternalProjectId");

                    b.Property<string>("ManagerId");

                    b.Property<DateTime?>("Modified");

                    b.Property<short>("Month");

                    b.Property<string>("OpportunityId");

                    b.Property<int>("SolfacId");

                    b.Property<decimal>("Total");

                    b.HasKey("Id");

                    b.HasIndex("SolfacId");

                    b.ToTable("Hitos");
                });

            modelBuilder.Entity("Sofco.Model.Models.Billing.HitoDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("Created")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Description")
                        .HasMaxLength(3000);

                    b.Property<int>("HitoId");

                    b.Property<DateTime?>("Modified");

                    b.Property<decimal>("Quantity");

                    b.Property<decimal>("Total");

                    b.Property<decimal>("UnitPrice");

                    b.HasKey("Id");

                    b.HasIndex("HitoId");

                    b.ToTable("HitoDetails");
                });

            modelBuilder.Entity("Sofco.Model.Models.Billing.Invoice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AccountName")
                        .HasMaxLength(100);

                    b.Property<string>("Address")
                        .HasMaxLength(100);

                    b.Property<string>("Analytic")
                        .HasMaxLength(100);

                    b.Property<string>("City")
                        .HasMaxLength(50);

                    b.Property<string>("Country")
                        .HasMaxLength(100);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Cuit")
                        .HasMaxLength(100);

                    b.Property<string>("CustomerId");

                    b.Property<byte[]>("ExcelFile");

                    b.Property<DateTime>("ExcelFileCreatedDate");

                    b.Property<string>("ExcelFileName")
                        .HasMaxLength(150);

                    b.Property<string>("InvoiceNumber")
                        .HasMaxLength(50);

                    b.Property<int>("InvoiceStatus");

                    b.Property<byte[]>("PdfFile");

                    b.Property<DateTime>("PdfFileCreatedDate");

                    b.Property<string>("PdfFileName")
                        .HasMaxLength(150);

                    b.Property<string>("Project")
                        .HasMaxLength(100);

                    b.Property<string>("ProjectId");

                    b.Property<string>("Province")
                        .HasMaxLength(100);

                    b.Property<string>("Service")
                        .HasMaxLength(100);

                    b.Property<string>("ServiceId");

                    b.Property<int?>("SolfacId");

                    b.Property<int>("UserId");

                    b.Property<string>("Zipcode")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("SolfacId");

                    b.HasIndex("UserId");

                    b.ToTable("Invoices");
                });

            modelBuilder.Entity("Sofco.Model.Models.Billing.InvoiceHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Comment");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<int>("InvoiceId");

                    b.Property<int>("StatusFrom");

                    b.Property<int>("StatusTo");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("InvoiceId");

                    b.HasIndex("UserId");

                    b.ToTable("InvoiceHistories");
                });

            modelBuilder.Entity("Sofco.Model.Models.Billing.Solfac", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Analytic");

                    b.Property<decimal>("BuenosAiresPercentage");

                    b.Property<string>("BusinessName")
                        .HasMaxLength(100);

                    b.Property<decimal>("CapitalPercentage");

                    b.Property<DateTime?>("CashedDate");

                    b.Property<string>("CelPhone")
                        .HasMaxLength(50);

                    b.Property<string>("ClientName")
                        .HasMaxLength(100);

                    b.Property<string>("ContractNumber")
                        .HasMaxLength(1000);

                    b.Property<int>("CurrencyId");

                    b.Property<string>("CustomerId");

                    b.Property<int>("DocumentTypeId");

                    b.Property<string>("ImputationNumber1")
                        .HasMaxLength(50);

                    b.Property<int>("ImputationNumber3Id");

                    b.Property<string>("InvoiceCode")
                        .HasMaxLength(50);

                    b.Property<DateTime?>("InvoiceDate");

                    b.Property<bool>("InvoiceRequired");

                    b.Property<int>("ModifiedByUserId");

                    b.Property<decimal>("OtherProvince1Percentage");

                    b.Property<decimal>("OtherProvince2Percentage");

                    b.Property<decimal>("OtherProvince3Percentage");

                    b.Property<string>("ParticularSteps")
                        .HasMaxLength(1000);

                    b.Property<int>("PaymentTermId");

                    b.Property<string>("Project")
                        .HasMaxLength(1000);

                    b.Property<string>("ProjectId");

                    b.Property<int>("Province1Id");

                    b.Property<int>("Province2Id");

                    b.Property<int>("Province3Id");

                    b.Property<string>("Service");

                    b.Property<string>("ServiceId");

                    b.Property<DateTime>("StartDate");

                    b.Property<int>("Status");

                    b.Property<decimal>("TotalAmount");

                    b.Property<DateTime>("UpdatedDate");

                    b.Property<int>("UserApplicantId");

                    b.Property<bool>("WithTax");

                    b.HasKey("Id");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("DocumentTypeId");

                    b.HasIndex("ImputationNumber3Id");

                    b.HasIndex("PaymentTermId");

                    b.HasIndex("UserApplicantId");

                    b.ToTable("Solfacs");
                });

            modelBuilder.Entity("Sofco.Model.Models.Billing.SolfacAttachment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationDate");

                    b.Property<byte[]>("File");

                    b.Property<string>("Name")
                        .HasMaxLength(500);

                    b.Property<int>("SolfacId");

                    b.HasKey("Id");

                    b.HasIndex("SolfacId");

                    b.ToTable("SolfacAttachments");
                });

            modelBuilder.Entity("Sofco.Model.Models.Billing.SolfacHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Comment");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<int>("SolfacId");

                    b.Property<int>("SolfacStatusFrom");

                    b.Property<int>("SolfacStatusTo");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("SolfacId");

                    b.HasIndex("UserId");

                    b.ToTable("SolfacHistories");
                });

            modelBuilder.Entity("Sofco.Model.Relationships.RoleFunctionality", b =>
                {
                    b.Property<int>("RoleId");

                    b.Property<int>("FunctionalityId");

                    b.HasKey("RoleId", "FunctionalityId");

                    b.HasIndex("FunctionalityId");

                    b.ToTable("RoleFunctionality");
                });

            modelBuilder.Entity("Sofco.Model.Relationships.UserGroup", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("GroupId");

                    b.HasKey("UserId", "GroupId");

                    b.HasIndex("GroupId");

                    b.ToTable("UserGroup");
                });

            modelBuilder.Entity("Sofco.Model.Utils.ClientGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Text")
                        .HasMaxLength(60);

                    b.HasKey("Id");

                    b.ToTable("ClientGroups");
                });

            modelBuilder.Entity("Sofco.Model.Utils.Currency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Text")
                        .HasMaxLength(15);

                    b.HasKey("Id");

                    b.ToTable("Currencies");
                });

            modelBuilder.Entity("Sofco.Model.Utils.DocumentType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Text")
                        .HasMaxLength(60);

                    b.HasKey("Id");

                    b.ToTable("DocumentTypes");
                });

            modelBuilder.Entity("Sofco.Model.Utils.ImputationNumber", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Text")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.ToTable("ImputationNumbers");
                });

            modelBuilder.Entity("Sofco.Model.Utils.PaymentTerm", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Text")
                        .HasMaxLength(60);

                    b.HasKey("Id");

                    b.ToTable("PaymentTerms");
                });

            modelBuilder.Entity("Sofco.Model.Utils.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Text")
                        .HasMaxLength(60);

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Sofco.Model.Utils.Province", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Text")
                        .HasMaxLength(60);

                    b.HasKey("Id");

                    b.ToTable("Provinces");
                });

            modelBuilder.Entity("Sofco.Model.Utils.PurchaseOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Text")
                        .HasMaxLength(60);

                    b.HasKey("Id");

                    b.ToTable("PurchaseOrders");
                });

            modelBuilder.Entity("Sofco.Model.Utils.ServiceType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Text")
                        .HasMaxLength(60);

                    b.HasKey("Id");

                    b.ToTable("ServiceTypes");
                });

            modelBuilder.Entity("Sofco.Model.Utils.SoftwareLaw", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Text")
                        .HasMaxLength(60);

                    b.HasKey("Id");

                    b.ToTable("SoftwareLaws");
                });

            modelBuilder.Entity("Sofco.Model.Utils.Solution", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Text")
                        .HasMaxLength(60);

                    b.HasKey("Id");

                    b.ToTable("Solutions");
                });

            modelBuilder.Entity("Sofco.Model.Utils.Technology", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Text")
                        .HasMaxLength(60);

                    b.HasKey("Id");

                    b.ToTable("Technologies");
                });

            modelBuilder.Entity("Sofco.Model.Models.Admin.Functionality", b =>
                {
                    b.HasOne("Sofco.Model.Models.Admin.Module", "Module")
                        .WithMany("Functionalities")
                        .HasForeignKey("ModuleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sofco.Model.Models.Admin.Group", b =>
                {
                    b.HasOne("Sofco.Model.Models.Admin.Role", "Role")
                        .WithMany("Groups")
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Sofco.Model.Models.AllocationManagement.Allocation", b =>
                {
                    b.HasOne("Sofco.Model.Models.AllocationManagement.Analytic", "Analytic")
                        .WithMany("Allocations")
                        .HasForeignKey("AnalyticId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sofco.Model.Models.AllocationManagement.Employee", "Employee")
                        .WithMany("Allocations")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sofco.Model.Models.AllocationManagement.Analytic", b =>
                {
                    b.HasOne("Sofco.Model.Utils.ImputationNumber", "Activity")
                        .WithMany("Analytics")
                        .HasForeignKey("ActivityId");

                    b.HasOne("Sofco.Model.Utils.ClientGroup", "ClientGroup")
                        .WithMany("Analytics")
                        .HasForeignKey("ClientGroupId");

                    b.HasOne("Sofco.Model.Models.Admin.User", "CommercialManager")
                        .WithMany("Analytics3")
                        .HasForeignKey("CommercialManagerId");

                    b.HasOne("Sofco.Model.Models.AllocationManagement.CostCenter", "CostCenter")
                        .WithMany("Analytics")
                        .HasForeignKey("CostCenterId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sofco.Model.Utils.Currency", "Currency")
                        .WithMany("Analytics")
                        .HasForeignKey("CurrencyId");

                    b.HasOne("Sofco.Model.Models.Admin.User", "Director")
                        .WithMany("Analytics1")
                        .HasForeignKey("DirectorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sofco.Model.Models.Admin.User", "Manager")
                        .WithMany("Analytics2")
                        .HasForeignKey("ManagerId");

                    b.HasOne("Sofco.Model.Utils.Product", "Product")
                        .WithMany("Analytics")
                        .HasForeignKey("ProductId");

                    b.HasOne("Sofco.Model.Utils.PurchaseOrder", "PurchaseOrder")
                        .WithMany("Analytics")
                        .HasForeignKey("PurchaseOrderId");

                    b.HasOne("Sofco.Model.Utils.ServiceType", "ServiceType")
                        .WithMany("Analytics")
                        .HasForeignKey("ServiceTypeId");

                    b.HasOne("Sofco.Model.Utils.SoftwareLaw", "SoftwareLaw")
                        .WithMany("Analytics")
                        .HasForeignKey("SoftwareLawId");

                    b.HasOne("Sofco.Model.Utils.Solution", "Solution")
                        .WithMany("Analytics")
                        .HasForeignKey("SolutionId");

                    b.HasOne("Sofco.Model.Utils.Technology", "Technology")
                        .WithMany("Analytics")
                        .HasForeignKey("TechnologyId");
                });

            modelBuilder.Entity("Sofco.Model.Models.Billing.Hito", b =>
                {
                    b.HasOne("Sofco.Model.Models.Billing.Solfac", "Solfac")
                        .WithMany("Hitos")
                        .HasForeignKey("SolfacId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sofco.Model.Models.Billing.HitoDetail", b =>
                {
                    b.HasOne("Sofco.Model.Models.Billing.Hito", "Hito")
                        .WithMany("Details")
                        .HasForeignKey("HitoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sofco.Model.Models.Billing.Invoice", b =>
                {
                    b.HasOne("Sofco.Model.Models.Billing.Solfac", "Solfac")
                        .WithMany("Invoices")
                        .HasForeignKey("SolfacId");

                    b.HasOne("Sofco.Model.Models.Admin.User", "User")
                        .WithMany("Invoices")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sofco.Model.Models.Billing.InvoiceHistory", b =>
                {
                    b.HasOne("Sofco.Model.Models.Billing.Invoice", "Invoice")
                        .WithMany("Histories")
                        .HasForeignKey("InvoiceId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sofco.Model.Models.Admin.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sofco.Model.Models.Billing.Solfac", b =>
                {
                    b.HasOne("Sofco.Model.Utils.Currency", "Currency")
                        .WithMany("Solfacs")
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sofco.Model.Utils.DocumentType", "DocumentType")
                        .WithMany("Solfacs")
                        .HasForeignKey("DocumentTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sofco.Model.Utils.ImputationNumber", "ImputationNumber")
                        .WithMany("Solfacs")
                        .HasForeignKey("ImputationNumber3Id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sofco.Model.Utils.PaymentTerm", "PaymentTerm")
                        .WithMany("Solfacs")
                        .HasForeignKey("PaymentTermId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sofco.Model.Models.Admin.User", "UserApplicant")
                        .WithMany("Solfacs")
                        .HasForeignKey("UserApplicantId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sofco.Model.Models.Billing.SolfacAttachment", b =>
                {
                    b.HasOne("Sofco.Model.Models.Billing.Solfac", "Solfac")
                        .WithMany("Attachments")
                        .HasForeignKey("SolfacId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sofco.Model.Models.Billing.SolfacHistory", b =>
                {
                    b.HasOne("Sofco.Model.Models.Billing.Solfac", "Solfac")
                        .WithMany("Histories")
                        .HasForeignKey("SolfacId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sofco.Model.Models.Admin.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sofco.Model.Relationships.RoleFunctionality", b =>
                {
                    b.HasOne("Sofco.Model.Models.Admin.Functionality", "Functionality")
                        .WithMany("RoleFunctionality")
                        .HasForeignKey("FunctionalityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sofco.Model.Models.Admin.Role", "Role")
                        .WithMany("RoleFunctionality")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sofco.Model.Relationships.UserGroup", b =>
                {
                    b.HasOne("Sofco.Model.Models.Admin.Group", "Group")
                        .WithMany("UserGroups")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sofco.Model.Models.Admin.User", "User")
                        .WithMany("UserGroups")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
