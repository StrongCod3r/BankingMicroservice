﻿// <auto-generated />
using System;
using Banking.Account.Query.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Banking.Account.Query.Infrastructure.Migrations
{
    [DbContext(typeof(MySqlDbContext))]
    [Migration("20240815090637_MigrationSqlEventSourcing")]
    partial class MigrationSqlEventSourcing
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Banking.Account.Query.Domain.BankAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AccountHolder")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("AccountType")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<double>("Balance")
                        .HasColumnType("double");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Identifier")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("BankAccounts");
                });
#pragma warning restore 612, 618
        }
    }
}
