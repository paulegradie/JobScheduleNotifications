namespace Api.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    public Guid? UserId { get; set; }
}