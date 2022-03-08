using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksProccesing.Relational.Model;

namespace StocksProccesing.Relational.DataAccess.V1
{
    public class MaintainanceJobsRepository : Repository<MaintenanceAction, int>, IMaintainanceJobsRepository
    {
        public MaintainanceJobsRepository(StocksMarketContext context) : base(context)
        {

        }

        public MaintenanceAction GetMaintenanceActionByName(string name)
            => _dbContext.Actions.SingleOrDefault(e => e.Name == name);

        public void MarkJobFinished(string jobName)
        {
            var job = _dbContext.Actions.Where(e => e.Name == jobName)
                .FirstOrDefault();

            if (job != null)
                job.LastFinishedDate = DateTimeOffset.UtcNow;

            _dbContext.SaveChanges();
        }
        

        public async Task<List<MaintenanceAction>> EnsureJobsDataExists(List<MaintenanceAction> jobsData)
        {
            foreach(var job in jobsData)
            {
                var jobDetails = GetMaintenanceActionByName(job.Name);

                if (jobDetails != null)
                {
                    jobDetails.Schedule = job.Schedule;
                    jobDetails.Interval = job.Interval;
                    job.LastFinishedDate = jobDetails.LastFinishedDate;

                    await UpdateAsync(jobDetails);
                }
                else
                {
                    await AddAsync(new MaintenanceAction()
                    {
                        Interval = job.Interval,
                        LastFinishedDate = job.LastFinishedDate,
                        Name = job.Name,
                        Schedule = job.Schedule
                    });
                }
            }

            await _dbContext.SaveChangesAsync();

            return jobsData;
        }
    }
}
