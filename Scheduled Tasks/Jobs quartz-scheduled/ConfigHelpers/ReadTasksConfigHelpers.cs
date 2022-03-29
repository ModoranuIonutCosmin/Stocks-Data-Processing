using Stocks.General.ExtensionMethods;
using Stocks_Data_Processing.Actions;
using StocksProccesing.Relational.Model;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Stocks_Data_Processing.ConfigHelpers
{
    public static class ReadTasksConfigHelpers
    {
        public static List<MaintenanceAction> GetMaintainanceActions(List<string> tasksName)
        {
            var result = new List<MaintenanceAction>();

            foreach(var task in tasksName)
            {
                string interval = ConfigurationManager.AppSettings.Get(task + ".Interval");
                string schedule = ConfigurationManager.AppSettings.Get(task + ".Schedule");

                result.Add(new MaintenanceAction()
                {
                    Interval = TimespanParser.ParseTimeSpanTicks(interval),
                    Schedule = schedule,
                    Name = task,
                    LastFinishedDate = DateTimeOffset.MinValue
                });
            }

            return result;
        }
    }
}
