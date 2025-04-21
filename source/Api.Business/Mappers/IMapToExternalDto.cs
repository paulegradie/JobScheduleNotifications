namespace Api.Controllers;

public interface IMapToExternalDto<in TFrom, TTo>
{
    Task<TTo> Map(TFrom from, CancellationToken cancellationToken = default);
}