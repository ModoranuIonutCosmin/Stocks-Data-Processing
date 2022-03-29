using System;
using System.Collections.Generic;
using System.Configuration;
using Stocks.General.ExtensionMethods;
using StocksProccesing.Relational.Model;

namespace Stocks_Data_Processing.ConfigHelpers;

public static class ReadTasksConfigHelpers
{
    public static List<MaintenanceAction> GetMaintainanceActions(List<string> tasksName)
    {
        var result = new List<MaintenanceAction>();

        foreach (var task in tasksName)
        {
            var interval = ConfigurationManager.AppSettings.Get(task + ".Interval");
            var schedule = ConfigurationManager.AppSettings.Get(task + ".Schedule");

            result.Add(new MaintenanceAction
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