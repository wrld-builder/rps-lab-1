namespace CircleIntersection;

/// <summary>
/// Представляет окружность на декартовой плоскости.
/// </summary>
public readonly struct CircleData
{
    public CircleData(double centerX, double centerY, double radius)
    {
        CenterX = centerX;
        CenterY = centerY;
        Radius = radius;
    }

    public double CenterX { get; }
    public double CenterY { get; }
    public double Radius { get; }
}
