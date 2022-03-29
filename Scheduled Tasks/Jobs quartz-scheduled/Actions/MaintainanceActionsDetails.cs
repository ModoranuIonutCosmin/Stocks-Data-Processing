using System;

namespace Stocks_Data_Processing.Actions;

public class MaintainanceActionsDetails
{
    public string Name { get; set; }
    public string Schedule { get; set; }
    public long? Period { get; set; }
    public DateTimeOffset LastRun { get; set; }
}