using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Sofco.DAL;

namespace Sofco.WebApi.Migrations
{
    [DbContext(typeof(SofcoContext))]
    partial class SofcoContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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

            modelBuilder.Entity("Sofco.Model.Models.Functionality", b =>
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

                    b.ToTable("Functionalities");
                });

            modelBuilder.Entity("Sofco.Model.Models.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<DateTime?>("EndDate");

                    b.Property<int?>("RoleId");

                    b.Property<DateTime>("StartDate");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("Sofco.Model.Models.Menu", b =>
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

            modelBuilder.Entity("Sofco.Model.Models.Module", b =>
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

            modelBuilder.Entity("Sofco.Model.Models.Role", b =>
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

            modelBuilder.Entity("Sofco.Model.Models.User", b =>
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

            modelBuilder.Entity("Sofco.Model.Relationships.ModuleFunctionality", b =>
                {
                    b.Property<int>("ModuleId");

                    b.Property<int>("FunctionalityId");

                    b.HasKey("ModuleId", "FunctionalityId");

                    b.HasIndex("FunctionalityId");

                    b.ToTable("ModuleFunctionality");
                });

            modelBuilder.Entity("Sofco.Model.Relationships.RoleModule", b =>
                {
                    b.Property<int>("RoleId");

                    b.Property<int>("ModuleId");

                    b.HasKey("RoleId", "ModuleId");

                    b.HasIndex("ModuleId");

                    b.ToTable("RoleModule");
                });

            modelBuilder.Entity("Sofco.Model.Relationships.UserGroup", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("GroupId");

                    b.HasKey("UserId", "GroupId");

                    b.HasIndex("GroupId");

                    b.ToTable("UserGroup");
                });

            modelBuilder.Entity("Sofco.Model.Models.Group", b =>
                {
                    b.HasOne("Sofco.Model.Models.Role", "Role")
                        .WithMany("Groups")
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Sofco.Model.Models.Module", b =>
                {
                    b.HasOne("Sofco.Model.Models.Menu", "Menu")
                        .WithMany("Modules")
                        .HasForeignKey("MenuId");
                });

            modelBuilder.Entity("Sofco.Model.Relationships.ModuleFunctionality", b =>
                {
                    b.HasOne("Sofco.Model.Models.Functionality", "Functionality")
                        .WithMany("ModuleFunctionality")
                        .HasForeignKey("FunctionalityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sofco.Model.Models.Module", "Module")
                        .WithMany("ModuleFunctionality")
                        .HasForeignKey("ModuleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sofco.Model.Relationships.RoleModule", b =>
                {
                    b.HasOne("Sofco.Model.Models.Module", "Module")
                        .WithMany("RoleModule")
                        .HasForeignKey("ModuleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sofco.Model.Models.Role", "Role")
                        .WithMany("RoleModule")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sofco.Model.Relationships.UserGroup", b =>
                {
                    b.HasOne("Sofco.Model.Models.Group", "Group")
                        .WithMany("UserGroups")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sofco.Model.Models.User", "User")
                        .WithMany("UserGroups")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
