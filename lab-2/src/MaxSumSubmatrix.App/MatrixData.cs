namespace MaxSumSubmatrix;

/// <summary>
/// Хранит прямоугольную матрицу вещественных чисел, используемую программой.
/// </summary>
public sealed class MatrixData
{
    private readonly double[,] _values;

    public MatrixData(double[,] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        int rowCount = values.GetLength(0);
        int columnCount = values.GetLength(1);
        if (rowCount == 0 || columnCount == 0)
        {
            throw new ArgumentException("Матрица должна содержать хотя бы одну строку и один столбец.", nameof(values));
        }

        _values = (double[,])values.Clone();
        RowCount = rowCount;
        ColumnCount = columnCount;
    }

    public int RowCount { get; }

    public int ColumnCount { get; }

    public double GetValue(int rowIndex, int columnIndex)
    {
        return _values[rowIndex, columnIndex];
    }

    public double[,] ToArray()
    {
        return (double[,])_values.Clone();
    }
}
