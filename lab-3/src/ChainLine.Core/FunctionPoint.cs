namespace ChainLine;

/// <summary>
/// Represents one calculated point of the function graph.
/// </summary>
public sealed class FunctionPoint
{
    public FunctionPoint(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double X { get; }

    public double Y { get; }
}
