using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.IO;
using System.Linq;

namespace ConCord.ML;

public static class ToxicDetector
{
    public const string ModelName = "ToxicDetector";

    public class ModelInput
    {
        [LoadColumn(0)]
        [ColumnName(@"text")]
        public string Text { get; set; } = string.Empty;

        [LoadColumn(1)]
        [ColumnName(@"Y")]
        public string Y { get; set; } = string.Empty;
    }

    public class ModelOutput
    {
        [ColumnName(@"PredictedLabel")]
        public string PredictedLabel { get; set; } = string.Empty;

        [ColumnName(@"Score")]
        public float[] Score { get; set; } = Array.Empty<float>();
    }

    public static string ResolveModelPath(string? contentRootPath = null)
    {
        var modelFileNames = new[] { "ToxicDetector.mlnet", "ToxicModel.mlnet" };

        foreach (var fileName in modelFileNames)
        {
            var candidates = new[]
            {
                !string.IsNullOrWhiteSpace(contentRootPath) ? Path.Combine(contentRootPath, fileName) : null,
                Path.Combine(AppContext.BaseDirectory, fileName),
                Path.Combine(Directory.GetCurrentDirectory(), fileName),
                Path.Combine(Directory.GetCurrentDirectory(), "ConCord", fileName),
                Path.GetFullPath(fileName)
            };

            foreach (var candidate in candidates)
            {
                if (!string.IsNullOrWhiteSpace(candidate) && File.Exists(candidate))
                {
                    return candidate;
                }
            }
        }

        return Path.Combine(AppContext.BaseDirectory, "ToxicDetector.mlnet");
    }

    public static string[] ExtractLabels(DataViewSchema schema)
    {
        var labelColumn = schema.GetColumnOrNull("Y");
        if (labelColumn == null)
        {
            return Array.Empty<string>();
        }

        var keyNames = new VBuffer<ReadOnlyMemory<char>>();
        labelColumn.Value.GetKeyValues(ref keyNames);
        return keyNames.DenseValues().Select(x => x.ToString()).ToArray();
    }
}
