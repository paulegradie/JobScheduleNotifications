// using AutoMapper;
// using Api.Business.Entities;
// using Server.Contracts.Dtos;
// using System.Collections.Generic;
//
// public class DtoMappingProfile : Profile
// {
//     public DtoMappingProfile()
//     {
//         //
//         //  DomainModel → DTO
//         //
//         CreateMap<ScheduledJobDefinitionDomainModel, ScheduledJobDefinitionDto>()
//             .ForCtorParam("CustomerId",       opt => opt.MapFrom(src => src.CustomerId))
//             .ForCtorParam("Id",               opt => opt.MapFrom(src => src.ScheduledJobDefinitionId))
//             .ForCtorParam("AnchorDate",       opt => opt.MapFrom(src => src.AnchorDate))
//             .ForCtorParam("Pattern",          opt => opt.MapFrom(src => src.Pattern))
//             .ForCtorParam("JobOccurrences",   opt => opt.MapFrom(src => src.JobOccurrences))
//             .ForCtorParam("Title",            opt => opt.MapFrom(src => src.Title))
//             .ForCtorParam("Description",      opt => opt.MapFrom(src => src.Description));
//
//         CreateMap<RecurrencePatternDomainModel, RecurrencePatternDto>();
//         CreateMap<JobOccurrenceDomainModel, JobOccurrenceDto>();
//         CreateMap<JobReminderDomainModel, JobReminderDto>();
//
//         //
//         //  DTO → DomainModel
//         //
//         CreateMap<ScheduledJobDefinitionDto, ScheduledJobDefinitionDomainModel>()
//             .ForMember(dest => dest.CustomerId,
//                        opt => opt.MapFrom(src => src.CustomerId))
//             .ForMember(dest => dest.ScheduledJobDefinitionId,
//                        opt => opt.MapFrom(src => src.Id))
//             .ForMember(dest => dest.AnchorDate,
//                        opt => opt.MapFrom(src => src.AnchorDate))
//             .ForMember(dest => dest.Pattern,
//                        opt => opt.MapFrom(src => src.Pattern))
//             .ForMember(dest => dest.JobOccurrences,
//                        opt => opt.MapFrom(src => src.JobOccurrences))
//             .ForMember(dest => dest.Title,
//                        opt => opt.MapFrom(src => src.Title))
//             .ForMember(dest => dest.Description,
//                        opt => opt.MapFrom(src => src.Description));
//
//         CreateMap<RecurrencePatternDto, RecurrencePatternDomainModel>()
//             .ConstructUsing(src => new RecurrencePatternDomainModel(
//                 src.Frequency,
//                 src.Interval,
//                 src.WeekDays,
//                 src.DayOfMonth,
//                 src.CronExpression));
//
//         CreateMap<JobOccurrenceDto, JobOccurrenceDomainModel>()
//             .ConstructUsing((src, ctx) =>
//                 new JobOccurrenceDomainModel(
//                     src.OccurrenceDate,
//                     ctx.Mapper.Map<List<JobReminderDomainModel>>(src.JobReminders)
//                 )
//             );
//
//         CreateMap<JobReminderDto, JobReminderDomainModel>()
//             .ConstructUsing(src =>
//                 new JobReminderDomainModel(
//                     src.ReminderDateTime,
//                     src.Message
//                 )
//             );
//     }
// }
