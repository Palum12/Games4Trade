﻿// <auto-generated />
using System;
using Games4TradeAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Games4TradeAPI.Data.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20180909111021_announcements")]
    partial class announcements
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.2-rtm-30932")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Games4Trade.Models.Announcement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("Now()");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Announcements");
                });

            modelBuilder.Entity("Games4Trade.Models.ObservedUsersRelationShip", b =>
                {
                    b.Property<int>("ObservingUserId");

                    b.Property<int>("ObservedUserId");

                    b.HasKey("ObservingUserId", "ObservedUserId");

                    b.HasIndex("ObservedUserId");

                    b.ToTable("ObservedUsersRelationship");
                });

            modelBuilder.Entity("Games4Trade.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(512);

                    b.Property<string>("RecoveryAddress")
                        .HasMaxLength(32);

                    b.Property<string>("Role")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("'User'")
                        .HasMaxLength(8);

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Login")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Games4Trade.Models.Announcement", b =>
                {
                    b.HasOne("Games4Trade.Models.User", "User")
                        .WithMany("Announcements")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Games4Trade.Models.ObservedUsersRelationShip", b =>
                {
                    b.HasOne("Games4Trade.Models.User", "ObservedUser")
                        .WithMany("ObservedUsers")
                        .HasForeignKey("ObservedUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Games4Trade.Models.User", "ObservingUser")
                        .WithMany("ObservingUsers")
                        .HasForeignKey("ObservingUserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
