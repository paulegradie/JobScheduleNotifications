using Api.ValueTypes;
using Api.ValueTypes.Enums;

namespace Server.Contracts.Dtos;

public class UpdateJobDto
{
    public CustomerId CustomerId { get; set; }
    public ScheduledJobDefinitionId JobDefinitionId { get; set; }

    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string CronExpression { get; init; }
    public required DateTime AnchorDate { get; init; }
}