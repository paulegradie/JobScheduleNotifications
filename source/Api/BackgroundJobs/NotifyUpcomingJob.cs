// using System;
// using System.Net.Mail;
// using System.Threading;
// using System.Threading.Tasks;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Logging;
//
// namespace Api.BackgroundJobs
// {
//     public class NotifyUpcomingJob : BackgroundService
//     {
//         private readonly ILogger<NotifyUpcomingJob> _logger;
//         private readonly IServiceScopeFactory _scopeFactory;
//         private readonly TimeSpan _scheduledTimeOfDay;
//
//         /// <summary>
//         /// </summary>
//         /// <param name="logger"></param>
//         /// <param name="scopeFactory">
//         ///   Use this to pull in your repositories / DbContext inside the loop.
//         /// </param>
//         /// <param name="hour">Hour (0–23) to run daily.</param>
//         /// <param name="minute">Minute (0–59) to run daily.</param>
//         public NotifyUpcomingJob(
//             ILogger<NotifyUpcomingJob> logger,
//             IServiceScopeFactory scopeFactory,
//             int hour = 9,
//             int minute = 0)
//         {
//             _logger         = logger;
//             _scopeFactory   = scopeFactory;
//             _scheduledTimeOfDay = new TimeSpan(hour, minute, 0);
//         }
//
//         protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//         {
//             _logger.LogInformation("NotifyUpcomingJob service starting.");
//
//             // compute first run
//             var nextRun = GetNextRunTime(_scheduledTimeOfDay);
//
//             while (!stoppingToken.IsCancellationRequested)
//             {
//                 var delay = nextRun - DateTime.Now;
//                 if (delay > TimeSpan.Zero)
//                 {
//                     _logger.LogInformation("Delaying {Delay} until next notification at {NextRun}", delay, nextRun);
//                     try
//                     {
//                         await Task.Delay(delay, stoppingToken);
//                     }
//                     catch (TaskCanceledException)
//                     {
//                         // shutdown requested
//                         break;
//                     }
//                 }
//
//                 // Time to do the work!
//                 try
//                 {
//                     _logger.LogInformation("NotifyUpcomingJob running at {Time}", DateTime.Now);
//                     await DoWorkAsync(stoppingToken);
//                 }
//                 catch (Exception ex)
//                 {
//                     _logger.LogError(ex, "Error in NotifyUpcomingJob");
//                 }
//
//                 // schedule for the next day
//                 nextRun = nextRun.AddDays(1);
//             }
//
//             _logger.LogInformation("NotifyUpcomingJob service stopping.");
//         }
//
//         private static DateTime GetNextRunTime(TimeSpan timeOfDay)
//         {
//             var now      = DateTime.Now;
//             var todayRun = now.Date + timeOfDay;
//             return (todayRun > now)
//                 ? todayRun
//                 : todayRun.AddDays(1);
//         }
//
//         private async Task DoWorkAsync(CancellationToken ct)
//         {
//             // create a scope so you can resolve your DbContext / repositories
//             using var scope = _scopeFactory.CreateScope();
//             var jobRepo     = scope.ServiceProvider.GetRequiredService<IJobRepository>();
//             var upcoming    = await jobRepo.GetJobsStartingInNextDayAsync(ct);
//
//             if (!upcoming.Any())
//             {
//                 _logger.LogInformation("No upcoming jobs to notify.");
//                 return;
//             }
//
//             // build your SMTP client (or inject one via DI)
//             using var smtp = new SmtpClient("your-smtp.server.com")
//             {
//                 Port     = 587,
//                 Credentials = new System.Net.NetworkCredential("username", "password"),
//                 EnableSsl   = true
//             };
//
//             foreach (var job in upcoming)
//             {
//                 var msg = new MailMessage("noreply@yourdomain.com",
//                                           job.CustomerEmail,
//                                           "Your job is coming up",
//                                           $"Hi {job.CustomerName}, your scheduled job “{job.Title}” is tomorrow at {job.StartTime:hh\\:mm tt}.");
//                 try
//                 {
//                     await smtp.SendMailAsync(msg, ct);
//                     _logger.LogInformation("Sent notification for job {JobId} to {Email}", job.Id, job.CustomerEmail);
//                 }
//                 catch (Exception ex)
//                 {
//                     _logger.LogError(ex, "Failed to send email for job {JobId}", job.Id);
//                 }
//             }
//         }
//     }
// }
