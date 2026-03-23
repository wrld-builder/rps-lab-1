namespace ChainLine;

/// <summary>
/// Calculates values of the chain line y = a / 2 * (e^(x / a) + e^(-x / a)).
/// </summary>
public static class ChainLineCalculator
{
    private const double Epsilon = 1e-9;

    public static FunctionComputationResult Compute(FunctionParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        List<FunctionPoint> points = BuildPoints(parameters);
        if (points.Count == 0)
        {
            throw new InvalidOperationException(
                "На заданном интервале не удалось построить график функции. Попробуйте изменить границы построения.");
        }

        bool isDegenerateToPoint = points.Count == 1;
        string? warningMessage = null;

        if (isDegenerateToPoint)
        {
            warningMessage = "График вырождается в точку. Увеличьте интервал или уменьшите шаг построения.";
        }

        return new FunctionComputationResult(parameters, points, isDegenerateToPoint, warningMessage);
    }

    private static List<FunctionPoint> BuildPoints(FunctionParameters parameters)
    {
        List<FunctionPoint> points = new();
        double currentX = parameters.LeftBoundary;

        while (currentX <= parameters.RightBoundary + Epsilon)
        {
            double y = Evaluate(parameters.CoefficientA, currentX);
            if (double.IsFinite(y))
            {
                points.Add(new FunctionPoint(currentX, y));
            }

            double nextX = currentX + parameters.Step;
            if (nextX <= currentX + Epsilon)
            {
                break;
            }

            currentX = nextX;
        }

        double distanceToRight = Math.Abs(points.Count > 0 ? points[^1].X - parameters.RightBoundary : double.MaxValue);
        if (distanceToRight > Epsilon)
        {
            double rightY = Evaluate(parameters.CoefficientA, parameters.RightBoundary);
            if (double.IsFinite(rightY))
            {
                points.Add(new FunctionPoint(parameters.RightBoundary, rightY));
            }
        }

        return points
            .GroupBy(point => Math.Round(point.X, 10))
            .Select(group => group.First())
            .OrderBy(point => point.X)
            .ToList();
    }

    /// <summary>
    /// Evaluates the chain line function at a specified point.
    /// </summary>
    public static double Evaluate(double coefficientA, double x)
    {
        if (coefficientA == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(coefficientA), "Коэффициент a не может быть равен нулю.");
        }

        return coefficientA * Math.Cosh(x / coefficientA);
    }
}
