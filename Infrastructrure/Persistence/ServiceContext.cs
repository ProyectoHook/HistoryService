using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructrure.Persistence
{
    public class ServiceContext : DbContext
    {
        public DbSet<OptionHistory> OptionHistories { get; set; }
        public DbSet<SlideHistory> SlideHistories { get; set; }
        public DbSet<SessionHistory> SessionHistories { get; set; }
        public DbSet<UserHistory> UserHistories { get; set; }

        public ServiceContext(DbContextOptions<ServiceContext> options) : base(options){ }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OptionHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.OptionText).IsRequired();

                entity.HasOne(e => e.SlideHistory)
                      .WithMany(s => s.Options)
                      .HasForeignKey(e => e.SlideHistoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SlideHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.OriginalSlideId).IsRequired();
                entity.Property(e => e.Ask);
                entity.Property(e => e.AnswerCorrect);

                entity.HasMany(e => e.Options)
                      .WithOne(e => e.SlideHistory)
                      .HasForeignKey(e => e.SlideHistoryId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.SessionHistories)
                      .WithOne(e => e.SlideHistory)
                      .HasForeignKey(e => e.SlideHistoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SessionHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.OriginalSlideId).IsRequired();
                entity.Property(e => e.UserCreate).IsRequired();
                entity.Property(e => e.SessionId).IsRequired();
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.PresentationId).IsRequired();

                entity.Property(e => e.UserAnswer);
                entity.Property(e => e.TimeElapsed);

                entity.HasOne(e => e.UserHistory)
                      .WithMany(u => u.SessionHistories)
                      .HasForeignKey(e => e.UserHistoryId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.SlideHistory)
                      .WithMany(s => s.SessionHistories)
                      .HasForeignKey(e => e.SlideHistoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Name).IsRequired();

                entity.HasMany(e => e.SessionHistories)
                      .WithOne(e => e.UserHistory)
                      .HasForeignKey(e => e.UserHistoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
