using JobScheduleNotifications.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobScheduleNotifications.Infrastructure.Data;

public class JobScheduleDbContext : DbContext
{
    public JobScheduleDbContext(DbContextOptions<JobScheduleDbContext> options) : base(options)
    {
    }

    public DbSet<BusinessOwner> BusinessOwners { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<ScheduledJob> ScheduledJobs { get; set; } = null!;
    public DbSet<JobReminder> JobReminders { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BusinessOwner>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired();
            entity.Property(e => e.LastName).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.BusinessName).IsRequired();
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.HasOne(e => e.BusinessOwner)
                .WithMany(e => e.Customers)
                .HasForeignKey(e => e.BusinessOwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ScheduledJob>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired();
            entity.Property(e => e.ScheduledDate).IsRequired();
            entity.HasOne(e => e.Customer)
                .WithMany(e => e.ScheduledJobs)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.BusinessOwner)
                .WithMany(e => e.ScheduledJobs)
                .HasForeignKey(e => e.BusinessOwnerId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<JobReminder>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ReminderTime).IsRequired();
            entity.Property(e => e.Message).IsRequired();
            entity.HasOne(e => e.ScheduledJob)
                .WithMany(e => e.Reminders)
                .HasForeignKey(e => e.ScheduledJobId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
} 