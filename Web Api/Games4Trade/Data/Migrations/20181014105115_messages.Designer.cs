﻿// <auto-generated />
using System;
using Games4Trade.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Games4Trade.Data.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20181014105115_messages")]
    partial class messages
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.3-rtm-32065")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Games4Trade.Models.Advertisement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("Now()");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool?>("ExchangeActive")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(true);

                    b.Property<bool?>("IsActive")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(true);

                    b.Property<decimal>("Price")
                        .HasColumnType("money");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Advertisements");
                });

            modelBuilder.Entity("Games4Trade.Models.AdvertisementItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("AdvertisementId");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<int>("StateId");

                    b.Property<int>("SystemId");

                    b.HasKey("Id");

                    b.HasIndex("AdvertisementId")
                        .IsUnique();

                    b.HasIndex("StateId");

                    b.HasIndex("SystemId");

                    b.ToTable("AdvertisementItems");

                    b.HasDiscriminator<string>("Discriminator").HasValue("AdvertisementItem");
                });

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

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<int?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Announcements");
                });

            modelBuilder.Entity("Games4Trade.Models.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("Value")
                        .IsUnique();

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("Games4Trade.Models.Message", b =>
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

                    b.Property<int>("ReciverId");

                    b.Property<int>("SenderId");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("ReciverId");

                    b.HasIndex("SenderId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("Games4Trade.Models.ObservedUsersRelationship", b =>
                {
                    b.Property<int>("ObservingUserId");

                    b.Property<int>("ObservedUserId");

                    b.HasKey("ObservingUserId", "ObservedUserId");

                    b.HasIndex("ObservedUserId");

                    b.ToTable("ObservedUsersRelationship");
                });

            modelBuilder.Entity("Games4Trade.Models.Photo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("AdvertisementId");

                    b.Property<DateTime>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("Now()");

                    b.Property<string>("Path")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("AdvertisementId");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("Games4Trade.Models.Region", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.HasKey("Id");

                    b.HasIndex("Value")
                        .IsUnique();

                    b.ToTable("Regions");

                    b.HasData(
                        new { Id = 1, Value = "PAL" },
                        new { Id = 2, Value = "NTSC" },
                        new { Id = 3, Value = "INNY" },
                        new { Id = 4, Value = "MULTI" }
                    );
                });

            modelBuilder.Entity("Games4Trade.Models.State", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.HasKey("Id");

                    b.HasIndex("Value")
                        .IsUnique();

                    b.ToTable("States");

                    b.HasData(
                        new { Id = 1, Value = "Nowy" },
                        new { Id = 2, Value = "Używany" },
                        new { Id = 3, Value = "Uszkodzony" }
                    );
                });

            modelBuilder.Entity("Games4Trade.Models.System", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Manufacturer")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("Manufacturer", "Model");

                    b.ToTable("Systems");
                });

            modelBuilder.Entity("Games4Trade.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(512);

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(11);

                    b.Property<int?>("PhotoId");

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

                    b.HasIndex("PhotoId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Games4Trade.Models.UserLikedGenre", b =>
                {
                    b.Property<int>("GenreId");

                    b.Property<int>("UserId");

                    b.HasKey("GenreId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserGenreRelationship");
                });

            modelBuilder.Entity("Games4Trade.Models.UserOwnedSystem", b =>
                {
                    b.Property<int>("SystemId");

                    b.Property<int>("UserId");

                    b.HasKey("SystemId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserSystemRelationship");
                });

            modelBuilder.Entity("Games4Trade.Models.Accessory", b =>
                {
                    b.HasBaseType("Games4Trade.Models.AdvertisementItem");

                    b.Property<string>("AccessoryManufacturer");

                    b.Property<string>("AccessoryModel");

                    b.ToTable("Accessory");

                    b.HasDiscriminator().HasValue("Accessory");
                });

            modelBuilder.Entity("Games4Trade.Models.Console", b =>
                {
                    b.HasBaseType("Games4Trade.Models.AdvertisementItem");

                    b.Property<int>("ConsoleRegionId");

                    b.Property<DateTime>("DateManufactured");

                    b.HasIndex("ConsoleRegionId");

                    b.ToTable("Console");

                    b.HasDiscriminator().HasValue("Console");
                });

            modelBuilder.Entity("Games4Trade.Models.Game", b =>
                {
                    b.HasBaseType("Games4Trade.Models.AdvertisementItem");

                    b.Property<DateTime>("DateDeveloped");

                    b.Property<string>("Developer");

                    b.Property<int>("GameRegionId");

                    b.Property<int>("GenreId");

                    b.Property<string>("Title");

                    b.HasIndex("GameRegionId");

                    b.HasIndex("GenreId");

                    b.ToTable("Game");

                    b.HasDiscriminator().HasValue("Game");
                });

            modelBuilder.Entity("Games4Trade.Models.Advertisement", b =>
                {
                    b.HasOne("Games4Trade.Models.User", "User")
                        .WithMany("Advertisements")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Games4Trade.Models.AdvertisementItem", b =>
                {
                    b.HasOne("Games4Trade.Models.Advertisement", "Advertisement")
                        .WithOne("Item")
                        .HasForeignKey("Games4Trade.Models.AdvertisementItem", "AdvertisementId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Games4Trade.Models.State", "State")
                        .WithMany("AdvertisementItems")
                        .HasForeignKey("StateId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Games4Trade.Models.System", "System")
                        .WithMany("AdvertisementItems")
                        .HasForeignKey("SystemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Games4Trade.Models.Announcement", b =>
                {
                    b.HasOne("Games4Trade.Models.User", "User")
                        .WithMany("Announcements")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("Games4Trade.Models.Message", b =>
                {
                    b.HasOne("Games4Trade.Models.User", "Reciver")
                        .WithMany("MessagesRecived")
                        .HasForeignKey("ReciverId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Games4Trade.Models.User", "Sender")
                        .WithMany("MessagesSent")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Games4Trade.Models.ObservedUsersRelationship", b =>
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

            modelBuilder.Entity("Games4Trade.Models.Photo", b =>
                {
                    b.HasOne("Games4Trade.Models.Advertisement", "Advertisement")
                        .WithMany("Photos")
                        .HasForeignKey("AdvertisementId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Games4Trade.Models.User", b =>
                {
                    b.HasOne("Games4Trade.Models.Photo", "Photo")
                        .WithOne("User")
                        .HasForeignKey("Games4Trade.Models.User", "PhotoId");
                });

            modelBuilder.Entity("Games4Trade.Models.UserLikedGenre", b =>
                {
                    b.HasOne("Games4Trade.Models.Genre", "Genre")
                        .WithMany("LikedByUsers")
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Games4Trade.Models.User", "User")
                        .WithMany("LikedGenres")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Games4Trade.Models.UserOwnedSystem", b =>
                {
                    b.HasOne("Games4Trade.Models.System", "System")
                        .WithMany("OwnedByUsers")
                        .HasForeignKey("SystemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Games4Trade.Models.User", "User")
                        .WithMany("OwnedSystems")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Games4Trade.Models.Console", b =>
                {
                    b.HasOne("Games4Trade.Models.Region", "ConsoleRegion")
                        .WithMany("Consoles")
                        .HasForeignKey("ConsoleRegionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Games4Trade.Models.Game", b =>
                {
                    b.HasOne("Games4Trade.Models.Region", "GameRegion")
                        .WithMany("Games")
                        .HasForeignKey("GameRegionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Games4Trade.Models.Genre", "Genre")
                        .WithMany("Games")
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
