using Api.ValueTypes;

namespace Server.Contracts.Dtos;

public record ScheduledJobDefinitionDto(CustomerId CustomerId, ScheduledJobDefinitionId Id, DateTime AnchorDate, RecurrencePatternDto Pattern, string Title, string Description)
{
    public ScheduledJobDefinitionId Id { get; set; } = Id;
    public CustomerId CustomerId { get; set; } = CustomerId;
    public DateTime AnchorDate { get; set; } = AnchorDate;
    public RecurrencePatternDto Pattern { get; set; } = Pattern;
    public List<JobOccurrenceDto> JobOccurrences { get; set; } = new();

    public string Title { get; set; } = Title;
    public string Description { get; set; } = Description;
    public int DayOfMonth { get; set; }
};