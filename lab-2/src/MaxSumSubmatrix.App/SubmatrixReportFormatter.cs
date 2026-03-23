using System.Globalization;
using System.Text;

namespace MaxSumSubmatrix;

/// <summary>
/// Builds a human-readable report about the maximum-sum submatrix.
/// </summary>
public static class SubmatrixReportFormatter
{
    public static string Format(MatrixData matrix, SubmatrixResult result)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        ArgumentNullException.ThrowIfNull(result);

        StringBuilder builder = new();
        builder.AppendLine("Результат анализа матрицы:");
        builder.AppendLine($"Размер исходной матрицы: {matrix.RowCount} x {matrix.ColumnCount}");
        builder.AppendLine($"Максимальная сумма подматрицы: {result.Sum.ToString("G17", CultureInfo.InvariantCulture)}");
        builder.AppendLine(
            $"Координаты верхнего левого угла (строка, столбец): ({result.TopRow + 1}, {result.LeftColumn + 1})");
        builder.AppendLine(
            $"Координаты нижнего правого угла (строка, столбец): ({result.BottomRow + 1}, {result.RightColumn + 1})");
        builder.AppendLine($"Размер найденной подматрицы: {result.Height} x {result.Width}");
        builder.AppendLine("Элементы найденной подматрицы:");

        for (int rowIndex = result.TopRow; rowIndex <= result.BottomRow; rowIndex++)
        {
            for (int columnIndex = result.LeftColumn; columnIndex <= result.RightColumn; columnIndex++)
            {
                if (columnIndex > result.LeftColumn)
                {
                    builder.Append(' ');
                }

                builder.Append(matrix.GetValue(rowIndex, columnIndex).ToString("G17", CultureInfo.InvariantCulture));
            }

            builder.AppendLine();
        }

        return builder.ToString().TrimEnd();
    }
}
