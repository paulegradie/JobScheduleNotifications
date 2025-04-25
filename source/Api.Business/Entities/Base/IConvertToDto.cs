namespace Api.Business.Entities.Base;

public interface IConvertToDto<out T>
{
    T ToDto();
}