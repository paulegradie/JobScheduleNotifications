using System.Reflection;
using Api.Infrastructure.DbTables;
using Api.Infrastructure.DbTables.OrganizationModels;
using Api.Infrastructure.EntityFramework;
using Api.Infrastructure.Services;
using Api.ValueTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Api.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUserRecord, IdentityRole<UserId>, UserId>
{
    private readonly IEnumerable<IEntityPropertyConvention> _conventions;
    private readonly UserId? _currentUserId;

    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<ScheduledJob> ScheduledJobs => Set<ScheduledJob>();
    public DbSet<JobReminder> JobReminders => Set<JobReminder>();

    // Link Tables
    public DbSet<OrganizationUser> OrganizationUsers { get; set; }
    public DbSet<CustomerUser> CustomerUsers { get; set; }

    public AppDbContext(
        DbContextOptions options,
        IEnumerable<IEntityPropertyConvention> conventions,
        ICurrentUserService currentUserService)
        : base(options)
    {
        _conventions = conventions;
        _currentUserId = currentUserService.UserId;
    }


    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        var entry = Assembly.GetExecutingAssembly()
            .GetReferencedAssemblies()
            .SingleOrDefault(a => a.Name!.StartsWith("Api.ValueTypes"));
        if (entry is null) throw new Exception("ENTRY IS NULL DB _ EF CORE DAMMIT");
        var types = Assembly.Load(entry!);

        var converterTypes = types
            .GetTypes()
            .Where(t =>
                t is { IsAbstract: false, IsGenericTypeDefinition: false }
                && typeof(ValueConverter).IsAssignableFrom(t)
                && t.GetConstructor(Type.EmptyTypes) != null
            );

        foreach (var convType in converterTypes)
        {
            var converter = (ValueConverter)Activator.CreateInstance(convType)!;
            configurationBuilder
                .Properties(converter.ModelClrType)
                .HaveConversion(converter.GetType());
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<OrganizationUser>(b =>
        {
            b.HasKey(x => new { x.UserId, x.OrganizationId });

            b.HasOne(x => x.User)
                .WithMany(u => u.OrganizationUsers)
                .HasForeignKey(x => x.UserId);

            b.HasOne(x => x.Organization)
                .WithMany(o => o.OrganizationUsers)
                .HasForeignKey(x => x.OrganizationId);
        });

        modelBuilder.Entity<CustomerUser>(b =>
        {
            b.HasKey(x => new { x.UserId, x.CustomerId });

            b.HasOne(x => x.User)
                .WithMany(u => u.CustomerUsers)
                .HasForeignKey(x => x.UserId);

            b.HasOne(x => x.Customer)
                .WithMany(c => c.CustomerUsers)
                .HasForeignKey(x => x.CustomerId);
        });

        modelBuilder.Entity<Customer>(b =>
        {
            b.HasKey(c => c.Id);

            b.HasOne(c => c.Organization)
                .WithMany(o => o.Customers)
                .HasForeignKey(c => c.OrganizationId);
        });


        // if we want to scoop up db model types with the Attribute-driven conventions
        // Apply attribute-driven conventions
        // var entities = typeof(AppDbContext)
        //     .Assembly
        //     .GetTypes()
        //     .Where(x => x.IsClass && x.GetCustomAttribute<DatabaseModelAttribute>() != null);
        //
        // foreach (var entity in entities)
        // {
        //     var entityBuilder = modelBuilder.Entity(entity).ToTable(entity.Name);
        //     foreach (var entityProperty in entity.GetProperties()
        //                  .Where(x => x.GetCustomAttribute<NotMappedAttribute>() is null))
        //     {
        //         foreach (var convention in _conventions)
        //         {
        //             convention.Apply(modelBuilder, entityBuilder, entityProperty);
        //         }
        //     }
        // }

        modelBuilder.Entity<ScheduledJob>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired();
            entity.Property(e => e.ScheduledDate).IsRequired();
            entity
                .HasOne(e => e.Customer)
                .WithMany(e => e.ScheduledJobs)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
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