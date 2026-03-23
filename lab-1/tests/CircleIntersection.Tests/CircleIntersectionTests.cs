using CircleIntersection;
using Xunit;

namespace CircleIntersection.Tests;

public class CircleIntersectionTests
{
    [Fact]
    public void DisjointCircles_ZeroArea()
    {
        double area = CircleIntersectionCalculator.ComputeIntersectionArea(
            0, 0, 1,
            5, 0, 1);
        Assert.Equal(0, area, 12);
    }

    [Fact]
    public void IdenticalCircles_FullDiskArea()
    {
        const double radius = 3;
        double expected = Math.PI * radius * radius;
        double area = CircleIntersectionCalculator.ComputeIntersectionArea(
            1, 1, radius,
            1, 1, radius);
        Assert.Equal(expected, area, 10);
    }

    [Fact]
    public void OneCircleInsideAnother_SmallerDiskArea()
    {
        double area = CircleIntersectionCalculator.ComputeIntersectionArea(
            0, 0, 5,
            1, 0, 1);
        Assert.Equal(Math.PI, area, 10);
    }

    [Fact]
    public void TwoUnitCirclesDistanceOne_KnownLensArea()
    {
        const double radius = 1;
        const double distance = 1;
        double expected = 2 * radius * radius * Math.Acos(distance / (2 * radius))
            - 0.5 * distance * Math.Sqrt(4 * radius * radius - distance * distance);
        double area = CircleIntersectionCalculator.ComputeIntersectionArea(
            0, 0, radius,
            distance, 0, radius);
        Assert.Equal(expected, area, 10);
    }

    [Fact]
    public void ExternalTangent_ZeroArea()
    {
        double area = CircleIntersectionCalculator.ComputeIntersectionArea(
            0, 0, 1,
            2, 0, 1);
        Assert.Equal(0, area, 9);
    }

    [Fact]
    public void NegativeRadius_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            CircleIntersectionCalculator.ComputeIntersectionArea(0, 0, -1, 0, 0, 1));
    }

    [Fact]
    public void HaveOverlappingArea_TrueWhenPartialOverlap()
    {
        bool overlap = CircleIntersectionCalculator.HaveOverlappingArea(
            0, 0, 2,
            1, 0, 2);
        Assert.True(overlap);
    }
}
