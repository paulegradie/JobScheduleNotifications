﻿using System.ComponentModel.DataAnnotations.Schema;
using Api.Infrastructure.DbTables.OrganizationModels;
using Api.ValueTypes;

namespace Api.Infrastructure.DbTables.Jobs;

public class JobOccurrence
{
    public JobOccurrenceId JobOccurrenceId { get; set; }
    public DateTime OccurrenceDate { get; set; }
    public DateTime? CompletedDate { get; set; }

    [NotMapped] public bool MarkedAsCompleted => CompletedDate.HasValue;

    // relationships

    //UP
    public CustomerId CustomerId { get; set; }
    public virtual Customer Customer { get; set; } = null!;
    public virtual ScheduledJobDefinition ScheduledJobDefinition { get; set; } = null!;
    public ScheduledJobDefinitionId ScheduledJobDefinitionId { get; set; }

    // DOWN
    public JobOccurrenceStatus JobOccurrenceStatus { get; set; }
}

public enum JobOccurrenceStatus
{
    NotStarted,
    InProgress,
    Completed,
    Canceled
}