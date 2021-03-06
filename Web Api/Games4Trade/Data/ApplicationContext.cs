﻿using System.Collections.Generic;
using Games4TradeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Games4TradeAPI.Data
{
    public class ApplicationContext : DbContext, IApplicationContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Announcement> Announcements { get; set; }
        public virtual DbSet<ObservedUsersRelationship> ObservedUsersRelationship { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<UserLikedGenre> UserGenreRelationship { get; set; }
        public virtual DbSet<Models.System> Systems { get; set; }
        public virtual DbSet<UserOwnedSystem> UserSystemRelationship { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<Photo> Photos { get; set; }
        public virtual DbSet<Advertisement> Advertisements { get; set; }
        public virtual DbSet<AdvertisementItem> AdvertisementItems { get; set; }
        public virtual DbSet<Console> Consoles { get; set; }
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<Accessory> Accessories { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public DbContextOptions<ApplicationContext> Options { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Options = options;
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ObservedUsersRelationship>(entity =>
            {
                entity.HasKey(uu => new {uu.ObservingUserId, uu.ObservedUserId});

                entity.HasOne(uu => uu.ObservingUser)
                    .WithMany(uu => uu.ObservingUsers)
                    .HasForeignKey(uu => uu.ObservingUserId);

                entity.HasOne(uu => uu.ObservedUser)
                    .WithMany(uu => uu.ObservedUsers)
                    .HasForeignKey(uu => uu.ObservedUserId);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Id).UseIdentityByDefaultColumn();
                entity.HasKey(u => u.Id);

                entity.HasIndex(u => u.Login).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();

                entity.Property(u => u.Email).HasMaxLength(128)
                    .IsRequired();
                entity.Property(u => u.Login).HasMaxLength(32)
                    .IsRequired();
                entity.Property(u => u.Description).HasColumnType("text");
                entity.Property(u => u.PhoneNumber).HasMaxLength(11);
                entity.Property(u => u.Password).HasMaxLength(512).IsRequired();
                entity.Property(e => e.Salt).IsRequired()
                    .HasMaxLength(32);
                entity.Property(e => e.Role).HasMaxLength(8).IsRequired()
                    .HasDefaultValueSql("'User'");
                entity.Property(e => e.RecoveryAddress).HasMaxLength(32);

                entity.HasOne(u => u.Photo).WithOne(p => p.User).HasForeignKey<User>(u => u.PhotoId);

            });

            modelBuilder.Entity<Announcement>(entity =>
            {
                entity.Property(a => a.Id).UseIdentityByDefaultColumn();
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Title).IsRequired();
                entity.Property(a => a.Content).HasColumnType("text").IsRequired();
                entity.Property(a => a.DateCreated).HasDefaultValueSql("Now()");
                entity.Property(a => a.UserId);
                entity.HasOne(a => a.User)
                    .WithMany(u => u.Announcements)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });


            modelBuilder.Entity<Genre>(entity =>
            {
                entity.Property(g => g.Id).UseIdentityByDefaultColumn();
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Value).IsRequired().HasMaxLength(128);
                entity.HasIndex(u => u.Value).IsUnique();
            });

            modelBuilder.Entity<UserLikedGenre>(entity =>
            {
                entity.HasKey(ug => new {ug.GenreId, ug.UserId});

                entity.HasOne(ug => ug.User)
                    .WithMany(ug => ug.LikedGenres)
                    .HasForeignKey(ug => ug.UserId);

                entity.HasOne(ug => ug.Genre)
                    .WithMany(ug => ug.LikedByUsers)
                    .HasForeignKey(ug => ug.GenreId);
            });

            modelBuilder.Entity<Models.System>(entity =>
            {
                entity.Property(s => s.Id).UseIdentityByDefaultColumn();
                entity.HasKey(s => s.Id);

                entity.Property(s => s.Manufacturer).IsRequired().HasMaxLength(128);
                entity.Property(s => s.Model).IsRequired().HasMaxLength(128);

                entity.HasIndex(u => new {u.Manufacturer, u.Model});
            });

            modelBuilder.Entity<UserOwnedSystem>(entity =>
            {
                entity.HasKey(us => new {us.SystemId, us.UserId});

                entity.HasOne(us => us.User)
                    .WithMany(us => us.OwnedSystems)
                    .HasForeignKey(us => us.UserId);

                entity.HasOne(us => us.System)
                    .WithMany(us => us.OwnedByUsers)
                    .HasForeignKey(us => us.SystemId);
            });

            modelBuilder.Entity<Region>(entity =>
            {
                entity.Property(r => r.Id).UseIdentityByDefaultColumn();
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Value).IsRequired().HasMaxLength(16);
                entity.HasIndex(r => r.Value).IsUnique();
            });

            modelBuilder.Entity<Region>().HasData(
                new Region() { Id = 1, Value = "PAL"},
                new Region() { Id = 2, Value = "NTSC"},
                new Region() { Id = 3, Value = "INNY"},
                new Region() { Id = 4, Value = "MULTI"}
                );

            modelBuilder.Entity<State>(entity =>
            {
                entity.Property(s => s.Id).UseIdentityByDefaultColumn();
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Value).IsRequired().HasMaxLength(16);
                entity.HasIndex(s => s.Value).IsUnique();
            });

            modelBuilder.Entity<State>().HasData(
                new State() { Id = 1, Value = "Nowy" },
                new State() { Id = 2, Value = "Używany" },
                new State() { Id = 3, Value = "Uszkodzony" }
            );

            modelBuilder.Entity<Photo>(entity =>
            {
                entity.Property(p => p.Id).UseIdentityByDefaultColumn();
                entity.HasKey(p => p.Id);
                entity.Property(p => p.DateCreated).HasDefaultValueSql("Now()");
                entity.Property(p => p.Path).IsRequired();
                entity.Property(p => p.AdvertisementId);
                entity.HasOne(p => p.Advertisement)
                    .WithMany(a => a.Photos)
                    .HasForeignKey(p => p.AdvertisementId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Advertisement>(entity =>
            {
                entity.Property(a => a.Id).UseIdentityByDefaultColumn();
                entity.HasKey(a => a.Id);               
                entity.Property(a => a.DateCreated).HasDefaultValueSql("Now()");
                entity.Property(a => a.IsActive).IsRequired().HasDefaultValue(true);
                entity.Property(a => a.ExchangeActive).IsRequired().HasDefaultValue(true);
                entity.Property(a => a.Price).IsRequired().HasColumnType("money");
                entity.Property(a => a.Title).IsRequired();
                entity.Property(a => a.UserId).IsRequired();
                entity.HasOne(a => a.User).WithMany(u => u.Advertisements).HasForeignKey(a => a.UserId);
            });

            modelBuilder.Entity<AdvertisementItem>(entity =>
            {
                entity.Property(a => a.Id).UseIdentityByDefaultColumn();
                entity.HasKey(a => a.Id);
                entity.Property(a => a.AdvertisementId).IsRequired();
                entity.Property(a => a.StateId).IsRequired();
                entity.Property(a => a.SystemId).IsRequired();
                entity.Property(a => a.Description).HasColumnType("text").IsRequired();

                entity.HasOne(a => a.State).WithMany(s => s.AdvertisementItems).HasForeignKey(a => a.StateId);
                entity.HasOne(a => a.System).WithMany(s => s.AdvertisementItems).HasForeignKey(a => a.SystemId);
                entity.HasOne(a => a.Advertisement).WithOne(ad => ad.Item).HasForeignKey<AdvertisementItem>(a => a.AdvertisementId);
            });

            modelBuilder.Entity<Console>(entity =>
            {
                entity.HasOne(c => c.ConsoleRegion).WithMany(r => r.Consoles)
                    .HasForeignKey(c => c.ConsoleRegionId);
            });

            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasOne(g => g.Genre).WithMany(ge => ge.Games).HasForeignKey(g => g.GenreId);
                entity.HasOne(g => g.GameRegion).WithMany(r => r.Games).HasForeignKey(g => g.GameRegionId);
            });

            modelBuilder.Entity<Message>(entity =>
                {
                    entity.Property(m => m.Id).UseIdentityByDefaultColumn();
                    entity.HasKey(m => m.Id);
                    entity.Property(m => m.Content).HasColumnType("text").IsRequired();
                    entity.Property(m => m.DateCreated).HasDefaultValueSql("Now()");
                    entity.Property(m => m.SenderId).IsRequired();
                    entity.Property(m => m.ReceiverId).IsRequired();
                    entity
                        .HasOne(m => m.Sender)
                        .WithMany(u => u.MessagesSent)
                        .HasForeignKey(m => m.SenderId);
                    entity
                        .HasOne(m => m.Receiver)
                        .WithMany(u => u.MessagesRecived)
                        .HasForeignKey(m => m.ReceiverId);

                });

            base.OnModelCreating(modelBuilder);
        } 
        
    }
}
