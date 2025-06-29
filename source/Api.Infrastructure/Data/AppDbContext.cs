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
    public DbSet<JobCompletedPhoto> JobCompletedPhotos { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<ScheduledJobDefinition> ScheduledJobDefinitions { get; set; }
    public DbSet<JobOccurrence> JobOccurrences { get; set; }
    public DbSet<JobReminder> JobReminders { get; set; }
    public DbSet<Invoice> Invoices { get; set; }

    // Link Tables
    public DbSet<CustomerUser> CustomerUsers { get; set; }
    public DbSet<OrganizationUser> OrganizationUsers { get; set; }

    // Org
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<OrganizationSettings> OrganizationSettings { get; set; }
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

        modelBuilder.Entity<OrganizationSettings>(settings =>
        {
            settings.HasKey(s => s.OrganizationId);
            settings.Property(s => s.OrganizationId).HasConversion<OrganizationIdValueConverter>().IsRequired();

            settings.Property(s => s.BusinessName).HasMaxLength(200);
            settings.Property(s => s.BusinessDescription).HasMaxLength(1000);
            settings.Property(s => s.BusinessIdNumber).HasMaxLength(50);
            settings.Property(s => s.Email).HasMaxLength(255);
            settings.Property(s => s.PhoneNumber).HasMaxLength(50);
            settings.Property(s => s.StreetAddress).HasMaxLength(500);
            settings.Property(s => s.City).HasMaxLength(100);
            settings.Property(s => s.State).HasMaxLength(100);
            settings.Property(s => s.PostalCode).HasMaxLength(20);
            settings.Property(s => s.Country).HasMaxLength(100);
            settings.Property(s => s.BankName).HasMaxLength(200);
            settings.Property(s => s.BankBsb).HasMaxLength(20);
            settings.Property(s => s.BankAccountNumber).HasMaxLength(50);
            settings.Property(s => s.BankAccountName).HasMaxLength(200);
            settings.Property(s => s.CreatedAt).IsRequired();

            settings.HasOne(s => s.Organization)
                .WithOne()
                .HasForeignKey<OrganizationSettings>(s => s.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
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

                oc.HasMany(o => o.JobCompletedPhotos)
                    .WithOne(p => p.JobOccurrence)
                    .HasForeignKey(p => p.JobOccurrenceId)
                    .OnDelete(DeleteBehavior.Cascade);

                oc.HasMany(o => o.Invoices)
                    .WithOne(i => i.JobOccurrence)
                    .HasForeignKey(i => i.JobOccurrenceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

        modelBuilder
            .Entity<JobCompletedPhoto>(photo =>
            {
                photo.HasKey(p => p.JobCompletedPhotoId);

                photo.Property(p => p.JobCompletedPhotoId)
                    .HasConversion<JobCompletedPhotoIdConverter>()
                    .HasValueGenerator<JobCompletedPhotoIdValueGenerator>()
                    .ValueGeneratedOnAdd();

                photo.Property(p => p.CustomerId).HasConversion<CustomerIdConverter>();
                photo.Property(p => p.JobOccurrenceId).HasConversion<JobOccurrenceIdConverter>();
                photo.Property(p => p.LocalFilePath).IsRequired();
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

        modelBuilder.Entity<Invoice>(invoice =>
        {
            invoice.HasKey(i => i.InvoiceId);
            invoice.Property(i => i.InvoiceId)
                .HasConversion<InvoiceIdConverter>()
                .HasValueGenerator<InvoiceIdValueGenerator>()
                .ValueGeneratedOnAdd();

            invoice.Property(i => i.JobOccurrenceId).HasConversion<JobOccurrenceIdConverter>();
            invoice.Property(i => i.CustomerId).HasConversion<CustomerIdConverter>();
            invoice.Property(i => i.FileName).IsRequired().HasMaxLength(255);
            invoice.Property(i => i.FilePath).IsRequired().HasMaxLength(500);
            invoice.Property(i => i.StorageLocation).HasConversion<int>();
            invoice.Property(i => i.CreatedDate).IsRequired();



            invoice.HasOne(i => i.Customer)
                .WithMany()
                .HasForeignKey(i => i.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}