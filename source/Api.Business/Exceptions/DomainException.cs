namespace JobScheduleNotifications.Core.Exceptions;

public class DomainException(string? message) : Exception(message)
{
}