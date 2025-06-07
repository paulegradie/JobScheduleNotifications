using Api.Infrastructure.Data.TypeConverters;
using Api.Infrastructure.DbTables.Jobs;
using Api.Infrastructure.DbTables.OrganizationModels;
using Api.Infrastructure.EntityFramework;
using Api.Infrastructure.Services;
using Api.ValueTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUserRecord, IdentityRole<IdentityUserId>, IdentityUserId>
{
    private readonly IEnumerable<IEntityPropertyConvention> _conventions; // not using this, but available if we decide to for now
    private readonly IdentityUserId? _currentUserId; // can we even filter by id here?


    // Domain Tables
    public DbSet<Customer> Customers { get; set; }
    public DbSet<ScheduledJobDefinition> ScheduledJobDefinitions { get; set; }
    public DbSet<JobOccurrence> JobOccurrences { get; set; }
    public DbSet<JobReminder> JobReminders { get; set; }

    // Link Tables
    public DbSet<CustomerUser> CustomerUsers { get; set; }
    public DbSet<OrganizationUser> OrganizationUsers { get; set; }

    // Org
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<ApplicationUserRecord> ApplicationUsers => Set<ApplicationUserRecord>();


    public AppDbContext(
        DbContextOptions options,
        IEnumerable<IEntityPropertyConvention> conventions,
        ICurrentUserContext currentUserContext)
        : base(options)
    {
        _conventions = conventions;
        _currentUserId = currentUserContext.UserId;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ApplicationUserRecord>(u =>
        {
            u
                .Property(u => u.Id)
                .HasConversion<IdentityUserIdValueConverter>()
                .HasValueGenerator<IdentityUserIdValueGenerator>();
        });


        modelBuilder.Entity<IdentityRole<IdentityUserId>>(t =>
        {
            t.Property(x => x.Id)
                .HasConversion<IdentityUserIdValueConverter>()
                .HasValueGenerator<IdentityUserIdValueGenerator>();
            t.Property(x => x.NormalizedName).HasMaxLength(256);
        });


        modelBuilder.Entity<Organization>(o =>
        {
            o.HasKey(b => b.Id);
            o.Property(p => p.Id)
                .HasValueGenerator<OrganizationIdValueGenerator>()
                .HasConversion<OrganizationIdValueConverter>()
                .ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<OrganizationUser>(b =>
        {
            b.HasKey(x => new { UserId = x.IdentityUserId, x.OrganizationId });

            b.HasOne(x => x.User)
                .WithMany(u => u.OrganizationUsers)
                .HasForeignKey(x => x.IdentityUserId);

            b.HasOne(x => x.Organization)
                .WithMany(o => o.OrganizationUsers)
                .HasForeignKey(x => x.OrganizationId);

            b.Property(x => x.IdentityUserId).HasConversion<IdentityUserIdValueConverter>().IsRequired();
            b.Property(x => x.OrganizationId).HasConversion<OrganizationIdValueConverter>().IsRequired();

            b.Property(x => x.Role).HasConversion<OrganizationRoleConverter>().IsRequired();
        });

        modelBuilder
            .Entity<Customer>(b =>
            {
                b.HasKey(x => x.CustomerId);
                b.Property(c => c.CustomerId)
                    .HasValueGenerator<CustomerIdValueGenerator>()
                    .HasConversion<CustomerIdConverter>()
                    .ValueGeneratedOnAdd();

                b.Property(c => c.OrganizationId).HasConversion<OrganizationIdValueConverter>();

                b.HasMany(c => c.ScheduledJobDefinitions)
                    .WithOne(d => d.Customer)
                    .HasForeignKey(d => d.CustomerId);
                b.HasOne(c => c.Organization)
                    .WithMany(o => o.Customers)
                    .HasForeignKey(c => c.OrganizationId);
            });

        modelBuilder
            .Entity<CustomerUser>(b =>
            {
                b.HasKey(x => new { UserId = x.IdentityUserId, x.CustomerId });

                b.HasOne(x => x.User)
                    .WithMany(u => u.CustomerUsers)
                    .HasForeignKey(x => x.IdentityUserId);

                b.HasOne(x => x.Customer)
                    .WithMany(c => c.CustomerUsers)
                    .HasForeignKey(x => x.CustomerId);
                b.Property(x => x.IdentityUserId).HasConversion<IdentityUserIdValueConverter>().IsRequired();
                b.Property(x => x.CustomerId).HasConversion<CustomerIdConverter>().IsRequired();
            });


        modelBuilder
            .Entity<ScheduledJobDefinition>(def =>
            {
                def.HasKey(d => d.ScheduledJobDefinitionId);
                def.Property(p => p.ScheduledJobDefinitionId)
                    .HasConversion<ScheduledJobDefinitionIdConverter>()
                    .HasValueGenerator<ScheduledJobDefinitionIdValueGenerator>()
                    .ValueGeneratedOnAdd();

                def.Property(x => x.CustomerId).HasConversion<CustomerIdConverter>();

                def.HasMany(d => d.JobOccurrences)
                    .WithOne(o => o.ScheduledJobDefinition)
                    .HasForeignKey(o => o.ScheduledJobDefinitionId)
                    .OnDelete(DeleteBehavior.Cascade);
                def.HasMany(o => o.JobReminders)
                    .WithOne(r => r.ScheduledJobDefinition)
                    .HasForeignKey(r => r.ScheduledJobDefinitionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

        // Occurrence → Reminders
        modelBuilder
            .Entity<JobOccurrence>(oc =>
            {
                oc.HasKey(x => x.JobOccurrenceId);
                oc.Property(o => o.JobOccurrenceId)
                    .HasConversion<JobOccurrenceIdConverter>()
                    .HasValueGenerator<JobOccurrenceIdValueGenerator>()
                    .ValueGeneratedOnAdd();
                oc.Property(o => o.OccurrenceDate).IsRequired();
                oc.Property(o => o.ScheduledJobDefinitionId).HasConversion<ScheduledJobDefinitionIdConverter>();
                oc.Property(o => o.CustomerId).HasConversion<CustomerIdConverter>();
            });

        modelBuilder.Entity<JobReminder>(entity =>
        {
            entity.HasKey(e => e.JobReminderId);
            entity.Property(e => e.ReminderDateTime).IsRequired();
            entity.Property(c => c.JobReminderId)
                .HasConversion<JobReminderIdConverter>()
                .HasValueGenerator<JobReminderIdValueGenerator>()
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Message).IsRequired();
            entity.HasOne(e => e.ScheduledJobDefinition)
                .WithMany(e => e.JobReminders)
                .HasForeignKey(e => e.ScheduledJobDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}