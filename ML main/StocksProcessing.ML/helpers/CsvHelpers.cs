using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksProcessing.ML;

public static class CsvHelpers
{
    public static async Task WriteDatasetToFile(this List<List<float>> dataset, string destinationPath)
    {
        var lines = dataset.Select(datapoint =>
        {
            return $"{string.Join(", ", datapoint.Select(val => val.ToString(CultureInfo.InvariantCulture)))}";
        }).ToList();

        var header = new StringBuilder();

        for (var feature = 0; feature < dataset[0]?.Count; feature++)
        {
            header.Append($"line__{feature}");

            if (feature < lines.Count - 1) header.Append(", ");
        }

        header.Append(Environment.NewLine);
        File.WriteAllText(destinationPath, header.ToString());

        await File.AppendAllLinesAsync(destinationPath, lines);
    }
}