using Api.Infrastructure.DbTables;

namespace Api.Infrastructure.Services;

public interface IUserRetriever
{
    Task<ApplicationUserRecord> GetAdminUser();
}