namespace Api.Infrastructure.Services;

public interface ICurrentUserService
{
    Guid? UserId { get; set; }
}