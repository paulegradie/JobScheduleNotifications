using Api.Business.Entities;
using Api.Business.Repositories;
using Api.Infrastructure.Data;
using Api.Infrastructure.DbTables.OrganizationModels;
using Api.ValueTypes;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class OrganizationSettingsRepository : IOrganizationSettingsRepository
{
    private readonly AppDbContext _context;

    public OrganizationSettingsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OrganizationSettingsDomainModel?> GetByOrganizationIdAsync(OrganizationId organizationId)
    {
        var entity = await _context.OrganizationSettings
            .FirstOrDefaultAsync(s => s.OrganizationId == organizationId);
            
        return entity?.ToDomainModel();
    }

    public async Task<OrganizationSettingsDomainModel> CreateAsync(OrganizationSettingsDomainModel settings)
    {
        var entity = settings.ToDbEntity();
        _context.OrganizationSettings.Add(entity);
        await _context.SaveChangesAsync();
        
        return entity.ToDomainModel();
    }

    public async Task<OrganizationSettingsDomainModel> UpdateAsync(OrganizationSettingsDomainModel settings)
    {
        var entity = await _context.OrganizationSettings
            .FirstOrDefaultAsync(s => s.OrganizationId == settings.OrganizationId);
            
        if (entity == null)
            throw new InvalidOperationException($"Organization settings not found for organization {settings.OrganizationId}");

        // Update entity properties
        entity.BusinessName = settings.BusinessName;
        entity.BusinessDescription = settings.BusinessDescription;
        entity.BusinessIdNumber = settings.BusinessIdNumber;
        entity.Email = settings.Email;
        entity.PhoneNumber = settings.PhoneNumber;
        entity.StreetAddress = settings.StreetAddress;
        entity.City = settings.City;
        entity.State = settings.State;
        entity.PostalCode = settings.PostalCode;
        entity.Country = settings.Country;
        entity.BankName = settings.BankName;
        entity.BankBsb = settings.BankBsb;
        entity.BankAccountNumber = settings.BankAccountNumber;
        entity.BankAccountName = settings.BankAccountName;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        
        return entity.ToDomainModel();
    }

    public async Task<OrganizationSettingsDomainModel> GetOrCreateAsync(OrganizationId organizationId)
    {
        var existing = await GetByOrganizationIdAsync(organizationId);
        if (existing != null)
            return existing;

        var defaultSettings = OrganizationSettingsDomainModel.CreateDefault(organizationId);
        return await CreateAsync(defaultSettings);
    }
}

// Extension methods for mapping between domain model and entity
public static class OrganizationSettingsExtensions
{
    public static OrganizationSettingsDomainModel ToDomainModel(this OrganizationSettings entity)
    {
        return new OrganizationSettingsDomainModel
        {
            OrganizationId = entity.OrganizationId,
            BusinessName = entity.BusinessName,
            BusinessDescription = entity.BusinessDescription,
            BusinessIdNumber = entity.BusinessIdNumber,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            StreetAddress = entity.StreetAddress,
            City = entity.City,
            State = entity.State,
            PostalCode = entity.PostalCode,
            Country = entity.Country,
            BankName = entity.BankName,
            BankBsb = entity.BankBsb,
            BankAccountNumber = entity.BankAccountNumber,
            BankAccountName = entity.BankAccountName,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static OrganizationSettings ToDbEntity(this OrganizationSettingsDomainModel domain)
    {
        return new OrganizationSettings
        {
            OrganizationId = domain.OrganizationId,
            BusinessName = domain.BusinessName,
            BusinessDescription = domain.BusinessDescription,
            BusinessIdNumber = domain.BusinessIdNumber,
            Email = domain.Email,
            PhoneNumber = domain.PhoneNumber,
            StreetAddress = domain.StreetAddress,
            City = domain.City,
            State = domain.State,
            PostalCode = domain.PostalCode,
            Country = domain.Country,
            BankName = domain.BankName,
            BankBsb = domain.BankBsb,
            BankAccountNumber = domain.BankAccountNumber,
            BankAccountName = domain.BankAccountName,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }
}
