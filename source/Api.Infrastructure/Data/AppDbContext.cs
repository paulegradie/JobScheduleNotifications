using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Api.Infrastructure.DbTables;
using Api.Infrastructure.EntityFramework;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUserRecord>
{
    private readonly IEnumerable<IEntityPropertyConvention> _conventions;

    public AppDbContext(DbContextOptions<AppDbContext> options, IEnumerable<IEntityPropertyConvention> conventions) : base(options)
    {
        _conventions = conventions;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var entities = typeof(AppDbContext)
            .Assembly
            .GetTypes()
            .Where(x => x.IsClass && x.GetCustomAttribute<DatabaseModelAttribute>() != null);

        foreach (var entity in entities)
        {
            var entityBuilder = modelBuilder.Entity(entity).ToTable(entity.Name);
            foreach (var entityProperty in entity.GetProperties()
                         .Where(x => x.GetCustomAttribute<NotMappedAttribute>() is null))
            {
                foreach (var convention in _conventions)
                {
                    convention.Apply(modelBuilder, entityBuilder, entityProperty);
                }
            }
        }

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