using ChainLine;
using Xunit;

namespace ChainLine.Tests;

public sealed class FunctionParametersFileServiceTests
{
    [Fact]
    public void SaveAndLoad_RoundTripPreservesParameters()
    {
        FunctionParameters parameters = new(-4.5, 8.25, 0.125, 3.5);
        string filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.txt");

        try
        {
            FunctionParametersFileService.Save(filePath, parameters);
            FunctionParameters restored = FunctionParametersFileService.Load(filePath);

            Assert.Equal(parameters.LeftBoundary, restored.LeftBoundary, 12);
            Assert.Equal(parameters.RightBoundary, restored.RightBoundary, 12);
            Assert.Equal(parameters.Step, restored.Step, 12);
            Assert.Equal(parameters.CoefficientA, restored.CoefficientA, 12);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
