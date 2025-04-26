using Server.Contracts.Dtos;

namespace Server.Contracts.Client.Endpoints.Reminders.Contracts;

public sealed record AcknowledgeJobReminderResponse(JobReminderDto Reminder);