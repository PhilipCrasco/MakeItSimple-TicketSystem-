﻿// <auto-generated />
using System;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    [DbContext(typeof(MisDbContext))]
    partial class MisDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("MakeItSimple.WebApi.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("id");

                    b.Property<Guid?>("AddedBy")
                        .HasColumnType("char(36)")
                        .HasColumnName("added_by");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<string>("Email")
                        .HasColumnType("longtext")
                        .HasColumnName("email");

                    b.Property<string>("EmpId")
                        .HasColumnType("longtext")
                        .HasColumnName("emp_id");

                    b.Property<string>("Fullname")
                        .HasColumnType("longtext")
                        .HasColumnName("fullname");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_active");

                    b.Property<bool?>("IsPasswordChange")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_password_change");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("char(36)")
                        .HasColumnName("modified_by");

                    b.Property<string>("Password")
                        .HasColumnType("longtext")
                        .HasColumnName("password");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.Property<int>("UserRoleId")
                        .HasColumnType("int")
                        .HasColumnName("user_role_id");

                    b.Property<string>("Username")
                        .HasColumnType("longtext")
                        .HasColumnName("username");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("AddedBy")
                        .HasDatabaseName("ix_users_added_by");

                    b.HasIndex("ModifiedBy")
                        .HasDatabaseName("ix_users_modified_by");

                    b.HasIndex("UserRoleId")
                        .HasDatabaseName("ix_users_user_role_id");

                    b.ToTable("users", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                            CreatedAt = new DateTime(2023, 12, 20, 9, 14, 33, 218, DateTimeKind.Local).AddTicks(281),
                            Email = "admin@gmail.com",
                            Fullname = "Admin",
                            IsActive = true,
                            Password = "$2a$11$.dmG8WMZlvtmtjEJV4b5z.J96Y9L4cnZjiIu1K7ADaEF2KGkqaJOS",
                            UserRoleId = 1,
                            Username = "admin"
                        });
                });

            modelBuilder.Entity("MakeItSimple.WebApi.Models.UserManagement.UserRoleAccount.UserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<Guid?>("AddedBy")
                        .HasColumnType("char(36)")
                        .HasColumnName("added_by");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_active");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("char(36)")
                        .HasColumnName("modified_by");

                    b.Property<string>("Permissions")
                        .HasColumnType("longtext")
                        .HasColumnName("permissions");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.Property<string>("UserRoleName")
                        .HasColumnType("longtext")
                        .HasColumnName("user_role_name");

                    b.HasKey("Id")
                        .HasName("pk_user_roles");

                    b.HasIndex("AddedBy")
                        .HasDatabaseName("ix_user_roles_added_by");

                    b.HasIndex("ModifiedBy")
                        .HasDatabaseName("ix_user_roles_modified_by");

                    b.ToTable("user_roles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedAt = new DateTime(2023, 12, 20, 9, 14, 33, 486, DateTimeKind.Local).AddTicks(205),
                            IsActive = true,
                            Permissions = "[\"User Management\"]",
                            UserRoleName = "Admin"
                        });
                });

            modelBuilder.Entity("MakeItSimple.WebApi.Models.User", b =>
                {
                    b.HasOne("MakeItSimple.WebApi.Models.User", "AddedByUser")
                        .WithMany()
                        .HasForeignKey("AddedBy")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("fk_users_users_added_by");

                    b.HasOne("MakeItSimple.WebApi.Models.User", "ModifiedByUser")
                        .WithMany()
                        .HasForeignKey("ModifiedBy")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("fk_users_users_modified_by");

                    b.HasOne("MakeItSimple.WebApi.Models.UserManagement.UserRoleAccount.UserRole", "UserRole")
                        .WithMany("Users")
                        .HasForeignKey("UserRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_users_user_roles_user_role_id");

                    b.Navigation("AddedByUser");

                    b.Navigation("ModifiedByUser");

                    b.Navigation("UserRole");
                });

            modelBuilder.Entity("MakeItSimple.WebApi.Models.UserManagement.UserRoleAccount.UserRole", b =>
                {
                    b.HasOne("MakeItSimple.WebApi.Models.User", "AddedByUser")
                        .WithMany()
                        .HasForeignKey("AddedBy")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("fk_user_roles_users_added_by_user_id");

                    b.HasOne("MakeItSimple.WebApi.Models.User", "ModifiedByUser")
                        .WithMany()
                        .HasForeignKey("ModifiedBy")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("fk_user_roles_users_modified_by_user_id");

                    b.Navigation("AddedByUser");

                    b.Navigation("ModifiedByUser");
                });

            modelBuilder.Entity("MakeItSimple.WebApi.Models.UserManagement.UserRoleAccount.UserRole", b =>
                {
                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
