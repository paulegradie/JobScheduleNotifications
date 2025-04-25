classDiagram
%% Entities
class ScheduledJobDefinition {
+ScheduledJobDefinitionId Id
+CustomerId CustomerId
+DateTime AnchorDate
+string Title
+string Description
}
class RecurrencePattern {
+RecurrencePatternId Id
+Frequency Frequency
+int Interval
+WeekDays DaysOfWeek
+int? DayOfMonth
+string? CronExpression
}
class JobOccurrence {
+JobOccurrenceId Id
+DateTime OccurrenceDate
+DateTime? CompletedDate
}
class JobReminder {
+JobReminderId Id
+DateTime ReminderDate
+string Message
}

    %% Relationships
    ScheduledJobDefinition o-- RecurrencePattern : hasPattern
    ScheduledJobDefinition "1" -- "0..*" JobOccurrence : occurrences
    JobOccurrence "1" -- "0..*" JobReminder : reminders

    %% Supporting enums
    class Frequency {
      <<enumeration>>
      Daily
      Weekly
      Monthly
      Yearly
      Cron
    }
    class WeekDays {
      <<flags enum>>
      None
      Sunday
      Monday
      Tuesday
      Wednesday
      Thursday
      Friday
      Saturday
    }
