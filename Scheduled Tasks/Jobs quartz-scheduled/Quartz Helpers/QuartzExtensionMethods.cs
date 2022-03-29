using System;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl.Triggers;
using StocksProccesing.Relational.Model;

namespace Stocks_Data_Processing.Quartz_Helpers;

public static class QuartzExtensionMethods
{
    public static async Task ScheduleJobAsync(this IScheduler scheduler, IJob job,
        MaintenanceAction jobDetails)
    {
        var method = typeof(JobBuilder).GetMethods()
            .First(method => method.ContainsGenericParameters && !method.GetParameters().Any());

        var generic = method.MakeGenericMethod(job.GetType());

        var quartzJob = ((JobBuilder) generic.Invoke(scheduler, null))
            .WithIdentity(jobDetails.Name, "Maintenance").Build();

        var cronTrigger = new CronTriggerImpl
        {
            CronExpression = new CronExpression(jobDetails.Schedule),
            TimeZone = TimeZoneInfo.Utc,
            JobName = jobDetails.Name,
            JobGroup = "Maintenance",
            Name = jobDetails.Name,
            Group = "Maintenance"
        };

        await scheduler.ScheduleJob(quartzJob, cronTrigger);
    }
}