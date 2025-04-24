using Api.Infrastructure.DbTables.OrganizationModels;

namespace Api.Infrastructure.Services;

public interface IUserRetriever
{
    Task<ApplicationUserRecord> GetCurrentUserAsync();
}