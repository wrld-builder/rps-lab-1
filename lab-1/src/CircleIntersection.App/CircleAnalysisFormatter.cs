using System.Globalization;
using System.Text;

namespace CircleIntersection;

/// <summary>
/// Формирует понятные пользователю отчёты по анализу пересечения окружностей (независимо от интерфейса).
/// </summary>
public static class CircleAnalysisFormatter
{
    public static string FormatAnalysis(
        CircleData circle1,
        CircleData circle2,
        double intersectionArea)
    {
        bool overlap = intersectionArea > 1e-9;
        var builder = new StringBuilder();
        builder.AppendLine(CultureInfo.InvariantCulture, $"Окружность 1: центр ({circle1.CenterX}; {circle1.CenterY}), радиус {circle1.Radius}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"Окружность 2: центр ({circle2.CenterX}; {circle2.CenterY}), радиус {circle2.Radius}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"Есть общая область положительной площади: {overlap}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"Площадь пересечения: {intersectionArea:R}");
        return builder.ToString();
    }
}
