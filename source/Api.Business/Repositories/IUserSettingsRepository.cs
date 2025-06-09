namespace Api.Business.Repositories;

public interface IUserSettingsRepository
{
    Task<CurrentUserSettings> GetCurrentUserSettings();
}

public class CurrentUserSettings
{
    public string BusinessName { get; set; }
    public string UserName { get; set; }
}