using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "app");

            migrationBuilder.CreateTable(
                name: "BudgetType",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 100, nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientGroups",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 60, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CloseDates",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Day = table.Column<int>(nullable: false),
                    Month = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloseDates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CrmId = table.Column<string>(maxLength: 200, nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    Email = table.Column<string>(maxLength: 100, nullable: true),
                    StatusId = table.Column<int>(nullable: false),
                    Status = table.Column<string>(maxLength: 50, nullable: true),
                    AccountId = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CostCenters",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<int>(maxLength: 3, nullable: false),
                    Letter = table.Column<string>(maxLength: 1, nullable: true),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostCenters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CostDetailCategories",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Default = table.Column<bool>(nullable: false),
                    BelongEmployee = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostDetailCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CostTypes",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 200, nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    Category = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CreditCards",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 100, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 15, nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    CrmId = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CrmId = table.Column<string>(maxLength: 200, nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    Telephone = table.Column<string>(maxLength: 200, nullable: true),
                    Address = table.Column<string>(maxLength: 200, nullable: true),
                    PostalCode = table.Column<string>(maxLength: 200, nullable: true),
                    City = table.Column<string>(maxLength: 200, nullable: true),
                    Province = table.Column<string>(maxLength: 200, nullable: true),
                    Country = table.Column<string>(maxLength: 200, nullable: true),
                    Cuit = table.Column<string>(maxLength: 200, nullable: true),
                    CurrencyId = table.Column<string>(maxLength: 200, nullable: true),
                    CurrencyDescription = table.Column<string>(maxLength: 200, nullable: true),
                    Contact = table.Column<string>(maxLength: 200, nullable: true),
                    PaymentTermCode = table.Column<int>(maxLength: 200, nullable: true),
                    PaymentTermDescription = table.Column<string>(maxLength: 200, nullable: true),
                    OwnerId = table.Column<string>(maxLength: 200, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 60, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeEndNotifications",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmployeeId = table.Column<int>(nullable: false),
                    ApplicantUserId = table.Column<int>(nullable: false),
                    Recipients = table.Column<string>(maxLength: 500, nullable: true),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Created = table.Column<DateTime>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeEndNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeEndReason",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeEndReason", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeHistory",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmployeeNumber = table.Column<string>(maxLength: 50, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    EmployeeData = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeLicenses",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmployeeNumber = table.Column<string>(maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    LicenseTypeNumber = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeLicenses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeProfile",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 150, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeProfileHistory",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmployeeNumber = table.Column<string>(maxLength: 50, nullable: true),
                    EmployeeData = table.Column<string>(nullable: true),
                    EmployeePreviousData = table.Column<string>(nullable: true),
                    ModifiedFields = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProfileHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSyncActions",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmployeeNumber = table.Column<string>(maxLength: 50, nullable: true),
                    Status = table.Column<string>(maxLength: 20, nullable: true),
                    EmployeeData = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Created = table.Column<DateTime>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSyncActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InternalFileName = table.Column<Guid>(maxLength: 100, nullable: false),
                    FileName = table.Column<string>(maxLength: 500, nullable: true),
                    FileType = table.Column<string>(maxLength: 10, nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HealthInsurances",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 400, nullable: true),
                    Created = table.Column<DateTime>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthInsurances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Holidays",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    DataSource = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holidays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImputationNumbers",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 50, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImputationNumbers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LicenseTypes",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 300, nullable: true),
                    Days = table.Column<int>(nullable: false),
                    WithPayment = table.Column<bool>(nullable: false),
                    TaskId = table.Column<int>(nullable: false),
                    CertificateRequired = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(maxLength: 50, nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    Comment = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 50, nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Code = table.Column<string>(maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthsReturns",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 100, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthsReturns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Opportunities",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CrmId = table.Column<string>(maxLength: 200, nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    Number = table.Column<string>(maxLength: 50, nullable: true),
                    ActualValue = table.Column<decimal>(nullable: true),
                    ContactId = table.Column<string>(maxLength: 200, nullable: true),
                    ParentContactName = table.Column<string>(maxLength: 200, nullable: true),
                    ProjectManagerId = table.Column<string>(maxLength: 200, nullable: true),
                    ProjectManagerName = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opportunities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTerms",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 60, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTerms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrepaidHealths",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HealthInsuranceCode = table.Column<int>(nullable: false),
                    PrepaidHealthCode = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 400, nullable: true),
                    Amount = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrepaidHealths", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prepaids",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 250, nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    Code = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prepaids", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 60, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 75, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CrmId = table.Column<string>(maxLength: 200, nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    AccountId = table.Column<string>(maxLength: 200, nullable: true),
                    ServiceId = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Incomes = table.Column<decimal>(nullable: false),
                    RealIncomes = table.Column<decimal>(nullable: false),
                    OpportunityId = table.Column<string>(maxLength: 200, nullable: true),
                    OpportunityName = table.Column<string>(maxLength: 200, nullable: true),
                    OpportunityNumber = table.Column<string>(maxLength: 200, nullable: true),
                    TotalAmmount = table.Column<decimal>(nullable: false),
                    Currency = table.Column<string>(maxLength: 200, nullable: true),
                    CurrencyId = table.Column<string>(maxLength: 200, nullable: true),
                    Remito = table.Column<bool>(nullable: false),
                    Integrator = table.Column<string>(maxLength: 200, nullable: true),
                    IntegratorId = table.Column<string>(maxLength: 200, nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    PrincipalContactId = table.Column<Guid>(nullable: true),
                    PrincipalContactName = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Provinces",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 60, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderOptions",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 60, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderOptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReasonCauses",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 75, nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReasonCauses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportsPowerBi",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Resource = table.Column<string>(maxLength: 200, nullable: true),
                    Seniority = table.Column<string>(maxLength: 200, nullable: true),
                    Profile = table.Column<string>(maxLength: 200, nullable: true),
                    Technology = table.Column<string>(maxLength: 200, nullable: true),
                    Manager = table.Column<string>(maxLength: 200, nullable: true),
                    Month1 = table.Column<decimal>(nullable: false),
                    Month2 = table.Column<decimal>(nullable: false),
                    Month3 = table.Column<decimal>(nullable: false),
                    Month4 = table.Column<decimal>(nullable: false),
                    FirstMonth = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportsPowerBi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResourceAssignments",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 75, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceAssignments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 50, nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Code = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Seniorities",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 75, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seniorities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CrmId = table.Column<string>(maxLength: 200, nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    AccountId = table.Column<string>(maxLength: 200, nullable: true),
                    AccountName = table.Column<string>(maxLength: 200, nullable: true),
                    Industry = table.Column<string>(maxLength: 200, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Manager = table.Column<string>(maxLength: 200, nullable: true),
                    ManagerId = table.Column<string>(maxLength: 200, nullable: true),
                    ServiceType = table.Column<string>(maxLength: 200, nullable: true),
                    ServiceTypeId = table.Column<int>(nullable: false),
                    SolutionType = table.Column<string>(maxLength: 200, nullable: true),
                    SolutionTypeId = table.Column<int>(nullable: false),
                    TechnologyType = table.Column<string>(maxLength: 200, nullable: true),
                    TechnologyTypeId = table.Column<int>(nullable: false),
                    Analytic = table.Column<string>(maxLength: 200, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTypes",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 60, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(maxLength: 100, nullable: false),
                    Value = table.Column<string>(maxLength: 500, nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Category = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 75, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SoftwareLaws",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 60, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoftwareLaws", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Solutions",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 60, nullable: true),
                    Active = table.Column<bool>(nullable: false)
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
                    Text = table.Column<string>(maxLength: 60, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Technologies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimeHirings",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 75, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeHirings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 150, nullable: false),
                    UserName = table.Column<string>(maxLength: 150, nullable: false),
                    Email = table.Column<string>(maxLength: 150, nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    ExternalManagerId = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSources",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 50, nullable: true),
                    SourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 50, nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "app",
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CostDetailSubcategories",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    CostDetailCategoryId = table.Column<int>(nullable: false),
                    Codigo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostDetailSubcategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostDetailSubcategories_CostDetailCategories_CostDetailCategoryId",
                        column: x => x.CostDetailCategoryId,
                        principalSchema: "app",
                        principalTable: "CostDetailCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyExchanges",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    Exchange = table.Column<decimal>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyExchanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrencyExchanges_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "app",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Certificates",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 150, nullable: true),
                    AccountId = table.Column<string>(maxLength: 100, nullable: true),
                    AccountName = table.Column<string>(maxLength: 100, nullable: true),
                    Year = table.Column<int>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: false),
                    UpdateByUser = table.Column<string>(maxLength: 50, nullable: true),
                    FileId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificates_Files_FileId",
                        column: x => x.FileId,
                        principalSchema: "app",
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Functionalities",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 50, nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Code = table.Column<string>(maxLength: 20, nullable: false),
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
                name: "PrepaidImportedData",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Period = table.Column<DateTime>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    EmployeeName = table.Column<string>(maxLength: 100, nullable: true),
                    EmployeeNumber = table.Column<string>(maxLength: 15, nullable: true),
                    PrepaidBeneficiaries = table.Column<int>(nullable: false),
                    TigerBeneficiaries = table.Column<int>(nullable: false),
                    PrepaidPlan = table.Column<string>(maxLength: 100, nullable: true),
                    TigerPlan = table.Column<string>(maxLength: 100, nullable: true),
                    PrepaidCost = table.Column<decimal>(nullable: false),
                    TigerCost = table.Column<decimal>(nullable: false),
                    CostDifference = table.Column<decimal>(nullable: false),
                    Dni = table.Column<string>(maxLength: 15, nullable: true),
                    Cuil = table.Column<string>(maxLength: 15, nullable: true),
                    Comments = table.Column<string>(maxLength: 500, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    PrepaidId = table.Column<int>(nullable: true),
                    Closed = table.Column<bool>(nullable: false),
                    NetoProvider = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrepaidImportedData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrepaidImportedData_Prepaids_PrepaidId",
                        column: x => x.PrepaidId,
                        principalSchema: "app",
                        principalTable: "Prepaids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 200, nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    RoleId = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Email = table.Column<string>(maxLength: 100, nullable: false),
                    Code = table.Column<string>(maxLength: 100, nullable: true)
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Areas",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 500, nullable: true),
                    ResponsableUserId = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Areas_Users_ResponsableUserId",
                        column: x => x.ResponsableUserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Delegations",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    GrantedUserId = table.Column<int>(nullable: false),
                    AnalyticSourceId = table.Column<int>(nullable: true),
                    UserSourceId = table.Column<int>(nullable: true),
                    EmployeeSourceId = table.Column<int>(nullable: true),
                    SourceType = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Delegations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Delegations_Users_GrantedUserId",
                        column: x => x.GrantedUserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Delegations_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmployeeNumber = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Birthday = table.Column<DateTime>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Profile = table.Column<string>(maxLength: 100, nullable: true),
                    Technology = table.Column<string>(maxLength: 300, nullable: true),
                    Seniority = table.Column<string>(maxLength: 100, nullable: true),
                    BillingPercentage = table.Column<decimal>(nullable: false),
                    Created = table.Column<DateTime>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: true),
                    CreatedByUser = table.Column<string>(maxLength: 50, nullable: true),
                    Address = table.Column<string>(maxLength: 400, nullable: true),
                    Location = table.Column<string>(maxLength: 200, nullable: true),
                    Province = table.Column<string>(maxLength: 200, nullable: true),
                    Country = table.Column<string>(maxLength: 200, nullable: true),
                    HealthInsuranceCode = table.Column<int>(nullable: false),
                    PrepaidHealthCode = table.Column<int>(nullable: false),
                    OfficeAddress = table.Column<string>(maxLength: 400, nullable: true),
                    ExtraHolidaysQuantity = table.Column<int>(nullable: false),
                    ExtraHolidaysQuantityByLaw = table.Column<int>(nullable: false),
                    HasExtraHolidays = table.Column<bool>(nullable: false),
                    HolidaysPending = table.Column<int>(nullable: false),
                    ExamDaysTaken = table.Column<int>(nullable: false),
                    Email = table.Column<string>(maxLength: 150, nullable: true),
                    HolidaysByLaw = table.Column<int>(nullable: false),
                    HolidaysPendingByLaw = table.Column<int>(nullable: false),
                    BusinessHours = table.Column<int>(nullable: false),
                    BusinessHoursDescription = table.Column<string>(maxLength: 150, nullable: true),
                    EndReason = table.Column<string>(maxLength: 2000, nullable: true),
                    TypeEndReasonId = table.Column<int>(nullable: true),
                    ManagerId = table.Column<int>(nullable: true),
                    DocumentNumberType = table.Column<string>(maxLength: 100, nullable: true),
                    DocumentNumber = table.Column<int>(nullable: false),
                    Cuil = table.Column<decimal>(type: "decimal(12, 0)", nullable: false),
                    PhoneCountryCode = table.Column<int>(nullable: false),
                    PhoneAreaCode = table.Column<int>(nullable: false),
                    PhoneNumber = table.Column<string>(maxLength: 100, nullable: true),
                    IsExternal = table.Column<bool>(nullable: false),
                    HasCreditCard = table.Column<bool>(nullable: false),
                    Bank = table.Column<string>(maxLength: 200, nullable: true),
                    Salary = table.Column<string>(maxLength: 200, nullable: true),
                    BeneficiariesCount = table.Column<int>(nullable: false),
                    PrepaidAmount = table.Column<string>(maxLength: 200, nullable: true),
                    PrepaidPlan = table.Column<string>(maxLength: 100, nullable: true),
                    AssignComments = table.Column<string>(maxLength: 1000, nullable: true),
                    ExcludeForTigerReport = table.Column<bool>(nullable: false),
                    Activity = table.Column<int>(nullable: false),
                    ContractType = table.Column<string>(nullable: true),
                    Nationality = table.Column<string>(nullable: true),
                    OnTestPeriod = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Users_ManagerId",
                        column: x => x.ManagerId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Employees_EmployeeEndReason_TypeEndReasonId",
                        column: x => x.TypeEndReasonId,
                        principalSchema: "app",
                        principalTable: "EmployeeEndReason",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobSearchs",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsStaffDesc = table.Column<string>(maxLength: 150, nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    RecruiterId = table.Column<int>(nullable: true),
                    ReasonCauseId = table.Column<int>(nullable: false),
                    ClientId = table.Column<int>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    MaximunSalary = table.Column<decimal>(nullable: false),
                    Comments = table.Column<string>(maxLength: 3000, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ReopenDate = table.Column<DateTime>(nullable: true),
                    SuspendedDate = table.Column<DateTime>(nullable: true),
                    CloseDate = table.Column<DateTime>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 50, nullable: true),
                    ReasonComments = table.Column<string>(maxLength: 1000, nullable: true),
                    TimeHiringId = table.Column<int>(nullable: false),
                    YearsExperience = table.Column<int>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Telephone = table.Column<string>(nullable: true),
                    ClientContact = table.Column<string>(nullable: true),
                    JobType = table.Column<int>(nullable: false),
                    Language = table.Column<string>(maxLength: 100, nullable: true),
                    LanguageRequired = table.Column<bool>(nullable: false),
                    Study = table.Column<string>(maxLength: 100, nullable: true),
                    StudyRequired = table.Column<bool>(nullable: false),
                    JobTime = table.Column<string>(maxLength: 100, nullable: true),
                    Location = table.Column<string>(maxLength: 200, nullable: true),
                    Benefits = table.Column<string>(maxLength: 3000, nullable: true),
                    Observations = table.Column<string>(maxLength: 3000, nullable: true),
                    TasksToDo = table.Column<string>(maxLength: 3000, nullable: true),
                    HasExtraHours = table.Column<bool>(nullable: false),
                    ExtraHoursPaid = table.Column<bool>(nullable: false),
                    HasGuards = table.Column<bool>(nullable: false),
                    GuardsPaid = table.Column<bool>(nullable: false),
                    Area = table.Column<string>(maxLength: 3000, nullable: true),
                    MarketStudy = table.Column<string>(maxLength: 150, nullable: true),
                    IsMarketStudy = table.Column<bool>(nullable: false),
                    ResourceAssignmentId = table.Column<int>(nullable: false),
                    IsStaff = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSearchs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSearchs_Customers_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "app",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSearchs_ReasonCauses_ReasonCauseId",
                        column: x => x.ReasonCauseId,
                        principalSchema: "app",
                        principalTable: "ReasonCauses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSearchs_Users_RecruiterId",
                        column: x => x.RecruiterId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSearchs_ResourceAssignments_ResourceAssignmentId",
                        column: x => x.ResourceAssignmentId,
                        principalSchema: "app",
                        principalTable: "ResourceAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSearchs_TimeHirings_TimeHiringId",
                        column: x => x.TimeHiringId,
                        principalSchema: "app",
                        principalTable: "TimeHirings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSearchs_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sectors",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 500, nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    ResponsableUserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sectors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sectors_Users_ResponsableUserId",
                        column: x => x.ResponsableUserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowStates",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 300, nullable: true),
                    ActionName = table.Column<string>(maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedById = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowStates_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowStates_Users_ModifiedById",
                        column: x => x.ModifiedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowTypes",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 300, nullable: true),
                    Code = table.Column<string>(maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowTypes_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowTypes_Users_ModifiedById",
                        column: x => x.ModifiedById,
                        principalSchema: "app",
                        principalTable: "Users",
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
                    GroupId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
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
                name: "PurchaseOrders",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 1000, nullable: true),
                    Number = table.Column<string>(maxLength: 150, nullable: true),
                    AccountId = table.Column<string>(maxLength: 150, nullable: true),
                    AccountName = table.Column<string>(maxLength: 150, nullable: true),
                    ReceptionDate = table.Column<DateTime>(nullable: false),
                    AreaId = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: false),
                    UpdateByUser = table.Column<string>(maxLength: 25, nullable: true),
                    FileId = table.Column<int>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    Adjustment = table.Column<bool>(nullable: false),
                    AdjustmentDate = table.Column<DateTime>(nullable: true),
                    FicheDeSignature = table.Column<string>(maxLength: 200, nullable: true),
                    PaymentForm = table.Column<string>(maxLength: 200, nullable: true),
                    Margin = table.Column<decimal>(nullable: false),
                    Comments = table.Column<string>(maxLength: 2000, nullable: true),
                    CommentsForAdjustment = table.Column<string>(maxLength: 2000, nullable: true),
                    Proposal = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Areas_AreaId",
                        column: x => x.AreaId,
                        principalSchema: "app",
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Files_FileId",
                        column: x => x.FileId,
                        principalSchema: "app",
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdvancementSalaryDiscounts",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<decimal>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvancementSalaryDiscounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdvancementSalaryDiscounts_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "app",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeCategories",
                schema: "app",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeCategories", x => new { x.CategoryId, x.EmployeeId });
                    table.ForeignKey(
                        name: "FK_EmployeeCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "app",
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeCategories_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "app",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SocialCharges",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Year = table.Column<int>(nullable: false),
                    Month = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    ChargesTotal = table.Column<string>(nullable: true),
                    SalaryTotal = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialCharges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialCharges_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "app",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobSearchHistories",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UserName = table.Column<string>(maxLength: 50, nullable: true),
                    StatusFromId = table.Column<int>(nullable: false),
                    StatusToId = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(maxLength: 1000, nullable: true),
                    ReasonCauseId = table.Column<int>(nullable: false),
                    JobSearchId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSearchHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSearchHistories_JobSearchs_JobSearchId",
                        column: x => x.JobSearchId,
                        principalSchema: "app",
                        principalTable: "JobSearchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSearchHistories_ReasonCauses_ReasonCauseId",
                        column: x => x.ReasonCauseId,
                        principalSchema: "app",
                        principalTable: "ReasonCauses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobSearchProfiles",
                schema: "app",
                columns: table => new
                {
                    JobSearchId = table.Column<int>(nullable: false),
                    ProfileId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSearchProfiles", x => new { x.JobSearchId, x.ProfileId });
                    table.ForeignKey(
                        name: "FK_JobSearchProfiles_JobSearchs_JobSearchId",
                        column: x => x.JobSearchId,
                        principalSchema: "app",
                        principalTable: "JobSearchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSearchProfiles_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "app",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobSearchSeniorities",
                schema: "app",
                columns: table => new
                {
                    JobSearchId = table.Column<int>(nullable: false),
                    SeniorityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSearchSeniorities", x => new { x.JobSearchId, x.SeniorityId });
                    table.ForeignKey(
                        name: "FK_JobSearchSeniorities_JobSearchs_JobSearchId",
                        column: x => x.JobSearchId,
                        principalSchema: "app",
                        principalTable: "JobSearchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSearchSeniorities_Seniorities_SeniorityId",
                        column: x => x.SeniorityId,
                        principalSchema: "app",
                        principalTable: "Seniorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobSearchSkillNotRequired",
                schema: "app",
                columns: table => new
                {
                    JobSearchId = table.Column<int>(nullable: false),
                    SkillId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSearchSkillNotRequired", x => new { x.JobSearchId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_JobSearchSkillNotRequired_JobSearchs_JobSearchId",
                        column: x => x.JobSearchId,
                        principalSchema: "app",
                        principalTable: "JobSearchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSearchSkillNotRequired_Skills_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "app",
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobSearchSkills",
                schema: "app",
                columns: table => new
                {
                    JobSearchId = table.Column<int>(nullable: false),
                    SkillId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSearchSkills", x => new { x.JobSearchId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_JobSearchSkills_JobSearchs_JobSearchId",
                        column: x => x.JobSearchId,
                        principalSchema: "app",
                        principalTable: "JobSearchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSearchSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "app",
                        principalTable: "Skills",
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
                    Title = table.Column<string>(maxLength: 150, nullable: true),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    TitleId = table.Column<int>(nullable: false),
                    CostCenterId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    AccountId = table.Column<string>(maxLength: 150, nullable: true),
                    AccountName = table.Column<string>(maxLength: 150, nullable: true),
                    ServiceName = table.Column<string>(maxLength: 200, nullable: true),
                    ServiceId = table.Column<string>(nullable: true),
                    ActivityId = table.Column<int>(nullable: true),
                    StartDateContract = table.Column<DateTime>(nullable: false),
                    EndDateContract = table.Column<DateTime>(nullable: false),
                    SectorId = table.Column<int>(nullable: false),
                    ManagerId = table.Column<int>(nullable: true),
                    CommercialManagerId = table.Column<int>(nullable: true),
                    Proposal = table.Column<string>(maxLength: 2000, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    SolutionId = table.Column<int>(nullable: true),
                    TechnologyId = table.Column<int>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    ClientGroupId = table.Column<int>(nullable: true),
                    ServiceTypeId = table.Column<int>(nullable: true),
                    UsersQv = table.Column<string>(maxLength: 500, nullable: true),
                    SoftwareLawId = table.Column<int>(nullable: true),
                    ClosedBy = table.Column<string>(maxLength: 25, nullable: true),
                    ClosedAt = table.Column<DateTime>(nullable: true)
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
                        name: "FK_Analytics_Users_CommercialManagerId",
                        column: x => x.CommercialManagerId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Analytics_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalSchema: "app",
                        principalTable: "CostCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Analytics_Users_ManagerId",
                        column: x => x.ManagerId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Analytics_Sectors_SectorId",
                        column: x => x.SectorId,
                        principalSchema: "app",
                        principalTable: "Sectors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Analytics_ServiceTypes_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalSchema: "app",
                        principalTable: "ServiceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Analytics_SoftwareLaws_SoftwareLawId",
                        column: x => x.SoftwareLawId,
                        principalSchema: "app",
                        principalTable: "SoftwareLaws",
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
                name: "Licenses",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmployeeId = table.Column<int>(nullable: false),
                    ManagerId = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    SectorId = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    TypeId = table.Column<int>(nullable: false),
                    WithPayment = table.Column<bool>(nullable: false),
                    DaysQuantity = table.Column<int>(nullable: false),
                    HasCertificate = table.Column<bool>(nullable: false),
                    Parcial = table.Column<bool>(nullable: false),
                    Final = table.Column<bool>(nullable: false),
                    Comments = table.Column<string>(maxLength: 500, nullable: true),
                    ExamDescription = table.Column<string>(maxLength: 200, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    DaysQuantityByLaw = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Licenses_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "app",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Licenses_Users_ManagerId",
                        column: x => x.ManagerId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Licenses_Sectors_SectorId",
                        column: x => x.SectorId,
                        principalSchema: "app",
                        principalTable: "Sectors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Licenses_LicenseTypes_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "app",
                        principalTable: "LicenseTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Workflows",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    Version = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    WorkflowTypeId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workflows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workflows_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Workflows_Users_ModifiedById",
                        column: x => x.ModifiedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Workflows_WorkflowTypes_WorkflowTypeId",
                        column: x => x.WorkflowTypeId,
                        principalSchema: "app",
                        principalTable: "WorkflowTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderAmmountDetails",
                schema: "app",
                columns: table => new
                {
                    PurchaseOrderId = table.Column<int>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    Balance = table.Column<decimal>(nullable: false),
                    Ammount = table.Column<decimal>(nullable: false),
                    Adjustment = table.Column<decimal>(nullable: false),
                    AdjustmentBalance = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderAmmountDetails", x => new { x.PurchaseOrderId, x.CurrencyId });
                    table.ForeignKey(
                        name: "FK_PurchaseOrderAmmountDetails_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "app",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderAmmountDetails_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalSchema: "app",
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderHistories",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    From = table.Column<int>(nullable: false),
                    To = table.Column<int>(nullable: false),
                    PurchaseOrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderHistories_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalSchema: "app",
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderHistories_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Solfacs",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BusinessName = table.Column<string>(maxLength: 100, nullable: true),
                    CelPhone = table.Column<string>(maxLength: 50, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    InvoiceCode = table.Column<string>(maxLength: 50, nullable: true),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    WithTax = table.Column<bool>(nullable: false),
                    CapitalPercentage = table.Column<decimal>(nullable: false),
                    BuenosAiresPercentage = table.Column<decimal>(nullable: false),
                    OtherProvince1Percentage = table.Column<decimal>(nullable: false),
                    OtherProvince2Percentage = table.Column<decimal>(nullable: false),
                    OtherProvince3Percentage = table.Column<decimal>(nullable: false),
                    Province1Id = table.Column<int>(nullable: false),
                    Province2Id = table.Column<int>(nullable: false),
                    Province3Id = table.Column<int>(nullable: false),
                    ParticularSteps = table.Column<string>(maxLength: 1000, nullable: true),
                    Analytic = table.Column<string>(nullable: true),
                    ContactEmail = table.Column<string>(maxLength: 100, nullable: true),
                    CurrencyId = table.Column<int>(nullable: false),
                    DocumentTypeId = table.Column<int>(nullable: false),
                    UserApplicantId = table.Column<int>(nullable: false),
                    ImputationNumber1 = table.Column<string>(maxLength: 50, nullable: true),
                    ImputationNumber3Id = table.Column<int>(nullable: false),
                    PaymentTerm = table.Column<string>(maxLength: 300, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    ModifiedByUserId = table.Column<int>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    InvoiceDate = table.Column<DateTime>(nullable: true),
                    CashedDate = table.Column<DateTime>(nullable: true),
                    ProjectId = table.Column<string>(nullable: true),
                    Project = table.Column<string>(maxLength: 1000, nullable: true),
                    AccountId = table.Column<string>(nullable: true),
                    AccountName = table.Column<string>(maxLength: 100, nullable: true),
                    ServiceId = table.Column<string>(nullable: true),
                    Service = table.Column<string>(nullable: true),
                    InvoiceRequired = table.Column<bool>(nullable: false),
                    Integrator = table.Column<string>(maxLength: 300, nullable: true),
                    IntegratorId = table.Column<string>(maxLength: 50, nullable: true),
                    Manager = table.Column<string>(maxLength: 300, nullable: true),
                    ManagerId = table.Column<string>(maxLength: 100, nullable: true),
                    PurchaseOrderId = table.Column<int>(nullable: true),
                    OpportunityNumber = table.Column<string>(maxLength: 100, nullable: true),
                    CurrencyExchange = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    IdToCompareByCreditNote = table.Column<int>(nullable: true)
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
                        name: "FK_Solfacs_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalSchema: "app",
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Solfacs_Users_UserApplicantId",
                        column: x => x.UserApplicantId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SocialChargeItems",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountName = table.Column<string>(maxLength: 500, nullable: true),
                    AccountNumber = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    SocialChargeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialChargeItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialChargeItems_SocialCharges_SocialChargeId",
                        column: x => x.SocialChargeId,
                        principalSchema: "app",
                        principalTable: "SocialCharges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Allocations",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StartDate = table.Column<DateTime>(nullable: false),
                    Percentage = table.Column<decimal>(nullable: false),
                    AnalyticId = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    ReleaseDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 50, nullable: true),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    RealPercentage = table.Column<decimal>(nullable: false)
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
                name: "Applicants",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LastName = table.Column<string>(maxLength: 75, nullable: true),
                    FirstName = table.Column<string>(maxLength: 75, nullable: true),
                    Comments = table.Column<string>(maxLength: 3000, nullable: true),
                    Email = table.Column<string>(maxLength: 75, nullable: true),
                    RecommendedByUserId = table.Column<int>(nullable: true),
                    CountryCode1 = table.Column<string>(maxLength: 5, nullable: true),
                    AreaCode1 = table.Column<string>(maxLength: 5, nullable: true),
                    Telephone1 = table.Column<string>(maxLength: 100, nullable: true),
                    CountryCode2 = table.Column<string>(maxLength: 5, nullable: true),
                    AreaCode2 = table.Column<string>(maxLength: 5, nullable: true),
                    DocumentNumber = table.Column<string>(maxLength: 10, nullable: true),
                    Telephone2 = table.Column<string>(maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 25, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Nationality = table.Column<string>(maxLength: 50, nullable: true),
                    CivilStatus = table.Column<string>(maxLength: 50, nullable: true),
                    Address = table.Column<string>(maxLength: 100, nullable: true),
                    Cuil = table.Column<string>(maxLength: 12, nullable: true),
                    Prepaid = table.Column<string>(maxLength: 100, nullable: true),
                    ProfileId = table.Column<int>(nullable: true),
                    Office = table.Column<string>(maxLength: 100, nullable: true),
                    Salary = table.Column<decimal>(nullable: true),
                    ManagerId = table.Column<int>(nullable: true),
                    AnalyticId = table.Column<int>(nullable: true),
                    ProjectId = table.Column<int>(nullable: true),
                    BirthDate = table.Column<DateTime>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    Aggreements = table.Column<string>(maxLength: 3000, nullable: true),
                    SkillId = table.Column<int>(nullable: true),
                    SeniorityId = table.Column<int>(nullable: true),
                    CustomerId = table.Column<int>(nullable: true),
                    ReasonCauseId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applicants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applicants_Analytics_AnalyticId",
                        column: x => x.AnalyticId,
                        principalSchema: "app",
                        principalTable: "Analytics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applicants_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "app",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applicants_Users_ManagerId",
                        column: x => x.ManagerId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applicants_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "app",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applicants_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "app",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applicants_ReasonCauses_ReasonCauseId",
                        column: x => x.ReasonCauseId,
                        principalSchema: "app",
                        principalTable: "ReasonCauses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applicants_Users_RecommendedByUserId",
                        column: x => x.RecommendedByUserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applicants_Seniorities_SeniorityId",
                        column: x => x.SeniorityId,
                        principalSchema: "app",
                        principalTable: "Seniorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applicants_Skills_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "app",
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ManagementReports",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AnalyticId = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    StateId = table.Column<int>(nullable: true),
                    StateGenerated = table.Column<bool>(nullable: false),
                    AcumulatedCosts = table.Column<decimal>(nullable: true),
                    AcumulatedSales = table.Column<decimal>(nullable: true),
                    AcumulatedPeriod = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagementReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagementReports_Analytics_AnalyticId",
                        column: x => x.AnalyticId,
                        principalSchema: "app",
                        principalTable: "Analytics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManagementReports_BudgetType_StateId",
                        column: x => x.StateId,
                        principalSchema: "app",
                        principalTable: "BudgetType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderAnalytics",
                schema: "app",
                columns: table => new
                {
                    PurchaseOrderId = table.Column<int>(nullable: false),
                    AnalyticId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderAnalytics", x => new { x.PurchaseOrderId, x.AnalyticId });
                    table.ForeignKey(
                        name: "FK_PurchaseOrderAnalytics_Analytics_AnalyticId",
                        column: x => x.AnalyticId,
                        principalSchema: "app",
                        principalTable: "Analytics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderAnalytics_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalSchema: "app",
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkTimes",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AnalyticId = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    TaskId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Hours = table.Column<decimal>(nullable: false),
                    Source = table.Column<string>(maxLength: 50, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ApprovalUserId = table.Column<int>(nullable: true),
                    UserComment = table.Column<string>(maxLength: 500, nullable: true),
                    ApprovalComment = table.Column<string>(maxLength: 500, nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    Reference = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkTimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkTimes_Analytics_AnalyticId",
                        column: x => x.AnalyticId,
                        principalSchema: "app",
                        principalTable: "Analytics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkTimes_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "app",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkTimes_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalSchema: "app",
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkTimes_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LicenseFiles",
                schema: "app",
                columns: table => new
                {
                    LicenseId = table.Column<int>(nullable: false),
                    FileId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseFiles", x => new { x.LicenseId, x.FileId });
                    table.ForeignKey(
                        name: "FK_LicenseFiles_Files_FileId",
                        column: x => x.FileId,
                        principalSchema: "app",
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LicenseFiles_Licenses_LicenseId",
                        column: x => x.LicenseId,
                        principalSchema: "app",
                        principalTable: "Licenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LicenseHistories",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    LicenseStatusFrom = table.Column<int>(nullable: false),
                    LicenseStatusTo = table.Column<int>(nullable: false),
                    LicenseId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LicenseHistories_Licenses_LicenseId",
                        column: x => x.LicenseId,
                        principalSchema: "app",
                        principalTable: "Licenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LicenseHistories_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Advancements",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserApplicantId = table.Column<int>(nullable: false),
                    StatusId = table.Column<int>(nullable: false),
                    InWorkflowProcess = table.Column<bool>(nullable: false),
                    UsersAlreadyApproved = table.Column<string>(nullable: true),
                    PaymentForm = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    MonthsReturnId = table.Column<int>(nullable: true),
                    StartDateReturn = table.Column<DateTime>(nullable: true),
                    CurrencyId = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    AdvancementReturnForm = table.Column<string>(maxLength: 200, nullable: true),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Ammount = table.Column<decimal>(nullable: false),
                    WorkflowId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advancements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Advancements_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "app",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Advancements_MonthsReturns_MonthsReturnId",
                        column: x => x.MonthsReturnId,
                        principalSchema: "app",
                        principalTable: "MonthsReturns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Advancements_WorkflowStates_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "app",
                        principalTable: "WorkflowStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Advancements_Users_UserApplicantId",
                        column: x => x.UserApplicantId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Advancements_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Advancements_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalSchema: "app",
                        principalTable: "Workflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Refunds",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserApplicantId = table.Column<int>(nullable: false),
                    StatusId = table.Column<int>(nullable: false),
                    InWorkflowProcess = table.Column<bool>(nullable: false),
                    UsersAlreadyApproved = table.Column<string>(nullable: true),
                    AnalyticId = table.Column<int>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    TotalAmmount = table.Column<decimal>(nullable: false),
                    CreditCardId = table.Column<int>(nullable: true),
                    WorkflowId = table.Column<int>(nullable: false),
                    LastRefund = table.Column<bool>(nullable: false),
                    CashReturn = table.Column<bool>(nullable: false),
                    CurrencyExchange = table.Column<decimal>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Refunds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Refunds_Analytics_AnalyticId",
                        column: x => x.AnalyticId,
                        principalSchema: "app",
                        principalTable: "Analytics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Refunds_CreditCards_CreditCardId",
                        column: x => x.CreditCardId,
                        principalSchema: "app",
                        principalTable: "CreditCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Refunds_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "app",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Refunds_WorkflowStates_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "app",
                        principalTable: "WorkflowStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Refunds_Users_UserApplicantId",
                        column: x => x.UserApplicantId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Refunds_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Refunds_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalSchema: "app",
                        principalTable: "Workflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowReadAccesses",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WorkflowId = table.Column<int>(nullable: false),
                    UserSourceId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowReadAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowReadAccesses_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowReadAccesses_Users_ModifiedById",
                        column: x => x.ModifiedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowReadAccesses_UserSources_UserSourceId",
                        column: x => x.UserSourceId,
                        principalSchema: "app",
                        principalTable: "UserSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowReadAccesses_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalSchema: "app",
                        principalTable: "Workflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowStateTransitions",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActualWorkflowStateId = table.Column<int>(nullable: false),
                    NextWorkflowStateId = table.Column<int>(nullable: false),
                    WorkflowId = table.Column<int>(nullable: false),
                    NotificationCode = table.Column<string>(maxLength: 50, nullable: true),
                    ValidationCode = table.Column<string>(maxLength: 50, nullable: true),
                    ConditionCode = table.Column<string>(nullable: true),
                    ParameterCode = table.Column<string>(maxLength: 50, nullable: true),
                    OnSuccessCode = table.Column<string>(maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowStateTransitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowStateTransitions_WorkflowStates_ActualWorkflowStateId",
                        column: x => x.ActualWorkflowStateId,
                        principalSchema: "app",
                        principalTable: "WorkflowStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowStateTransitions_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowStateTransitions_Users_ModifiedById",
                        column: x => x.ModifiedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowStateTransitions_WorkflowStates_NextWorkflowStateId",
                        column: x => x.NextWorkflowStateId,
                        principalSchema: "app",
                        principalTable: "WorkflowStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowStateTransitions_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalSchema: "app",
                        principalTable: "Workflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Hitos",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    Total = table.Column<decimal>(nullable: false),
                    Currency = table.Column<string>(maxLength: 10, nullable: true),
                    Month = table.Column<short>(nullable: false),
                    SolfacId = table.Column<int>(nullable: false),
                    ProjectId = table.Column<string>(nullable: true),
                    ExternalHitoId = table.Column<string>(nullable: true),
                    CurrencyId = table.Column<string>(maxLength: 150, nullable: true),
                    OpportunityId = table.Column<string>(maxLength: 150, nullable: true),
                    ManagerId = table.Column<string>(maxLength: 150, nullable: true),
                    Created = table.Column<DateTime>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: true),
                    OriginalAmount = table.Column<decimal>(nullable: true)
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
                    Zipcode = table.Column<string>(maxLength: 50, nullable: true),
                    City = table.Column<string>(maxLength: 50, nullable: true),
                    Province = table.Column<string>(maxLength: 100, nullable: true),
                    Country = table.Column<string>(maxLength: 100, nullable: true),
                    Cuit = table.Column<string>(maxLength: 100, nullable: true),
                    Service = table.Column<string>(maxLength: 100, nullable: true),
                    Project = table.Column<string>(maxLength: 100, nullable: true),
                    ProjectId = table.Column<string>(nullable: true),
                    Analytic = table.Column<string>(maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    InvoiceStatus = table.Column<int>(nullable: false),
                    InvoiceNumber = table.Column<string>(maxLength: 50, nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    SolfacId = table.Column<int>(nullable: true),
                    AccountId = table.Column<string>(nullable: true),
                    ServiceId = table.Column<string>(nullable: true),
                    ExcelFileId = table.Column<int>(nullable: true),
                    PdfFileId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_Files_ExcelFileId",
                        column: x => x.ExcelFileId,
                        principalSchema: "app",
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoices_Files_PdfFileId",
                        column: x => x.PdfFileId,
                        principalSchema: "app",
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolfacAttachments",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    File = table.Column<byte[]>(nullable: true),
                    Name = table.Column<string>(maxLength: 500, nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
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
                name: "SolfacCertificates",
                schema: "app",
                columns: table => new
                {
                    SolfacId = table.Column<int>(nullable: false),
                    CertificateId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolfacCertificates", x => new { x.SolfacId, x.CertificateId });
                    table.ForeignKey(
                        name: "FK_SolfacCertificates_Certificates_CertificateId",
                        column: x => x.CertificateId,
                        principalSchema: "app",
                        principalTable: "Certificates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolfacCertificates_Solfacs_SolfacId",
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
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    SolfacStatusFrom = table.Column<int>(nullable: false),
                    SolfacStatusTo = table.Column<int>(nullable: false),
                    SolfacId = table.Column<int>(nullable: false)
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicantFiles",
                schema: "app",
                columns: table => new
                {
                    FileId = table.Column<int>(nullable: false),
                    ApplicantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantFiles", x => new { x.ApplicantId, x.FileId });
                    table.ForeignKey(
                        name: "FK_ApplicantFiles_Applicants_ApplicantId",
                        column: x => x.ApplicantId,
                        principalSchema: "app",
                        principalTable: "Applicants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicantFiles_Files_FileId",
                        column: x => x.FileId,
                        principalSchema: "app",
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicantHistories",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UserName = table.Column<string>(maxLength: 50, nullable: true),
                    StatusFromId = table.Column<int>(nullable: false),
                    StatusToId = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(maxLength: 1000, nullable: true),
                    ReasonCauseId = table.Column<int>(nullable: false),
                    ApplicantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicantHistories_Applicants_ApplicantId",
                        column: x => x.ApplicantId,
                        principalSchema: "app",
                        principalTable: "Applicants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicantHistories_ReasonCauses_ReasonCauseId",
                        column: x => x.ReasonCauseId,
                        principalSchema: "app",
                        principalTable: "ReasonCauses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicantProfiles",
                schema: "app",
                columns: table => new
                {
                    ApplicantId = table.Column<int>(nullable: false),
                    ProfileId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantProfiles", x => new { x.ApplicantId, x.ProfileId });
                    table.ForeignKey(
                        name: "FK_ApplicantProfiles_Applicants_ApplicantId",
                        column: x => x.ApplicantId,
                        principalSchema: "app",
                        principalTable: "Applicants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicantProfiles_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "app",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicantSkillses",
                schema: "app",
                columns: table => new
                {
                    ApplicantId = table.Column<int>(nullable: false),
                    SkillId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantSkillses", x => new { x.ApplicantId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_ApplicantSkillses_Applicants_ApplicantId",
                        column: x => x.ApplicantId,
                        principalSchema: "app",
                        principalTable: "Applicants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicantSkillses_Skills_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "app",
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobSearchApplicants",
                schema: "app",
                columns: table => new
                {
                    JobSearchId = table.Column<int>(nullable: false),
                    ApplicantId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ReasonId = table.Column<int>(nullable: false),
                    Comments = table.Column<string>(maxLength: 3000, nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 50, nullable: true),
                    CreatedByUserId = table.Column<int>(nullable: true),
                    HasRrhhInterview = table.Column<bool>(nullable: false),
                    RrhhInterviewDate = table.Column<DateTime>(nullable: true),
                    RrhhInterviewPlace = table.Column<string>(maxLength: 100, nullable: true),
                    RrhhInterviewerId = table.Column<int>(nullable: true),
                    HasTechnicalInterview = table.Column<bool>(nullable: false),
                    TechnicalInterviewDate = table.Column<DateTime>(nullable: true),
                    TechnicalInterviewPlace = table.Column<string>(maxLength: 100, nullable: true),
                    HasClientInterview = table.Column<bool>(nullable: false),
                    ClientInterviewDate = table.Column<DateTime>(nullable: true),
                    ClientInterviewPlace = table.Column<string>(maxLength: 100, nullable: true),
                    TechnicalInterviewComments = table.Column<string>(maxLength: 1000, nullable: true),
                    TechnicalExternalInterviewer = table.Column<string>(nullable: true),
                    ClientInterviewComments = table.Column<string>(maxLength: 1000, nullable: true),
                    ClientExternalInterviewer = table.Column<string>(nullable: true),
                    RrhhInterviewComments = table.Column<string>(maxLength: 1000, nullable: true),
                    RemoteWork = table.Column<bool>(nullable: false),
                    Salary = table.Column<decimal>(nullable: true),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    HasPhoneInterview = table.Column<bool>(nullable: false),
                    PhoneInterviewComments = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSearchApplicants", x => new { x.JobSearchId, x.ApplicantId, x.CreatedDate, x.ReasonId });
                    table.ForeignKey(
                        name: "FK_JobSearchApplicants_Applicants_ApplicantId",
                        column: x => x.ApplicantId,
                        principalSchema: "app",
                        principalTable: "Applicants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSearchApplicants_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSearchApplicants_JobSearchs_JobSearchId",
                        column: x => x.JobSearchId,
                        principalSchema: "app",
                        principalTable: "JobSearchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSearchApplicants_ReasonCauses_ReasonId",
                        column: x => x.ReasonId,
                        principalSchema: "app",
                        principalTable: "ReasonCauses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSearchApplicants_Users_RrhhInterviewerId",
                        column: x => x.RrhhInterviewerId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Budgets",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    Value = table.Column<decimal>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 100, nullable: true),
                    ManagementReportId = table.Column<int>(nullable: false),
                    LastValue = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Budgets_ManagementReports_ManagementReportId",
                        column: x => x.ManagementReportId,
                        principalSchema: "app",
                        principalTable: "ManagementReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CostDetails",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ManagementReportId = table.Column<int>(nullable: false),
                    MonthYear = table.Column<DateTime>(nullable: false),
                    TotalProvisioned = table.Column<decimal>(nullable: true),
                    TotalBilling = table.Column<decimal>(nullable: true),
                    Provision = table.Column<decimal>(nullable: true),
                    Closed = table.Column<bool>(nullable: false),
                    HasReal = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostDetails_ManagementReports_ManagementReportId",
                        column: x => x.ManagementReportId,
                        principalSchema: "app",
                        principalTable: "ManagementReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManagementReportBillings",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ManagementReportId = table.Column<int>(nullable: false),
                    MonthYear = table.Column<DateTime>(nullable: false),
                    EvalPropBillingValue = table.Column<decimal>(nullable: false),
                    EvalPropExpenseValue = table.Column<decimal>(nullable: false),
                    Comments = table.Column<string>(maxLength: 2000, nullable: true),
                    BilledResources = table.Column<int>(nullable: false),
                    EvalPropDifference = table.Column<decimal>(nullable: false),
                    Closed = table.Column<bool>(nullable: false),
                    BilledResourceTotal = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagementReportBillings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagementReportBillings_ManagementReports_ManagementReportId",
                        column: x => x.ManagementReportId,
                        principalSchema: "app",
                        principalTable: "ManagementReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManagementReportComments",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Comment = table.Column<string>(maxLength: 2000, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    ManagementReportId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagementReportComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagementReportComments_ManagementReports_ManagementReportId",
                        column: x => x.ManagementReportId,
                        principalSchema: "app",
                        principalTable: "ManagementReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdvancementHistories",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UserName = table.Column<string>(maxLength: 100, nullable: true),
                    StatusFromId = table.Column<int>(nullable: false),
                    StatusToId = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(maxLength: 1000, nullable: true),
                    AdvancementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvancementHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdvancementHistories_Advancements_AdvancementId",
                        column: x => x.AdvancementId,
                        principalSchema: "app",
                        principalTable: "Advancements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdvancementHistories_WorkflowStates_StatusFromId",
                        column: x => x.StatusFromId,
                        principalSchema: "app",
                        principalTable: "WorkflowStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdvancementHistories_WorkflowStates_StatusToId",
                        column: x => x.StatusToId,
                        principalSchema: "app",
                        principalTable: "WorkflowStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdvancementRefunds",
                schema: "app",
                columns: table => new
                {
                    AdvancementId = table.Column<int>(nullable: false),
                    RefundId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvancementRefunds", x => new { x.AdvancementId, x.RefundId });
                    table.ForeignKey(
                        name: "FK_AdvancementRefunds_Advancements_AdvancementId",
                        column: x => x.AdvancementId,
                        principalSchema: "app",
                        principalTable: "Advancements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdvancementRefunds_Refunds_RefundId",
                        column: x => x.RefundId,
                        principalSchema: "app",
                        principalTable: "Refunds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefundDetails",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 300, nullable: true),
                    Ammount = table.Column<decimal>(nullable: false),
                    RefundId = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    CostTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefundDetails_CostTypes_CostTypeId",
                        column: x => x.CostTypeId,
                        principalSchema: "app",
                        principalTable: "CostTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefundDetails_Refunds_RefundId",
                        column: x => x.RefundId,
                        principalSchema: "app",
                        principalTable: "Refunds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefundFiles",
                schema: "app",
                columns: table => new
                {
                    FileId = table.Column<int>(nullable: false),
                    RefundId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundFiles", x => new { x.FileId, x.RefundId });
                    table.ForeignKey(
                        name: "FK_RefundFiles_Files_FileId",
                        column: x => x.FileId,
                        principalSchema: "app",
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RefundFiles_Refunds_RefundId",
                        column: x => x.RefundId,
                        principalSchema: "app",
                        principalTable: "Refunds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefundHistories",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UserName = table.Column<string>(maxLength: 100, nullable: true),
                    StatusFromId = table.Column<int>(nullable: false),
                    StatusToId = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(maxLength: 1000, nullable: true),
                    RefundId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefundHistories_Refunds_RefundId",
                        column: x => x.RefundId,
                        principalSchema: "app",
                        principalTable: "Refunds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RefundHistories_WorkflowStates_StatusFromId",
                        column: x => x.StatusFromId,
                        principalSchema: "app",
                        principalTable: "WorkflowStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefundHistories_WorkflowStates_StatusToId",
                        column: x => x.StatusToId,
                        principalSchema: "app",
                        principalTable: "WorkflowStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowStateAccesses",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WorkflowStateTransitionId = table.Column<int>(nullable: false),
                    UserSourceId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedById = table.Column<int>(nullable: false),
                    AccessDenied = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowStateAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowStateAccesses_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowStateAccesses_Users_ModifiedById",
                        column: x => x.ModifiedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowStateAccesses_UserSources_UserSourceId",
                        column: x => x.UserSourceId,
                        principalSchema: "app",
                        principalTable: "UserSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowStateAccesses_WorkflowStateTransitions_WorkflowStateTransitionId",
                        column: x => x.WorkflowStateTransitionId,
                        principalSchema: "app",
                        principalTable: "WorkflowStateTransitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowStateNotifiers",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WorkflowStateTransitionId = table.Column<int>(nullable: false),
                    UserSourceId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowStateNotifiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowStateNotifiers_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowStateNotifiers_Users_ModifiedById",
                        column: x => x.ModifiedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowStateNotifiers_UserSources_UserSourceId",
                        column: x => x.UserSourceId,
                        principalSchema: "app",
                        principalTable: "UserSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowStateNotifiers_WorkflowStateTransitions_WorkflowStateTransitionId",
                        column: x => x.WorkflowStateTransitionId,
                        principalSchema: "app",
                        principalTable: "WorkflowStateTransitions",
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
                    Quantity = table.Column<decimal>(nullable: false),
                    UnitPrice = table.Column<decimal>(nullable: false),
                    Total = table.Column<decimal>(nullable: false),
                    HitoId = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: true)
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
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    StatusFrom = table.Column<int>(nullable: false),
                    StatusTo = table.Column<int>(nullable: false),
                    InvoiceId = table.Column<int>(nullable: false)
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractedDetails",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CostDetailId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Insurance = table.Column<decimal>(nullable: false),
                    Honorary = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractedDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractedDetails_CostDetails_CostDetailId",
                        column: x => x.CostDetailId,
                        principalSchema: "app",
                        principalTable: "CostDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CostDetailOthers",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CostDetailId = table.Column<int>(nullable: false),
                    CostDetailSubcategoryId = table.Column<int>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    IsReal = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostDetailOthers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostDetailOthers_CostDetails_CostDetailId",
                        column: x => x.CostDetailId,
                        principalSchema: "app",
                        principalTable: "CostDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CostDetailOthers_CostDetailSubcategories_CostDetailSubcategoryId",
                        column: x => x.CostDetailSubcategoryId,
                        principalSchema: "app",
                        principalTable: "CostDetailSubcategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CostDetailOthers_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "app",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CostDetailProfiles",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Guid = table.Column<string>(nullable: true),
                    CostDetailId = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    EmployeeProfileId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostDetailProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostDetailProfiles_CostDetails_CostDetailId",
                        column: x => x.CostDetailId,
                        principalSchema: "app",
                        principalTable: "CostDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CostDetailProfiles_EmployeeProfile_EmployeeProfileId",
                        column: x => x.EmployeeProfileId,
                        principalSchema: "app",
                        principalTable: "EmployeeProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CostDetailResources",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CostDetailId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    Adjustment = table.Column<decimal>(nullable: true),
                    Charges = table.Column<string>(nullable: true),
                    BudgetTypeId = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: true),
                    Comments = table.Column<string>(nullable: true),
                    PercentageModified = table.Column<decimal>(nullable: true),
                    Bono = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostDetailResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostDetailResources_BudgetType_BudgetTypeId",
                        column: x => x.BudgetTypeId,
                        principalSchema: "app",
                        principalTable: "BudgetType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CostDetailResources_Users_CostDetailId",
                        column: x => x.CostDetailId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CostDetailResources_Employees_CostDetailId",
                        column: x => x.CostDetailId,
                        principalSchema: "app",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CostDetailResources_CostDetails_CostDetailId",
                        column: x => x.CostDetailId,
                        principalSchema: "app",
                        principalTable: "CostDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CostDetailStaff",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Value = table.Column<decimal>(nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    CostDetailId = table.Column<int>(nullable: false),
                    CostDetailSubcategoryId = table.Column<int>(nullable: false),
                    BudgetTypeId = table.Column<int>(nullable: false),
                    OriginalValue = table.Column<decimal>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostDetailStaff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostDetailStaff_BudgetType_BudgetTypeId",
                        column: x => x.BudgetTypeId,
                        principalSchema: "app",
                        principalTable: "BudgetType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CostDetailStaff_CostDetails_CostDetailId",
                        column: x => x.CostDetailId,
                        principalSchema: "app",
                        principalTable: "CostDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CostDetailStaff_CostDetailSubcategories_CostDetailSubcategoryId",
                        column: x => x.CostDetailSubcategoryId,
                        principalSchema: "app",
                        principalTable: "CostDetailSubcategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CostDetailStaff_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "app",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceBillings",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Profile = table.Column<string>(nullable: true),
                    SeniorityId = table.Column<int>(nullable: true),
                    EmployeeId = table.Column<int>(nullable: true),
                    MonthHour = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    SubTotal = table.Column<decimal>(nullable: false),
                    HitoCrmId = table.Column<string>(nullable: true),
                    ManagementReportBillingId = table.Column<int>(nullable: false),
                    ProfileId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceBillings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceBillings_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "app",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResourceBillings_ManagementReportBillings_ManagementReportBillingId",
                        column: x => x.ManagementReportBillingId,
                        principalSchema: "app",
                        principalTable: "ManagementReportBillings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceBillings_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "app",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResourceBillings_Seniorities_SeniorityId",
                        column: x => x.SeniorityId,
                        principalSchema: "app",
                        principalTable: "Seniorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdvancementHistories_AdvancementId",
                schema: "app",
                table: "AdvancementHistories",
                column: "AdvancementId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvancementHistories_StatusFromId",
                schema: "app",
                table: "AdvancementHistories",
                column: "StatusFromId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvancementHistories_StatusToId",
                schema: "app",
                table: "AdvancementHistories",
                column: "StatusToId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvancementRefunds_RefundId",
                schema: "app",
                table: "AdvancementRefunds",
                column: "RefundId");

            migrationBuilder.CreateIndex(
                name: "IX_Advancements_CurrencyId",
                schema: "app",
                table: "Advancements",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Advancements_MonthsReturnId",
                schema: "app",
                table: "Advancements",
                column: "MonthsReturnId");

            migrationBuilder.CreateIndex(
                name: "IX_Advancements_StatusId",
                schema: "app",
                table: "Advancements",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Advancements_UserApplicantId",
                schema: "app",
                table: "Advancements",
                column: "UserApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_Advancements_UserId",
                schema: "app",
                table: "Advancements",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Advancements_WorkflowId",
                schema: "app",
                table: "Advancements",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvancementSalaryDiscounts_EmployeeId",
                schema: "app",
                table: "AdvancementSalaryDiscounts",
                column: "EmployeeId");

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
                name: "IX_Analytics_CommercialManagerId",
                schema: "app",
                table: "Analytics",
                column: "CommercialManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_CostCenterId",
                schema: "app",
                table: "Analytics",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_ManagerId",
                schema: "app",
                table: "Analytics",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_SectorId",
                schema: "app",
                table: "Analytics",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_ServiceTypeId",
                schema: "app",
                table: "Analytics",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_SoftwareLawId",
                schema: "app",
                table: "Analytics",
                column: "SoftwareLawId");

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
                name: "IX_ApplicantFiles_FileId",
                schema: "app",
                table: "ApplicantFiles",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantHistories_ApplicantId",
                schema: "app",
                table: "ApplicantHistories",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantHistories_ReasonCauseId",
                schema: "app",
                table: "ApplicantHistories",
                column: "ReasonCauseId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantProfiles_ProfileId",
                schema: "app",
                table: "ApplicantProfiles",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_AnalyticId",
                schema: "app",
                table: "Applicants",
                column: "AnalyticId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_CustomerId",
                schema: "app",
                table: "Applicants",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_ManagerId",
                schema: "app",
                table: "Applicants",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_ProfileId",
                schema: "app",
                table: "Applicants",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_ProjectId",
                schema: "app",
                table: "Applicants",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_ReasonCauseId",
                schema: "app",
                table: "Applicants",
                column: "ReasonCauseId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_RecommendedByUserId",
                schema: "app",
                table: "Applicants",
                column: "RecommendedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_SeniorityId",
                schema: "app",
                table: "Applicants",
                column: "SeniorityId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_SkillId",
                schema: "app",
                table: "Applicants",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantSkillses_SkillId",
                schema: "app",
                table: "ApplicantSkillses",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_Areas_ResponsableUserId",
                schema: "app",
                table: "Areas",
                column: "ResponsableUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_ManagementReportId",
                schema: "app",
                table: "Budgets",
                column: "ManagementReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_FileId",
                schema: "app",
                table: "Certificates",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractedDetails_CostDetailId",
                schema: "app",
                table: "ContractedDetails",
                column: "CostDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailOthers_CostDetailId",
                schema: "app",
                table: "CostDetailOthers",
                column: "CostDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailOthers_CostDetailSubcategoryId",
                schema: "app",
                table: "CostDetailOthers",
                column: "CostDetailSubcategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailOthers_CurrencyId",
                schema: "app",
                table: "CostDetailOthers",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailProfiles_CostDetailId",
                schema: "app",
                table: "CostDetailProfiles",
                column: "CostDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailProfiles_EmployeeProfileId",
                schema: "app",
                table: "CostDetailProfiles",
                column: "EmployeeProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailResources_BudgetTypeId",
                schema: "app",
                table: "CostDetailResources",
                column: "BudgetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailResources_CostDetailId_EmployeeId_BudgetTypeId",
                schema: "app",
                table: "CostDetailResources",
                columns: new[] { "CostDetailId", "EmployeeId", "BudgetTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CostDetails_ManagementReportId",
                schema: "app",
                table: "CostDetails",
                column: "ManagementReportId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailStaff_BudgetTypeId",
                schema: "app",
                table: "CostDetailStaff",
                column: "BudgetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailStaff_CostDetailId",
                schema: "app",
                table: "CostDetailStaff",
                column: "CostDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailStaff_CostDetailSubcategoryId",
                schema: "app",
                table: "CostDetailStaff",
                column: "CostDetailSubcategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailStaff_CurrencyId",
                schema: "app",
                table: "CostDetailStaff",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailSubcategories_CostDetailCategoryId",
                schema: "app",
                table: "CostDetailSubcategories",
                column: "CostDetailCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchanges_CurrencyId",
                schema: "app",
                table: "CurrencyExchanges",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Delegations_GrantedUserId",
                schema: "app",
                table: "Delegations",
                column: "GrantedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Delegations_UserId",
                schema: "app",
                table: "Delegations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeCategories_EmployeeId",
                schema: "app",
                table: "EmployeeCategories",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeNumber",
                schema: "app",
                table: "Employees",
                column: "EmployeeNumber",
                unique: true,
                filter: "[EmployeeNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ManagerId",
                schema: "app",
                table: "Employees",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_TypeEndReasonId",
                schema: "app",
                table: "Employees",
                column: "TypeEndReasonId");

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
                name: "IX_HealthInsurances_Code",
                schema: "app",
                table: "HealthInsurances",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HitoDetails_HitoId",
                schema: "app",
                table: "HitoDetails",
                column: "HitoId");

            migrationBuilder.CreateIndex(
                name: "IX_Hitos_SolfacId",
                schema: "app",
                table: "Hitos",
                column: "SolfacId");

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
                name: "IX_Invoices_ExcelFileId",
                schema: "app",
                table: "Invoices",
                column: "ExcelFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PdfFileId",
                schema: "app",
                table: "Invoices",
                column: "PdfFileId");

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
                name: "IX_JobSearchApplicants_ApplicantId",
                schema: "app",
                table: "JobSearchApplicants",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchApplicants_CreatedByUserId",
                schema: "app",
                table: "JobSearchApplicants",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchApplicants_ReasonId",
                schema: "app",
                table: "JobSearchApplicants",
                column: "ReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchApplicants_RrhhInterviewerId",
                schema: "app",
                table: "JobSearchApplicants",
                column: "RrhhInterviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchHistories_JobSearchId",
                schema: "app",
                table: "JobSearchHistories",
                column: "JobSearchId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchHistories_ReasonCauseId",
                schema: "app",
                table: "JobSearchHistories",
                column: "ReasonCauseId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchProfiles_ProfileId",
                schema: "app",
                table: "JobSearchProfiles",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchs_ClientId",
                schema: "app",
                table: "JobSearchs",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchs_ReasonCauseId",
                schema: "app",
                table: "JobSearchs",
                column: "ReasonCauseId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchs_RecruiterId",
                schema: "app",
                table: "JobSearchs",
                column: "RecruiterId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchs_ResourceAssignmentId",
                schema: "app",
                table: "JobSearchs",
                column: "ResourceAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchs_TimeHiringId",
                schema: "app",
                table: "JobSearchs",
                column: "TimeHiringId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchs_UserId",
                schema: "app",
                table: "JobSearchs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchSeniorities_SeniorityId",
                schema: "app",
                table: "JobSearchSeniorities",
                column: "SeniorityId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchSkillNotRequired_SkillId",
                schema: "app",
                table: "JobSearchSkillNotRequired",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchSkills_SkillId",
                schema: "app",
                table: "JobSearchSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseFiles_FileId",
                schema: "app",
                table: "LicenseFiles",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseHistories_LicenseId",
                schema: "app",
                table: "LicenseHistories",
                column: "LicenseId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseHistories_UserId",
                schema: "app",
                table: "LicenseHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_EmployeeId",
                schema: "app",
                table: "Licenses",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_ManagerId",
                schema: "app",
                table: "Licenses",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_SectorId",
                schema: "app",
                table: "Licenses",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_TypeId",
                schema: "app",
                table: "Licenses",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagementReportBillings_ManagementReportId",
                schema: "app",
                table: "ManagementReportBillings",
                column: "ManagementReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagementReportComments_ManagementReportId",
                schema: "app",
                table: "ManagementReportComments",
                column: "ManagementReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagementReports_AnalyticId",
                schema: "app",
                table: "ManagementReports",
                column: "AnalyticId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ManagementReports_StateId",
                schema: "app",
                table: "ManagementReports",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_PrepaidImportedData_PrepaidId",
                schema: "app",
                table: "PrepaidImportedData",
                column: "PrepaidId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderAmmountDetails_CurrencyId",
                schema: "app",
                table: "PurchaseOrderAmmountDetails",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderAnalytics_AnalyticId",
                schema: "app",
                table: "PurchaseOrderAnalytics",
                column: "AnalyticId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderHistories_PurchaseOrderId",
                schema: "app",
                table: "PurchaseOrderHistories",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderHistories_UserId",
                schema: "app",
                table: "PurchaseOrderHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_AreaId",
                schema: "app",
                table: "PurchaseOrders",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_FileId",
                schema: "app",
                table: "PurchaseOrders",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundDetails_CostTypeId",
                schema: "app",
                table: "RefundDetails",
                column: "CostTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundDetails_RefundId",
                schema: "app",
                table: "RefundDetails",
                column: "RefundId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundFiles_RefundId",
                schema: "app",
                table: "RefundFiles",
                column: "RefundId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundHistories_RefundId",
                schema: "app",
                table: "RefundHistories",
                column: "RefundId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundHistories_StatusFromId",
                schema: "app",
                table: "RefundHistories",
                column: "StatusFromId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundHistories_StatusToId",
                schema: "app",
                table: "RefundHistories",
                column: "StatusToId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_AnalyticId",
                schema: "app",
                table: "Refunds",
                column: "AnalyticId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_CreditCardId",
                schema: "app",
                table: "Refunds",
                column: "CreditCardId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_CurrencyId",
                schema: "app",
                table: "Refunds",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_StatusId",
                schema: "app",
                table: "Refunds",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_UserApplicantId",
                schema: "app",
                table: "Refunds",
                column: "UserApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_UserId",
                schema: "app",
                table: "Refunds",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_WorkflowId",
                schema: "app",
                table: "Refunds",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceBillings_EmployeeId",
                schema: "app",
                table: "ResourceBillings",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceBillings_ManagementReportBillingId",
                schema: "app",
                table: "ResourceBillings",
                column: "ManagementReportBillingId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceBillings_ProfileId",
                schema: "app",
                table: "ResourceBillings",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceBillings_SeniorityId",
                schema: "app",
                table: "ResourceBillings",
                column: "SeniorityId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleFunctionality_FunctionalityId",
                schema: "app",
                table: "RoleFunctionality",
                column: "FunctionalityId");

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_ResponsableUserId",
                schema: "app",
                table: "Sectors",
                column: "ResponsableUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialChargeItems_SocialChargeId",
                schema: "app",
                table: "SocialChargeItems",
                column: "SocialChargeId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialCharges_EmployeeId",
                schema: "app",
                table: "SocialCharges",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_SolfacAttachments_SolfacId",
                schema: "app",
                table: "SolfacAttachments",
                column: "SolfacId");

            migrationBuilder.CreateIndex(
                name: "IX_SolfacCertificates_CertificateId",
                schema: "app",
                table: "SolfacCertificates",
                column: "CertificateId");

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
                name: "IX_Solfacs_PurchaseOrderId",
                schema: "app",
                table: "Solfacs",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Solfacs_UserApplicantId",
                schema: "app",
                table: "Solfacs",
                column: "UserApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CategoryId",
                schema: "app",
                table: "Tasks",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroup_GroupId",
                schema: "app",
                table: "UserGroup",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                schema: "app",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowReadAccesses_CreatedById",
                schema: "app",
                table: "WorkflowReadAccesses",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowReadAccesses_ModifiedById",
                schema: "app",
                table: "WorkflowReadAccesses",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowReadAccesses_UserSourceId",
                schema: "app",
                table: "WorkflowReadAccesses",
                column: "UserSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowReadAccesses_WorkflowId",
                schema: "app",
                table: "WorkflowReadAccesses",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_CreatedById",
                schema: "app",
                table: "Workflows",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_ModifiedById",
                schema: "app",
                table: "Workflows",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_WorkflowTypeId",
                schema: "app",
                table: "Workflows",
                column: "WorkflowTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateAccesses_CreatedById",
                schema: "app",
                table: "WorkflowStateAccesses",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateAccesses_ModifiedById",
                schema: "app",
                table: "WorkflowStateAccesses",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateAccesses_UserSourceId",
                schema: "app",
                table: "WorkflowStateAccesses",
                column: "UserSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateAccesses_WorkflowStateTransitionId",
                schema: "app",
                table: "WorkflowStateAccesses",
                column: "WorkflowStateTransitionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateNotifiers_CreatedById",
                schema: "app",
                table: "WorkflowStateNotifiers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateNotifiers_ModifiedById",
                schema: "app",
                table: "WorkflowStateNotifiers",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateNotifiers_UserSourceId",
                schema: "app",
                table: "WorkflowStateNotifiers",
                column: "UserSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateNotifiers_WorkflowStateTransitionId",
                schema: "app",
                table: "WorkflowStateNotifiers",
                column: "WorkflowStateTransitionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStates_CreatedById",
                schema: "app",
                table: "WorkflowStates",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStates_ModifiedById",
                schema: "app",
                table: "WorkflowStates",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateTransitions_ActualWorkflowStateId",
                schema: "app",
                table: "WorkflowStateTransitions",
                column: "ActualWorkflowStateId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateTransitions_CreatedById",
                schema: "app",
                table: "WorkflowStateTransitions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateTransitions_ModifiedById",
                schema: "app",
                table: "WorkflowStateTransitions",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateTransitions_NextWorkflowStateId",
                schema: "app",
                table: "WorkflowStateTransitions",
                column: "NextWorkflowStateId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateTransitions_WorkflowId",
                schema: "app",
                table: "WorkflowStateTransitions",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTypes_CreatedById",
                schema: "app",
                table: "WorkflowTypes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTypes_ModifiedById",
                schema: "app",
                table: "WorkflowTypes",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTimes_AnalyticId",
                schema: "app",
                table: "WorkTimes",
                column: "AnalyticId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTimes_EmployeeId",
                schema: "app",
                table: "WorkTimes",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTimes_TaskId",
                schema: "app",
                table: "WorkTimes",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTimes_UserId",
                schema: "app",
                table: "WorkTimes",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvancementHistories",
                schema: "app");

            migrationBuilder.DropTable(
                name: "AdvancementRefunds",
                schema: "app");

            migrationBuilder.DropTable(
                name: "AdvancementSalaryDiscounts",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Allocations",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ApplicantFiles",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ApplicantHistories",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ApplicantProfiles",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ApplicantSkillses",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Budgets",
                schema: "app");

            migrationBuilder.DropTable(
                name: "CloseDates",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Contacts",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ContractedDetails",
                schema: "app");

            migrationBuilder.DropTable(
                name: "CostDetailOthers",
                schema: "app");

            migrationBuilder.DropTable(
                name: "CostDetailProfiles",
                schema: "app");

            migrationBuilder.DropTable(
                name: "CostDetailResources",
                schema: "app");

            migrationBuilder.DropTable(
                name: "CostDetailStaff",
                schema: "app");

            migrationBuilder.DropTable(
                name: "CurrencyExchanges",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Delegations",
                schema: "app");

            migrationBuilder.DropTable(
                name: "EmployeeCategories",
                schema: "app");

            migrationBuilder.DropTable(
                name: "EmployeeEndNotifications",
                schema: "app");

            migrationBuilder.DropTable(
                name: "EmployeeHistory",
                schema: "app");

            migrationBuilder.DropTable(
                name: "EmployeeLicenses",
                schema: "app");

            migrationBuilder.DropTable(
                name: "EmployeeProfileHistory",
                schema: "app");

            migrationBuilder.DropTable(
                name: "EmployeeSyncActions",
                schema: "app");

            migrationBuilder.DropTable(
                name: "HealthInsurances",
                schema: "app");

            migrationBuilder.DropTable(
                name: "HitoDetails",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Holidays",
                schema: "app");

            migrationBuilder.DropTable(
                name: "InvoiceHistories",
                schema: "app");

            migrationBuilder.DropTable(
                name: "JobSearchApplicants",
                schema: "app");

            migrationBuilder.DropTable(
                name: "JobSearchHistories",
                schema: "app");

            migrationBuilder.DropTable(
                name: "JobSearchProfiles",
                schema: "app");

            migrationBuilder.DropTable(
                name: "JobSearchSeniorities",
                schema: "app");

            migrationBuilder.DropTable(
                name: "JobSearchSkillNotRequired",
                schema: "app");

            migrationBuilder.DropTable(
                name: "JobSearchSkills",
                schema: "app");

            migrationBuilder.DropTable(
                name: "LicenseFiles",
                schema: "app");

            migrationBuilder.DropTable(
                name: "LicenseHistories",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Logs",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ManagementReportComments",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Opportunities",
                schema: "app");

            migrationBuilder.DropTable(
                name: "PaymentTerms",
                schema: "app");

            migrationBuilder.DropTable(
                name: "PrepaidHealths",
                schema: "app");

            migrationBuilder.DropTable(
                name: "PrepaidImportedData",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Provinces",
                schema: "app");

            migrationBuilder.DropTable(
                name: "PurchaseOrderAmmountDetails",
                schema: "app");

            migrationBuilder.DropTable(
                name: "PurchaseOrderAnalytics",
                schema: "app");

            migrationBuilder.DropTable(
                name: "PurchaseOrderHistories",
                schema: "app");

            migrationBuilder.DropTable(
                name: "PurchaseOrderOptions",
                schema: "app");

            migrationBuilder.DropTable(
                name: "RefundDetails",
                schema: "app");

            migrationBuilder.DropTable(
                name: "RefundFiles",
                schema: "app");

            migrationBuilder.DropTable(
                name: "RefundHistories",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ReportsPowerBi",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ResourceBillings",
                schema: "app");

            migrationBuilder.DropTable(
                name: "RoleFunctionality",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Services",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Settings",
                schema: "app");

            migrationBuilder.DropTable(
                name: "SocialChargeItems",
                schema: "app");

            migrationBuilder.DropTable(
                name: "SolfacAttachments",
                schema: "app");

            migrationBuilder.DropTable(
                name: "SolfacCertificates",
                schema: "app");

            migrationBuilder.DropTable(
                name: "SolfacHistories",
                schema: "app");

            migrationBuilder.DropTable(
                name: "UserGroup",
                schema: "app");

            migrationBuilder.DropTable(
                name: "WorkflowReadAccesses",
                schema: "app");

            migrationBuilder.DropTable(
                name: "WorkflowStateAccesses",
                schema: "app");

            migrationBuilder.DropTable(
                name: "WorkflowStateNotifiers",
                schema: "app");

            migrationBuilder.DropTable(
                name: "WorkTimes",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Advancements",
                schema: "app");

            migrationBuilder.DropTable(
                name: "EmployeeProfile",
                schema: "app");

            migrationBuilder.DropTable(
                name: "CostDetails",
                schema: "app");

            migrationBuilder.DropTable(
                name: "CostDetailSubcategories",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Hitos",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Invoices",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Applicants",
                schema: "app");

            migrationBuilder.DropTable(
                name: "JobSearchs",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Licenses",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Prepaids",
                schema: "app");

            migrationBuilder.DropTable(
                name: "CostTypes",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Refunds",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ManagementReportBillings",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Functionalities",
                schema: "app");

            migrationBuilder.DropTable(
                name: "SocialCharges",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Certificates",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Groups",
                schema: "app");

            migrationBuilder.DropTable(
                name: "UserSources",
                schema: "app");

            migrationBuilder.DropTable(
                name: "WorkflowStateTransitions",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Tasks",
                schema: "app");

            migrationBuilder.DropTable(
                name: "MonthsReturns",
                schema: "app");

            migrationBuilder.DropTable(
                name: "CostDetailCategories",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Solfacs",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Profiles",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Projects",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Seniorities",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Skills",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Customers",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ReasonCauses",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ResourceAssignments",
                schema: "app");

            migrationBuilder.DropTable(
                name: "TimeHirings",
                schema: "app");

            migrationBuilder.DropTable(
                name: "LicenseTypes",
                schema: "app");

            migrationBuilder.DropTable(
                name: "CreditCards",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ManagementReports",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Modules",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Employees",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "app");

            migrationBuilder.DropTable(
                name: "WorkflowStates",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Workflows",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Currencies",
                schema: "app");

            migrationBuilder.DropTable(
                name: "DocumentTypes",
                schema: "app");

            migrationBuilder.DropTable(
                name: "PurchaseOrders",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Analytics",
                schema: "app");

            migrationBuilder.DropTable(
                name: "BudgetType",
                schema: "app");

            migrationBuilder.DropTable(
                name: "EmployeeEndReason",
                schema: "app");

            migrationBuilder.DropTable(
                name: "WorkflowTypes",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Areas",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Files",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ImputationNumbers",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ClientGroups",
                schema: "app");

            migrationBuilder.DropTable(
                name: "CostCenters",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Sectors",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ServiceTypes",
                schema: "app");

            migrationBuilder.DropTable(
                name: "SoftwareLaws",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Solutions",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Technologies",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "app");
        }
    }
}
