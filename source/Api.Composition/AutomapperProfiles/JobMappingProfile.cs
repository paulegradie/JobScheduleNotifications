using Api.Business.Entities;
using Api.Infrastructure.DbTables.Jobs;
using AutoMapper;

namespace Api.Composition.AutomapperProfiles;

public class JobMappingProfile : Profile
{
    public JobMappingProfile()
    {
        //
        //  Domain → EF-Entity
        //
        CreateMap<ScheduledJobDefinitionDomainModel, ScheduledJobDefinition>()
            // map your domain’s ScheduledJobDefinitionId → entity.Id
            .ForMember(dest => dest.ScheduledJobDefinitionId,
                opt => opt.MapFrom(src => src.ScheduledJobDefinitionId))

            // anchor, title, description, CustomerId all match by name
            .ForMember(dest => dest.AnchorDate, opt => opt.MapFrom(src => src.AnchorDate))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))

            // nested one-to-one
            .ForMember(dest => dest.Pattern,
                opt => opt.MapFrom(src => src.Pattern))

            // child collection
            .ForMember(dest => dest.JobOccurrences,
                opt => opt.MapFrom(src => src.JobOccurrences))
            ;

        CreateMap<RecurrencePatternDomainModel, RecurrencePattern>()
            // if you want EF to generate RecurrencePatternId on insert, ignore it:
            .ForMember(dest => dest.RecurrencePatternId, opt => opt.Ignore())
            // ScheduledJobDefinitionId will get set by EF when you save the parent:
            .ForMember(dest => dest.ScheduledJobDefinitionId, opt => opt.Ignore())
            ;

        CreateMap<JobOccurrenceDomainModel, JobOccurrence>()
            .ForMember(dest => dest.JobOccurrenceId, opt => opt.Ignore())
            .ForMember(dest => dest.ScheduledJobDefinitionId, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
            .ForMember(dest => dest.JobReminders,
                opt => opt.MapFrom(src => src.JobReminders))
            ;

        CreateMap<JobReminderDomainModel, JobReminder>()
            .ForMember(dest => dest.JobReminderId, opt => opt.Ignore())
            .ForMember(dest => dest.JobOccurrenceId, opt => opt.Ignore())
            ;

        //
        //  EF-Entity → Domain
        //
        CreateMap<ScheduledJobDefinition, ScheduledJobDefinitionDomainModel>()
            .ForMember(dest => dest.ScheduledJobDefinitionId,
                opt => opt.MapFrom(src => src.ScheduledJobDefinitionId))
            .ForMember(dest => dest.AnchorDate,
                opt => opt.MapFrom(src => src.AnchorDate))
            .ForMember(dest => dest.Title,
                opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description,
                opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.CustomerId,
                opt => opt.MapFrom(src => src.CustomerId))
            .ForMember(dest => dest.Pattern,
                opt => opt.MapFrom(src => src.Pattern))
            .ForMember(dest => dest.JobOccurrences,
                opt => opt.MapFrom(src => src.JobOccurrences))
            ;

        CreateMap<RecurrencePattern, RecurrencePatternDomainModel>()
            // you may need a ctor or parameterless on your domain model
            .ConstructUsing(src => new RecurrencePatternDomainModel(
                src.Frequency, src.Interval, src.WeekDays, src.DayOfMonth, src.CronExpression))
            ;

        CreateMap<JobOccurrence, JobOccurrenceDomainModel>()
            .ForMember(dest => dest.OccurrenceDate, opt => opt.MapFrom(src => src.OccurrenceDate))
            .ForMember(dest => dest.JobReminders,
                opt => opt.MapFrom(src => src.JobReminders))
            ;

        CreateMap<JobReminder, JobReminderDomainModel>()
            .ForMember(dest => dest.ReminderDateTime,
                opt => opt.MapFrom(src => src.ReminderDateTime))
            .ForMember(dest => dest.Message,
                opt => opt.MapFrom(src => src.Message))
            ;
    }
}