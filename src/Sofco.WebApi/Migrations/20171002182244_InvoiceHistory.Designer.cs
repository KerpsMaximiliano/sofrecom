using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Sofco.DAL;
using Sofco.Model.Enums;

namespace Sofco.WebApi.Migrations
{
    [DbContext(typeof(SofcoContext))]
    [Migration("20171002182244_InvoiceHistory")]
    partial class InvoiceHistory
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
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

            modelBuilder.Entity("Sofco.Model.Models.Admin.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

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

            modelBuilder.Entity("Sofco.Model.Models.Admin.Menu", b =>
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

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("Menus");
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

                    b.Property<int?>("MenuId");

                    b.HasKey("Id");

                    b.HasIndex("MenuId");

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
                        .HasMaxLength(50);

                    b.Property<DateTime?>("EndDate");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<DateTime>("StartDate");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Sofco.Model.Models.Billing.Hito", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Currency");

                    b.Property<string>("Description");

                    b.Property<string>("ExternalHitoId");

                    b.Property<string>("ExternalProjectId");

                    b.Property<short>("Month");

                    b.Property<short>("Quantity");

                    b.Property<int>("SolfacId");

                    b.Property<decimal>("Total");

                    b.Property<decimal>("UnitPrice");

                    b.HasKey("Id");

                    b.HasIndex("SolfacId");

                    b.ToTable("Hitos");
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

                    b.Property<int>("UserId");

                    b.Property<string>("Zipcode")
                        .HasMaxLength(50);

                    b.HasKey("Id");

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

                    b.Property<decimal>("Amount");

                    b.Property<string>("Analytic");

                    b.Property<decimal>("BuenosAiresPercentage");

                    b.Property<string>("BusinessName")
                        .HasMaxLength(100);

                    b.Property<decimal>("CapitalPercentage");

                    b.Property<DateTime?>("CashedDate");

                    b.Property<string>("CelPhone")
                        .HasMaxLength(15);

                    b.Property<string>("ClientName")
                        .HasMaxLength(100);

                    b.Property<string>("ContractNumber")
                        .HasMaxLength(50);

                    b.Property<int>("CurrencyId");

                    b.Property<string>("CustomerId");

                    b.Property<int>("DocumentTypeId");

                    b.Property<string>("ImputationNumber1")
                        .HasMaxLength(50);

                    b.Property<int>("ImputationNumber3Id");

                    b.Property<string>("InvoiceCode")
                        .HasMaxLength(50);

                    b.Property<DateTime?>("InvoiceDate");

                    b.Property<int?>("InvoiceId");

                    b.Property<decimal>("Iva21");

                    b.Property<int>("ModifiedByUserId");

                    b.Property<decimal>("OtherProvince1Percentage");

                    b.Property<decimal>("OtherProvince2Percentage");

                    b.Property<decimal>("OtherProvince3Percentage");

                    b.Property<string>("ParticularSteps")
                        .HasMaxLength(500);

                    b.Property<string>("Project")
                        .HasMaxLength(100);

                    b.Property<string>("ProjectId");

                    b.Property<int>("Province1Id");

                    b.Property<int>("Province2Id");

                    b.Property<int>("Province3Id");

                    b.Property<string>("Service");

                    b.Property<string>("ServiceId");

                    b.Property<DateTime>("StartDate");

                    b.Property<int>("Status");

                    b.Property<short>("TimeLimit");

                    b.Property<decimal>("TotalAmount");

                    b.Property<DateTime>("UpdatedDate");

                    b.Property<int>("UserApplicantId");

                    b.HasKey("Id");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("DocumentTypeId");

                    b.HasIndex("ImputationNumber3Id");

                    b.HasIndex("InvoiceId")
                        .IsUnique();

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
                        .HasMaxLength(200);

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

            modelBuilder.Entity("Sofco.Model.Models.Customer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<string>("Description")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Customers");
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
                        .HasMaxLength(50);

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

            modelBuilder.Entity("Sofco.Model.Utils.Province", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Text")
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.ToTable("Provinces");
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

            modelBuilder.Entity("Sofco.Model.Models.Admin.Module", b =>
                {
                    b.HasOne("Sofco.Model.Models.Admin.Menu", "Menu")
                        .WithMany("Modules")
                        .HasForeignKey("MenuId");
                });

            modelBuilder.Entity("Sofco.Model.Models.Billing.Hito", b =>
                {
                    b.HasOne("Sofco.Model.Models.Billing.Solfac", "Solfac")
                        .WithMany("Hitos")
                        .HasForeignKey("SolfacId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sofco.Model.Models.Billing.Invoice", b =>
                {
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

                    b.HasOne("Sofco.Model.Models.Billing.Invoice", "Invoice")
                        .WithOne("Solfac")
                        .HasForeignKey("Sofco.Model.Models.Billing.Solfac", "InvoiceId");

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
