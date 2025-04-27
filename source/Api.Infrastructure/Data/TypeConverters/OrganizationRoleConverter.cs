using Api.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Api.Infrastructure.Data.TypeConverters;

public class OrganizationRoleConverter()
    : ValueConverter<OrganizationRole, string>(
        role => role.Name,
        s => new OrganizationRole(s));