using Api.ValueTypes;
using Api.ValueTypes.Enums;

namespace Server.Contracts.Dtos;

public class CreateScheduledJobDefinitionDto
{
    public CustomerId CustomerId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime AnchorDate { get; set; }
    public string CronExpression { get; set; }
}