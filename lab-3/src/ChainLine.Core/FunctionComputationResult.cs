namespace ChainLine;

/// <summary>
/// Contains the points, warnings and metadata of one computation run.
/// </summary>
public sealed class FunctionComputationResult
{
    public FunctionComputationResult(
        FunctionParameters parameters,
        IReadOnlyList<FunctionPoint> points,
        bool isDegenerateToPoint,
        string? warningMessage)
    {
        Parameters = parameters;
        Points = points;
        IsDegenerateToPoint = isDegenerateToPoint;
        WarningMessage = warningMessage;
    }

    public FunctionParameters Parameters { get; }

    public IReadOnlyList<FunctionPoint> Points { get; }

    public bool IsDegenerateToPoint { get; }

    public string? WarningMessage { get; }
}
