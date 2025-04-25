using Api.Infrastructure.Data.TypeConverters;
using Api.Infrastructure.DbTables;
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
    public DbSet<Customer> Customers { get; set; } //=> Set<Customer>();
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
        ICurrentUserService currentUserService)
        : base(options)
    {
        _conventions = conventions;
        _currentUserId = currentUserService.UserId;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ApplicationUserRecord>(u =>
        {
            u
                .Property(u => u.Id)
                .HasConversion<IdentityUserIdValueConverter>()
                .HasValueGenerator<IdentityUserIdValueGenerator>()
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
        });

        modelBuilder
            .Entity<Customer>(b =>
            {
                b.Property(c => c.Id)
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
                def.Property(p => p.Id).HasConversion<ScheduledJobDefinitionIdConverter>().HasValueGenerator<ScheduledJobDefinitionIdValueGenerator>().ValueGeneratedOnAdd();
                def.OwnsOne(
                    d => d.Pattern,
                    rp =>
                    {
                        rp.Property(p => p.Id).HasConversion<RecurrencePatternIdConverter>().HasValueGenerator<RecurrencePatternIdValueGenerator>().ValueGeneratedOnAdd();
                        rp.Property(p => p.Frequency).HasColumnName("Frequency").IsRequired();
                        rp.Property(p => p.Interval).HasColumnName("Interval").IsRequired();
                        rp.Property(p => p.WeekDays).HasColumnName("DaysOfWeek");
                        rp.Property(p => p.DayOfMonth).HasColumnName("DayOfMonth");
                        rp.Property(p => p.CronExpression).HasColumnName("CronExpression");
                    });

                // Definition → Occurrences
                def.HasMany(d => d.JobOccurrences)
                    .WithOne(o => o.ScheduledJobDefinition)
                    .HasForeignKey(o => o.ScheduledJobDefinitionId)
                    .OnDelete(DeleteBehavior.Cascade);
                    ;
            });

        // Occurrence → Reminders
        modelBuilder
            .Entity<JobOccurrence>(oc =>
            {
                oc.Property(o => o.Id)
                    .HasConversion<JobOccurrenceIdConverter>()
                    .HasValueGenerator<JobOccurrenceIdValueGenerator>()
                    .ValueGeneratedOnAdd();
                oc.Property(o => o.OccurrenceDate).IsRequired();
                oc.Property(o => o.ScheduledJobDefinitionId).HasConversion<ScheduledJobDefinitionIdConverter>();
                oc.Property(o => o.CustomerId).HasConversion<CustomerIdConverter>();
                oc.HasMany(o => o.JobReminders)
                    .WithOne(r => r.JobOccurrence)
                    .HasForeignKey(r => r.JobOccurrenceId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

        modelBuilder.Entity<JobReminder>(entity =>
        {
            entity.Property(e => e.ReminderDateTime).IsRequired();
            entity.Property(c => c.Id)
                .HasConversion<JobReminderIdConverter>()
                .HasValueGenerator<JobReminderIdValueGenerator>()
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Message).IsRequired();
            entity.HasOne(e => e.JobOccurrence)
                .WithMany(e => e.JobReminders)
                .HasForeignKey(e => e.JobOccurrenceId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}