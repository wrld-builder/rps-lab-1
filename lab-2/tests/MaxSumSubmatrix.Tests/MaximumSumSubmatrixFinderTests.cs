using MaxSumSubmatrix;
using Xunit;

namespace MaxSumSubmatrix.Tests;

public sealed class MaximumSumSubmatrixFinderTests
{
    [Fact]
    public void Find_SingleElementMatrix_ReturnsThatElement()
    {
        MatrixData matrix = new(new[,] { { 42.5 } });

        SubmatrixResult result = MaximumSumSubmatrixFinder.Find(matrix);

        Assert.Equal(42.5, result.Sum, 10);
        Assert.Equal(0, result.TopRow);
        Assert.Equal(0, result.LeftColumn);
        Assert.Equal(0, result.BottomRow);
        Assert.Equal(0, result.RightColumn);
    }

    [Fact]
    public void Find_KnownExample_ReturnsExpectedRectangleAndSum()
    {
        MatrixData matrix = new(new[,]
        {
            { 1.0, 2.0, -1.0, -4.0, -20.0 },
            { -8.0, -3.0, 4.0, 2.0, 1.0 },
            { 3.0, 8.0, 10.0, 1.0, 3.0 },
            { -4.0, -1.0, 1.0, 7.0, -6.0 },
        });

        SubmatrixResult result = MaximumSumSubmatrixFinder.Find(matrix);

        Assert.Equal(29.0, result.Sum, 10);
        Assert.Equal(1, result.TopRow);
        Assert.Equal(1, result.LeftColumn);
        Assert.Equal(3, result.BottomRow);
        Assert.Equal(3, result.RightColumn);
    }

    [Fact]
    public void Find_AllNegativeValues_ReturnsLargestElement()
    {
        MatrixData matrix = new(new[,]
        {
            { -5.0, -9.0 },
            { -2.0, -7.0 },
        });

        SubmatrixResult result = MaximumSumSubmatrixFinder.Find(matrix);

        Assert.Equal(-2.0, result.Sum, 10);
        Assert.Equal(1, result.TopRow);
        Assert.Equal(0, result.LeftColumn);
        Assert.Equal(1, result.BottomRow);
        Assert.Equal(0, result.RightColumn);
    }

    [Fact]
    public void SaveAndLoadMatrix_RoundTripPreservesValues()
    {
        MatrixData matrix = new(new[,]
        {
            { 1.25, -2.5, 3.75 },
            { 4.5, 5.0, -6.125 },
        });

        string filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.txt");

        try
        {
            MatrixFileService.SaveMatrix(filePath, matrix);
            MatrixData restored = MatrixFileService.LoadMatrix(filePath);

            Assert.Equal(matrix.RowCount, restored.RowCount);
            Assert.Equal(matrix.ColumnCount, restored.ColumnCount);
            for (int rowIndex = 0; rowIndex < matrix.RowCount; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < matrix.ColumnCount; columnIndex++)
                {
                    Assert.Equal(matrix.GetValue(rowIndex, columnIndex), restored.GetValue(rowIndex, columnIndex), 12);
                }
            }
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public void LoadMatrix_InvalidRowLength_Throws()
    {
        string filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.txt");
        File.WriteAllText(filePath, "2 3\n1 2 3\n4 5\n");

        try
        {
            InvalidDataException exception = Assert.Throws<InvalidDataException>(() =>
                MatrixFileService.LoadMatrix(filePath));

            Assert.Contains("ожидалось 3 элементов", exception.Message);
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
