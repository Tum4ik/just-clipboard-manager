﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tum4ik.JustClipboardManager.Data;

#nullable disable

namespace Tum4ik.JustClipboardManager.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230621143125_ReplacedClipTypeByPluginId")]
    partial class ReplacedClipTypeByPluginId
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true);

            modelBuilder.Entity("Tum4ik.JustClipboardManager.Data.Models.Clip", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("ClippedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("datetime('now', 'localtime')");

                    b.Property<Guid>("PluginId")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("RepresentationData")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<string>("SearchLabel")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("SearchLabel");

                    b.ToTable("Clips");
                });

            modelBuilder.Entity("Tum4ik.JustClipboardManager.Data.Models.FormattedDataObject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ClipId")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Data")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<string>("DataDotnetType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Format")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("FormatOrder")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ClipId");

                    b.ToTable("FormattedDataObjects");
                });

            modelBuilder.Entity("Tum4ik.JustClipboardManager.Data.Models.FormattedDataObject", b =>
                {
                    b.HasOne("Tum4ik.JustClipboardManager.Data.Models.Clip", "Clip")
                        .WithMany("FormattedDataObjects")
                        .HasForeignKey("ClipId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Clip");
                });

            modelBuilder.Entity("Tum4ik.JustClipboardManager.Data.Models.Clip", b =>
                {
                    b.Navigation("FormattedDataObjects");
                });
#pragma warning restore 612, 618
        }
    }
}
