using System.Globalization;
using System.Text;

namespace MaxSumSubmatrix;

/// <summary>
/// Loads and saves matrices in text form.
/// File format: first line contains "rows columns", next lines contain matrix rows.
/// </summary>
public static class MatrixFileService
{
    private const int SizeTokenCount = 2;

    public static MatrixData LoadMatrix(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Файл с матрицей не найден.", filePath);
        }

        string[] nonEmptyLines = File.ReadAllLines(filePath)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();

        if (nonEmptyLines.Length < 2)
        {
            throw new InvalidDataException("Файл должен содержать размеры матрицы и хотя бы одну строку данных.");
        }

        (int rowCount, int columnCount) = ParseMatrixSize(nonEmptyLines[0]);
        if (nonEmptyLines.Length != rowCount + 1)
        {
            throw new InvalidDataException("Количество строк данных не совпадает с указанным числом строк матрицы.");
        }

        double[,] values = new double[rowCount, columnCount];
        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            string[] parts = SplitLine(nonEmptyLines[rowIndex + 1]);
            if (parts.Length != columnCount)
            {
                throw new InvalidDataException(
                    $"Строка {rowIndex + 2}: ожидалось {columnCount} элементов, получено {parts.Length}.");
            }

            for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {
                if (!double.TryParse(parts[columnIndex], NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                {
                    throw new InvalidDataException(
                        $"Строка {rowIndex + 2}, столбец {columnIndex + 1}: некорректное вещественное число.");
                }

                values[rowIndex, columnIndex] = value;
            }
        }

        return new MatrixData(values);
    }

    public static void SaveMatrix(string filePath, MatrixData matrix)
    {
        ArgumentNullException.ThrowIfNull(matrix);

        SaveText(filePath, BuildMatrixText(matrix));
    }

    public static void SaveText(string filePath, string content)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(filePath));
        }

        string? directoryPath = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        File.WriteAllText(filePath, content, Encoding.UTF8);
    }

    private static string BuildMatrixText(MatrixData matrix)
    {
        StringBuilder builder = new();
        builder.Append(matrix.RowCount);
        builder.Append(' ');
        builder.Append(matrix.ColumnCount);
        builder.AppendLine();

        for (int rowIndex = 0; rowIndex < matrix.RowCount; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < matrix.ColumnCount; columnIndex++)
            {
                if (columnIndex > 0)
                {
                    builder.Append(' ');
                }

                builder.Append(matrix.GetValue(rowIndex, columnIndex).ToString("G17", CultureInfo.InvariantCulture));
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }

    private static (int RowCount, int ColumnCount) ParseMatrixSize(string line)
    {
        string[] parts = SplitLine(line);
        if (parts.Length != SizeTokenCount)
        {
            throw new InvalidDataException("Первая строка файла должна содержать два целых числа: число строк и столбцов.");
        }

        if (!int.TryParse(parts[0], out int rowCount) || rowCount <= 0)
        {
            throw new InvalidDataException("Число строк матрицы должно быть положительным целым числом.");
        }

        if (!int.TryParse(parts[1], out int columnCount) || columnCount <= 0)
        {
            throw new InvalidDataException("Число столбцов матрицы должно быть положительным целым числом.");
        }

        return (rowCount, columnCount);
    }

    private static string[] SplitLine(string line)
    {
        return line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
    }
}
