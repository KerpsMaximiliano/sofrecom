﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "app");

            migrationBuilder.CreateTable(
                name: "GlobalSettings",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(nullable: true),
                    Key = table.Column<string>(maxLength: 100, nullable: false),
                    Modified = table.Column<DateTime>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Value = table.Column<string>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    Code = table.Column<string>(maxLength: 5, nullable: false),
                    Description = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 50, nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    Email = table.Column<string>(maxLength: 50, nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    UserName = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BillingPercentage = table.Column<decimal>(nullable: false),
                    Birthday = table.Column<DateTime>(nullable: true),
                    Created = table.Column<DateTime>(nullable: true),
                    EmployeeNumber = table.Column<string>(maxLength: 50, nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Profile = table.Column<string>(maxLength: 100, nullable: true),
                    Seniority = table.Column<string>(maxLength: 100, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    Technology = table.Column<string>(maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeLicenses",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(nullable: true),
                    EmployeeNumber = table.Column<string>(maxLength: 50, nullable: true),
                    EndDate = table.Column<DateTime>(nullable: false),
                    LicenseTypeNumber = table.Column<int>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeLicenses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LicenseTypes",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(maxLength: 300, nullable: true),
                    LicenseTypeNumber = table.Column<int>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientGroups",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImputationNumbers",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImputationNumbers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTerms",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTerms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Provinces",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Solutions",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solutions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Technologies",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Technologies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Functionalities",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    Code = table.Column<string>(maxLength: 5, nullable: false),
                    Description = table.Column<string>(maxLength: 50, nullable: false),
                    ModuleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Functionalities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Functionalities_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalSchema: "app",
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    Code = table.Column<string>(maxLength: 100, nullable: true),
                    Description = table.Column<string>(maxLength: 50, nullable: false),
                    Email = table.Column<string>(maxLength: 100, nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    RoleId = table.Column<int>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groups_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "app",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Solfacs",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Analytic = table.Column<string>(nullable: true),
                    BuenosAiresPercentage = table.Column<decimal>(nullable: false),
                    BusinessName = table.Column<string>(maxLength: 100, nullable: true),
                    CapitalPercentage = table.Column<decimal>(nullable: false),
                    CashedDate = table.Column<DateTime>(nullable: true),
                    CelPhone = table.Column<string>(maxLength: 15, nullable: true),
                    ClientName = table.Column<string>(maxLength: 100, nullable: true),
                    ContractNumber = table.Column<string>(maxLength: 50, nullable: true),
                    CurrencyId = table.Column<int>(nullable: false),
                    CustomerId = table.Column<string>(nullable: true),
                    DocumentTypeId = table.Column<int>(nullable: false),
                    ImputationNumber1 = table.Column<string>(maxLength: 50, nullable: true),
                    ImputationNumber3Id = table.Column<int>(nullable: false),
                    InvoiceCode = table.Column<string>(maxLength: 50, nullable: true),
                    InvoiceDate = table.Column<DateTime>(nullable: true),
                    InvoiceRequired = table.Column<bool>(nullable: false),
                    ModifiedByUserId = table.Column<int>(nullable: false),
                    OtherProvince1Percentage = table.Column<decimal>(nullable: false),
                    OtherProvince2Percentage = table.Column<decimal>(nullable: false),
                    OtherProvince3Percentage = table.Column<decimal>(nullable: false),
                    ParticularSteps = table.Column<string>(maxLength: 500, nullable: true),
                    PaymentTermId = table.Column<int>(nullable: false),
                    Project = table.Column<string>(maxLength: 100, nullable: true),
                    ProjectId = table.Column<string>(nullable: true),
                    Province1Id = table.Column<int>(nullable: false),
                    Province2Id = table.Column<int>(nullable: false),
                    Province3Id = table.Column<int>(nullable: false),
                    Service = table.Column<string>(nullable: true),
                    ServiceId = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    UserApplicantId = table.Column<int>(nullable: false),
                    WithTax = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solfacs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Solfacs_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "app",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Solfacs_DocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalSchema: "app",
                        principalTable: "DocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Solfacs_ImputationNumbers_ImputationNumber3Id",
                        column: x => x.ImputationNumber3Id,
                        principalSchema: "app",
                        principalTable: "ImputationNumbers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Solfacs_PaymentTerms_PaymentTermId",
                        column: x => x.PaymentTermId,
                        principalSchema: "app",
                        principalTable: "PaymentTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Solfacs_Users_UserApplicantId",
                        column: x => x.UserApplicantId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Analytics",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActivityId = table.Column<int>(nullable: true),
                    AmountEarned = table.Column<string>(maxLength: 500, nullable: true),
                    AmountProject = table.Column<string>(maxLength: 500, nullable: true),
                    BugsAccess = table.Column<bool>(nullable: false),
                    ClientExternalId = table.Column<string>(maxLength: 150, nullable: true),
                    ClientExternalName = table.Column<string>(maxLength: 150, nullable: true),
                    ClientGroupId = table.Column<int>(nullable: true),
                    ClientProjectTfs = table.Column<string>(maxLength: 150, nullable: true),
                    CommercialManager = table.Column<string>(maxLength: 150, nullable: true),
                    ContractNumber = table.Column<string>(maxLength: 50, nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    DirectorId = table.Column<int>(nullable: false),
                    EndDateContract = table.Column<DateTime>(nullable: false),
                    EvalProp = table.Column<bool>(nullable: true),
                    ManagerId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    ProductId = table.Column<int>(nullable: true),
                    Proposal = table.Column<string>(maxLength: 200, nullable: true),
                    PurchaseOrder = table.Column<int>(nullable: false),
                    Service = table.Column<string>(maxLength: 50, nullable: true),
                    SoftwareLaw = table.Column<bool>(nullable: true),
                    SolutionId = table.Column<int>(nullable: true),
                    StartDateContract = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    TechnologyId = table.Column<int>(nullable: true),
                    Title = table.Column<string>(maxLength: 150, nullable: true),
                    UsersQv = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analytics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Analytics_ImputationNumbers_ActivityId",
                        column: x => x.ActivityId,
                        principalSchema: "app",
                        principalTable: "ImputationNumbers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Analytics_ClientGroups_ClientGroupId",
                        column: x => x.ClientGroupId,
                        principalSchema: "app",
                        principalTable: "ClientGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Analytics_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "app",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Analytics_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "app",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Analytics_Solutions_SolutionId",
                        column: x => x.SolutionId,
                        principalSchema: "app",
                        principalTable: "Solutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Analytics_Technologies_TechnologyId",
                        column: x => x.TechnologyId,
                        principalSchema: "app",
                        principalTable: "Technologies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleFunctionality",
                schema: "app",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false),
                    FunctionalityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleFunctionality", x => new { x.RoleId, x.FunctionalityId });
                    table.ForeignKey(
                        name: "FK_RoleFunctionality_Functionalities_FunctionalityId",
                        column: x => x.FunctionalityId,
                        principalSchema: "app",
                        principalTable: "Functionalities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleFunctionality_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "app",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserGroup",
                schema: "app",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    GroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroup", x => new { x.UserId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_UserGroup_Groups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "app",
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGroup_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hitos",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Currency = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ExternalHitoId = table.Column<string>(nullable: true),
                    ExternalProjectId = table.Column<string>(nullable: true),
                    Month = table.Column<short>(nullable: false),
                    SolfacId = table.Column<int>(nullable: false),
                    Total = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hitos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hitos_Solfacs_SolfacId",
                        column: x => x.SolfacId,
                        principalSchema: "app",
                        principalTable: "Solfacs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountName = table.Column<string>(maxLength: 100, nullable: true),
                    Address = table.Column<string>(maxLength: 100, nullable: true),
                    Analytic = table.Column<string>(maxLength: 100, nullable: true),
                    City = table.Column<string>(maxLength: 50, nullable: true),
                    Country = table.Column<string>(maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Cuit = table.Column<string>(maxLength: 100, nullable: true),
                    CustomerId = table.Column<string>(nullable: true),
                    ExcelFile = table.Column<byte[]>(nullable: true),
                    ExcelFileCreatedDate = table.Column<DateTime>(nullable: false),
                    ExcelFileName = table.Column<string>(maxLength: 150, nullable: true),
                    InvoiceNumber = table.Column<string>(maxLength: 50, nullable: true),
                    InvoiceStatus = table.Column<int>(nullable: false),
                    PdfFile = table.Column<byte[]>(nullable: true),
                    PdfFileCreatedDate = table.Column<DateTime>(nullable: false),
                    PdfFileName = table.Column<string>(maxLength: 150, nullable: true),
                    Project = table.Column<string>(maxLength: 100, nullable: true),
                    ProjectId = table.Column<string>(nullable: true),
                    Province = table.Column<string>(maxLength: 100, nullable: true),
                    Service = table.Column<string>(maxLength: 100, nullable: true),
                    ServiceId = table.Column<string>(nullable: true),
                    SolfacId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    Zipcode = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_Solfacs_SolfacId",
                        column: x => x.SolfacId,
                        principalSchema: "app",
                        principalTable: "Solfacs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoices_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolfacAttachments",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    File = table.Column<byte[]>(nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    SolfacId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolfacAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolfacAttachments_Solfacs_SolfacId",
                        column: x => x.SolfacId,
                        principalSchema: "app",
                        principalTable: "Solfacs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolfacHistories",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Comment = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    SolfacId = table.Column<int>(nullable: false),
                    SolfacStatusFrom = table.Column<int>(nullable: false),
                    SolfacStatusTo = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolfacHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolfacHistories_Solfacs_SolfacId",
                        column: x => x.SolfacId,
                        principalSchema: "app",
                        principalTable: "Solfacs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolfacHistories_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Allocations",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AnalyticId = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    Percentage = table.Column<decimal>(nullable: false),
                    ReleaseDate = table.Column<DateTime>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Allocations_Analytics_AnalyticId",
                        column: x => x.AnalyticId,
                        principalSchema: "app",
                        principalTable: "Analytics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Allocations_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "app",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HitoDetails",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 3000, nullable: true),
                    HitoId = table.Column<int>(nullable: false),
                    Quantity = table.Column<short>(nullable: false),
                    Total = table.Column<decimal>(nullable: false),
                    UnitPrice = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HitoDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HitoDetails_Hitos_HitoId",
                        column: x => x.HitoId,
                        principalSchema: "app",
                        principalTable: "Hitos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceHistories",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Comment = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    InvoiceId = table.Column<int>(nullable: false),
                    StatusFrom = table.Column<int>(nullable: false),
                    StatusTo = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceHistories_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalSchema: "app",
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceHistories_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Functionalities_ModuleId",
                schema: "app",
                table: "Functionalities",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_RoleId",
                schema: "app",
                table: "Groups",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Hitos_SolfacId",
                schema: "app",
                table: "Hitos",
                column: "SolfacId");

            migrationBuilder.CreateIndex(
                name: "IX_HitoDetails_HitoId",
                schema: "app",
                table: "HitoDetails",
                column: "HitoId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_SolfacId",
                schema: "app",
                table: "Invoices",
                column: "SolfacId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_UserId",
                schema: "app",
                table: "Invoices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceHistories_InvoiceId",
                schema: "app",
                table: "InvoiceHistories",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceHistories_UserId",
                schema: "app",
                table: "InvoiceHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Solfacs_CurrencyId",
                schema: "app",
                table: "Solfacs",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Solfacs_DocumentTypeId",
                schema: "app",
                table: "Solfacs",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Solfacs_ImputationNumber3Id",
                schema: "app",
                table: "Solfacs",
                column: "ImputationNumber3Id");

            migrationBuilder.CreateIndex(
                name: "IX_Solfacs_PaymentTermId",
                schema: "app",
                table: "Solfacs",
                column: "PaymentTermId");

            migrationBuilder.CreateIndex(
                name: "IX_Solfacs_UserApplicantId",
                schema: "app",
                table: "Solfacs",
                column: "UserApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_SolfacAttachments_SolfacId",
                schema: "app",
                table: "SolfacAttachments",
                column: "SolfacId");

            migrationBuilder.CreateIndex(
                name: "IX_SolfacHistories_SolfacId",
                schema: "app",
                table: "SolfacHistories",
                column: "SolfacId");

            migrationBuilder.CreateIndex(
                name: "IX_SolfacHistories_UserId",
                schema: "app",
                table: "SolfacHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_AnalyticId",
                schema: "app",
                table: "Allocations",
                column: "AnalyticId");

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_EmployeeId",
                schema: "app",
                table: "Allocations",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_ActivityId",
                schema: "app",
                table: "Analytics",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_ClientGroupId",
                schema: "app",
                table: "Analytics",
                column: "ClientGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_CurrencyId",
                schema: "app",
                table: "Analytics",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_ProductId",
                schema: "app",
                table: "Analytics",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_SolutionId",
                schema: "app",
                table: "Analytics",
                column: "SolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_TechnologyId",
                schema: "app",
                table: "Analytics",
                column: "TechnologyId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleFunctionality_FunctionalityId",
                schema: "app",
                table: "RoleFunctionality",
                column: "FunctionalityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroup_GroupId",
                schema: "app",
                table: "UserGroup",
                column: "GroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GlobalSettings",
                schema: "app");

            migrationBuilder.DropTable(
                name: "HitoDetails",
                schema: "app");

            migrationBuilder.DropTable(
                name: "InvoiceHistories",
                schema: "app");

            migrationBuilder.DropTable(
                name: "SolfacAttachments",
                schema: "app");

            migrationBuilder.DropTable(
                name: "SolfacHistories",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Allocations",
                schema: "app");

            migrationBuilder.DropTable(
                name: "EmployeeLicenses",
                schema: "app");

            migrationBuilder.DropTable(
                name: "LicenseTypes",
                schema: "app");

            migrationBuilder.DropTable(
                name: "RoleFunctionality",
                schema: "app");

            migrationBuilder.DropTable(
                name: "UserGroup",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Provinces",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Hitos",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Invoices",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Analytics",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Employees",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Functionalities",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Groups",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Solfacs",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ClientGroups",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Solutions",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Technologies",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Modules",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Currencies",
                schema: "app");

            migrationBuilder.DropTable(
                name: "DocumentTypes",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ImputationNumbers",
                schema: "app");

            migrationBuilder.DropTable(
                name: "PaymentTerms",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "app");
        }
    }
}
