﻿using Server.Contracts.Dtos;

namespace Server.Contracts.Endpoints.Reminders.Contracts;

public sealed record AcknowledgeJobReminderResponse(JobReminderDto Reminder);