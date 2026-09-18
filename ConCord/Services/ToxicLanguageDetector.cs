using System;
using ConCord.ML;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;

namespace ConCord.Services;

public class ToxicLanguageDetector : IToxicLanguageDetector
{
    private readonly PredictionEnginePool<ToxicDetector.ModelInput, ToxicDetector.ModelOutput> _pool;
    private readonly ILogger<ToxicLanguageDetector> _logger;
    private readonly int _toxicLabelIndex;

    public ToxicLanguageDetector(
        PredictionEnginePool<ToxicDetector.ModelInput, ToxicDetector.ModelOutput> pool,
        ILogger<ToxicLanguageDetector> logger)
    {
        _pool = pool;
        _logger = logger;

        Microsoft.ML.PredictionEngine<ToxicDetector.ModelInput, ToxicDetector.ModelOutput>? predEngine = null;
        try
        {
            predEngine = _pool.GetPredictionEngine(ToxicDetector.ModelName);
            var labels = ToxicDetector.ExtractLabels(predEngine.OutputSchema);
            var toxicIndex = Array.IndexOf(labels, "1");
            _toxicLabelIndex = toxicIndex >= 0 ? toxicIndex : 1;
            _logger.LogInformation("Initialized ToxicLanguageDetector. Toxic label index: {Index}, Labels: [{Labels}]",
                _toxicLabelIndex, string.Join(", ", labels));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not extract model label schema at startup. Fallback index 1 will be used.");
            _toxicLabelIndex = 1;
        }
        finally
        {
            if (predEngine != null)
            {
                _pool.ReturnPredictionEngine(ToxicDetector.ModelName, predEngine);
            }
        }
    }

    public ToxicityResult CheckToxicity(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return new ToxicityResult
            {
                IsToxic = false,
                ToxicityScore = 0f,
                FlaggedText = text
            };
        }

        try
        {
            var input = new ToxicDetector.ModelInput { Text = text };
            var output = _pool.Predict(ToxicDetector.ModelName, input);

            bool isToxic = output.PredictedLabel == "1";
            float toxicScore = 0f;
            if (_toxicLabelIndex >= 0 && output.Score != null && output.Score.Length > _toxicLabelIndex)
            {
                toxicScore = output.Score[_toxicLabelIndex];
            }

            if (isToxic)
            {
                _logger.LogInformation("Toxic content detected. Score: {Score}", toxicScore);
            }

            return new ToxicityResult
            {
                IsToxic = isToxic,
                ToxicityScore = toxicScore,
                FlaggedText = text
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred evaluating text toxicity. Message will be permitted.");
            return new ToxicityResult
            {
                IsToxic = false,
                ToxicityScore = 0f,
                FlaggedText = text
            };
        }
    }
}

