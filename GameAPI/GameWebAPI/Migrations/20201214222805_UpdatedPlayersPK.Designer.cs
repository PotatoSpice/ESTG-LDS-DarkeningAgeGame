﻿// <auto-generated />
using System;
using GameWebAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GameWebAPI.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20201214222805_UpdatedPlayersPK")]
    partial class UpdatedPlayersPK
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("GameWebAPI.Entities.Player", b =>
                {
                    b.Property<string>("username")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("birthDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("email")
                        .HasColumnType("TEXT");

                    b.Property<string>("firstName")
                        .HasColumnType("TEXT");

                    b.Property<string>("lastName")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("passwordHash")
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("passwordSalt")
                        .HasColumnType("BLOB");

                    b.HasKey("username");

                    b.HasIndex("email")
                        .IsUnique();

                    b.ToTable("Players");
                });
#pragma warning restore 612, 618
        }
    }
}
