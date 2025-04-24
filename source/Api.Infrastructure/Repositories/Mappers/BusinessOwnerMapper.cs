// using Api.Business.Entities;
//
// namespace Api.Infrastructure.Repositories.Mappers;
//
// public class BusinessOwnerEntityMapper : IMapFrom<BusinessOwnerDomainModel, BusinessOwner>
// {
//     public Task<BusinessOwner> Map(BusinessOwnerDomainModel from)
//     {
//         var result = new BusinessOwner
//         {
//             Email = from.Email,
//             BusinessName = from.BusinessName,
//             FirstName = from.FirstName,
//             LastName = from.LastName,
//             PhoneNumber = from.PhoneNumber,
//             BusinessDescription = from.BusinessDescription,
//             CreatedAt = DateTime.UtcNow,
//             ModifiedAt = null,
//             IsActive = from.IsActive
//         };
//
//         return Task.FromResult(result);
//     }
// }
