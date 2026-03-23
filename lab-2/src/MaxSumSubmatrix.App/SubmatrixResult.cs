namespace MaxSumSubmatrix;

/// <summary>
/// Описывает прямоугольную подматрицу с максимальной суммой и её координаты.
/// </summary>
public sealed class SubmatrixResult
{
    public SubmatrixResult(int topRow, int leftColumn, int bottomRow, int rightColumn, double sum)
    {
        TopRow = topRow;
        LeftColumn = leftColumn;
        BottomRow = bottomRow;
        RightColumn = rightColumn;
        Sum = sum;
    }

    public int TopRow { get; }

    public int LeftColumn { get; }

    public int BottomRow { get; }

    public int RightColumn { get; }

    public double Sum { get; }

    public int Height => BottomRow - TopRow + 1;

    public int Width => RightColumn - LeftColumn + 1;
}
