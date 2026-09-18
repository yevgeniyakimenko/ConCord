namespace ConCord.Services;

public class ToxicityResult
{
    public bool IsToxic { get; set; }
    public float ToxicityScore { get; set; }
    public string? FlaggedText { get; set; }
}

public interface IToxicLanguageDetector
{
    ToxicityResult CheckToxicity(string text);
}
