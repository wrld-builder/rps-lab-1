namespace ChainLine;

/// <summary>
/// Представляет одну вычисленную точку графика функции.
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
