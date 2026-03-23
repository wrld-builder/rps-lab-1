namespace CircleIntersection;

/// <summary>
/// Вычисляет площадь пересечения двух окружностей на плоскости.
/// Автор: Михаил Шунин (учебное задание).
/// </summary>
public static class CircleIntersectionCalculator
{
    private const double Epsilon = 1e-9;

    /// <summary>
    /// Вычисляет площадь пересечения двух окружностей. Радиусы должны быть положительными.
    /// </summary>
    /// <param name="centerX1">Координата X центра первой окружности.</param>
    /// <param name="centerY1">Координата Y центра первой окружности.</param>
    /// <param name="radius1">Радиус первой окружности.</param>
    /// <param name="centerX2">Координата X центра второй окружности.</param>
    /// <param name="centerY2">Координата Y центра второй окружности.</param>
    /// <param name="radius2">Радиус второй окружности.</param>
    /// <returns>Площадь пересечения (ноль, если окружности только касаются или не пересекаются).</returns>
    public static double ComputeIntersectionArea(
        double centerX1,
        double centerY1,
        double radius1,
        double centerX2,
        double centerY2,
        double radius2)
    {
        if (radius1 <= 0 || radius2 <= 0)
        {
            throw new ArgumentOutOfRangeException(
                radius1 <= 0 ? nameof(radius1) : nameof(radius2),
                "Радиусы окружностей должны быть положительными.");
        }

        double dx = centerX2 - centerX1;
        double dy = centerY2 - centerY1;
        double distance = Math.Sqrt(dx * dx + dy * dy);

        if (distance > radius1 + radius2 + Epsilon)
        {
            return 0;
        }

        if (distance <= Math.Abs(radius1 - radius2) + Epsilon)
        {
            double smallerRadius = Math.Min(radius1, radius2);
            return Math.PI * smallerRadius * smallerRadius;
        }

        return ComputeLensArea(distance, radius1, radius2);
    }

    /// <summary>
    /// Возвращает true, если два круга имеют общую область положительной площади.
    /// </summary>
    public static bool HaveOverlappingArea(
        double centerX1,
        double centerY1,
        double radius1,
        double centerX2,
        double centerY2,
        double radius2)
    {
        double area = ComputeIntersectionArea(
            centerX1,
            centerY1,
            radius1,
            centerX2,
            centerY2,
            radius2);
        return area > Epsilon;
    }

    private static double ComputeLensArea(double distance, double radius1, double radius2)
    {
        double cos1 = (distance * distance + radius1 * radius1 - radius2 * radius2)
            / (2 * distance * radius1);
        double cos2 = (distance * distance + radius2 * radius2 - radius1 * radius1)
            / (2 * distance * radius2);

        cos1 = ClampCosineArgument(cos1);
        cos2 = ClampCosineArgument(cos2);

        double term1 = radius1 * radius1 * Math.Acos(cos1);
        double term2 = radius2 * radius2 * Math.Acos(cos2);

        double radicalInner = (-distance + radius1 + radius2)
            * (distance + radius1 - radius2)
            * (distance - radius1 + radius2)
            * (distance + radius1 + radius2);

        if (radicalInner < 0 && radicalInner > -Epsilon)
        {
            radicalInner = 0;
        }

        double term3 = 0.5 * Math.Sqrt(Math.Max(0, radicalInner));

        return term1 + term2 - term3;
    }

    private static double ClampCosineArgument(double value)
    {
        if (value > 1)
        {
            return 1;
        }

        if (value < -1)
        {
            return -1;
        }

        return value;
    }
}
