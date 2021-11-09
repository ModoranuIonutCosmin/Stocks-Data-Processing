using Microsoft.EntityFrameworkCore;
using Stocks.General.ConstantsConfig;
using StocksProccesing.Relational.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StocksProccesing.Relational.DataAccess.V1.Repositories
{
    public class MaintainanceJobsRepository : Repository<MaintenanceAction, int>, IMaintenanceJobsRepository
    {
        public MaintainanceJobsRepository(StocksMarketContext context) : base(context)
        {

        }

        public async Task<MaintenanceAction> GetByTypeAsync(string type)
        {
            return await _dbContext.Actions.SingleOrDefaultAsync(e => e.Type == type);
        }


        
    }
}
