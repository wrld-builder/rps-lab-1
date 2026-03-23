using ChainLine;
using Xunit;

namespace ChainLine.Tests;

public sealed class ExcelExportServiceTests
{
    [Fact]
    public void Export_CreatesExcelFile()
    {
        FunctionParameters parameters = new(-1, 1, 1, 2);
        FunctionComputationResult result = ChainLineCalculator.Compute(parameters);
        string filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xlsx");

        try
        {
            ExcelExportService.Export(filePath, result);

            Assert.True(File.Exists(filePath));
            Assert.True(new FileInfo(filePath).Length > 0);
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
