﻿using Games4Trade.Models;
using Microsoft.EntityFrameworkCore;

namespace Games4Trade.Data
{
    public class ApplicationContext : DbContext, IApplicationContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<ObservedUsersRelationShip> ObservedUsersRelationship { get; set; }
        public DbContextOptions<ApplicationContext> Options { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Options = options;
        }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ObservedUsersRelationShip>(entity =>
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
                entity.Property(u => u.Id).UseNpgsqlIdentityByDefaultColumn();
                entity.HasKey(u => u.Id);

                entity.HasIndex(u => u.Login).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();

                entity.Property(u => u.Email).HasMaxLength(128)
                    .IsRequired();
                entity.Property(u => u.Login).HasMaxLength(32)
                    .IsRequired();
                entity.Property(u => u.Password).HasMaxLength(512).IsRequired();
                entity.Property(e => e.Salt).IsRequired()
                    .HasMaxLength(32);
                entity.Property(e => e.Role).HasMaxLength(8).IsRequired()
                    .HasDefaultValueSql("'User'");
                entity.Property(e => e.RecoveryAddress).HasMaxLength(32);

            });

            base.OnModelCreating(modelBuilder);
        }


        
    }
}
