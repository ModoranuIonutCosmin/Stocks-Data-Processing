using System.Collections.Generic;

namespace StocksProcessing.ML.Models;

public class SeparatedDataset<TM>
{
    public IEnumerable<TM> TrainData { get; set; }
    public IEnumerable<TM> TestData { get; set; }
}