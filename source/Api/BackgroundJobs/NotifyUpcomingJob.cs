using System.Net.Mail;
using Api.Business.Entities;
using Api.Business.Repositories;
using Api.Business.Services;
using Server.Contracts.Cron;

namespace Api.BackgroundJobs
{
    public class NotifyUpcomingJob : BackgroundService
    {
        private readonly ILogger<NotifyUpcomingJob> _logger;
        private readonly IScheduledJobDefinitionRepository _scheduledJobDefinitionRepository;
        private readonly ISendNotificationOfUpcomingJobs _sendNotificationOfUpcomingJobs;
        private readonly IRecurrenceCalculator _recurrenceCalculator;
        private readonly IJobReminderRepository _jobReminderRepository;

        public NotifyUpcomingJob(
            ILogger<NotifyUpcomingJob> logger,
            IScheduledJobDefinitionRepository scheduledJobDefinitionRepository,
            ISendNotificationOfUpcomingJobs sendNotificationOfUpcomingJobs,
            IRecurrenceCalculator recurrenceCalculator,
            IJobReminderRepository jobReminderRepository)
        {
            _logger = logger;
            _scheduledJobDefinitionRepository = scheduledJobDefinitionRepository;
            _sendNotificationOfUpcomingJobs = sendNotificationOfUpcomingJobs;
            _recurrenceCalculator = recurrenceCalculator;
            _jobReminderRepository = jobReminderRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NotifyUpcomingJob service starting.");

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                // run on startup
            }
            catch (TaskCanceledException)
            {
                return;
            }

            await DoWorkAsyncOuter(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Delaying {Delay} until next notification at {NextRun}", "1Hr", DateTime.Now.AddHours(1));
                try
                {
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }

                // Time to do the work!
                await DoWorkAsyncOuter(stoppingToken);
            }

            _logger.LogInformation("NotifyUpcomingJob service stopping.");
        }

        private async Task DoWorkAsyncOuter(CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("NotifyUpcomingJob running at {Time}", DateTime.Now);
                await DoWorkAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in NotifyUpcomingJob");
            }
        }

        private record JobToNotify(ScheduledJobDefinitionDomainModel ScheduledJobDefinition, DateTime NextRun);

        private async Task DoWorkAsync(CancellationToken ct)
        {
            // create a scope so you can resolve your DbContext / repositories

            var allJobs = await _scheduledJobDefinitionRepository.ListAllAsync();

            var nextOccurrences = new List<JobToNotify>();
            foreach (var job in allJobs)
            {
                var cronTab = NCrontab.CrontabSchedule.Parse(job.CronExpression);
                var schedule = new CronSchedule(cronTab);
                var nextOccurrence = _recurrenceCalculator.GetNextOccurrence(schedule, job.AnchorDate);
                nextOccurrences.Add(new JobToNotify(job, nextOccurrence));
            }

            if (nextOccurrences.Count == 0)
            {
                _logger.LogInformation("No upcoming jobs to notify.");
                return;
            }

            var now = DateTime.UtcNow;
            var nextJobsToNotifyFor = nextOccurrences
                .Where(x => x.NextRun > now && x.NextRun <= now.AddDays(1))
                .ToList();

            if (nextJobsToNotifyFor.Count == 0)
            {
                _logger.LogInformation("No upcoming jobs to notify.");
            }

            foreach (var jobToNotify in nextJobsToNotifyFor)
            {
                await _sendNotificationOfUpcomingJobs
                    .SendNotification(jobToNotify.ScheduledJobDefinition);

                var newReminder = jobToNotify.ScheduledJobDefinition.CreateNewReminder();
                await _jobReminderRepository.AddAsync(newReminder);
            }
        }
    }
}