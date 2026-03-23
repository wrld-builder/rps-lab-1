using ChainLine;
using Xunit;

namespace ChainLine.Tests;

public sealed class ChainLineCalculatorTests
{
    [Fact]
    public void Evaluate_WithPositiveA_ReturnsExpectedValue()
    {
        double value = ChainLineCalculator.Evaluate(2, 0);

        Assert.Equal(2, value, 12);
    }

    [Fact]
    public void Compute_OnTypicalInterval_ReturnsBothBounds()
    {
        FunctionParameters parameters = new(-2, 2, 1, 2);

        FunctionComputationResult result = ChainLineCalculator.Compute(parameters);

        Assert.Equal(5, result.Points.Count);
        Assert.Equal(-2, result.Points.First().X, 12);
        Assert.Equal(2, result.Points.Last().X, 12);
        Assert.False(result.IsDegenerateToPoint);
    }

    [Fact]
    public void Compute_WithSinglePointInterval_ProducesDegenerateWarning()
    {
        FunctionParameters parameters = new(1, 1, 0.5, 2);

        FunctionComputationResult result = ChainLineCalculator.Compute(parameters);

        Assert.Single(result.Points);
        Assert.True(result.IsDegenerateToPoint);
        Assert.Contains("вырождается", result.WarningMessage);
    }

    [Fact]
    public void Constructor_WithZeroCoefficient_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new FunctionParameters(-1, 1, 0.5, 0));
    }
}
