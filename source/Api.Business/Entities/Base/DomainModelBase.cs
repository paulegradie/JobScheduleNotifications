namespace Api.Business.Entities.Base;

public abstract class DomainModelBase<T> : IConvertToDto<T>
{
    public abstract T ToDto();
}