using Api.Infrastructure.DbTables;
using JobScheduleNotifications.Core.Entities;
using JobScheduleNotifications.Core.Exceptions;
using JobScheduleNotifications.Core.Mappers;

namespace Api.Infrastructure.Repositories.Mappers;

public class ToBusinessOwnerDomainModelMapper : IMapToTheDomain<ApplicationUserRecord, BusinessOwnerDomainModel>
{
    public Task<BusinessOwnerDomainModel> Map(ApplicationUserRecord from, CancellationToken cancellationToken = default)
    {
        if (from.UserName is null) throw new DomainException("Failed retrieve username from user record");
        return Task.FromResult(new BusinessOwnerDomainModel());
    }
}