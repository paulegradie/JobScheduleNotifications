using Server.Contracts.Dtos;

namespace Server.Contracts.Client.Endpoints.Reminders.Contracts;

public sealed record ListJobRemindersResponse(IEnumerable<JobReminderDto> JobReminderDtos);