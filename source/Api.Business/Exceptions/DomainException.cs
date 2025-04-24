namespace Api.Business.Exceptions;

public class DomainException(string? message) : Exception(message)
{
}