namespace ChainLine;

/// <summary>
/// Stores the input data required to evaluate the chain line.
/// </summary>
public sealed class FunctionParameters
{
    public FunctionParameters(double leftBoundary, double rightBoundary, double step, double coefficientA)
    {
        if (step <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(step), "Шаг должен быть положительным.");
        }

        if (leftBoundary > rightBoundary)
        {
            throw new ArgumentException("Левая граница не может быть больше правой.");
        }

        if (coefficientA == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(coefficientA), "Коэффициент a не может быть равен нулю.");
        }

        LeftBoundary = leftBoundary;
        RightBoundary = rightBoundary;
        Step = step;
        CoefficientA = coefficientA;
    }

    public double LeftBoundary { get; }

    public double RightBoundary { get; }

    public double Step { get; }

    public double CoefficientA { get; }
}
