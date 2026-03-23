using System.Globalization;

namespace CircleIntersection;

/// <summary>
/// Загружает и сохраняет данные окружностей в текстовых файлах (одна окружность на строку: cx cy r).
/// </summary>
public static class CircleFileService
{
    private const int ExpectedTokenCount = 3;

    /// <summary>
    /// Считывает две окружности из файла: первая строка — первая окружность, вторая строка — вторая.
    /// </summary>
    public static (CircleData First, CircleData Second) LoadTwoCircles(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Файл с входными данными не найден.", filePath);
        }

        string[] lines = File.ReadAllLines(filePath);
        if (lines.Length < 2)
        {
            throw new InvalidDataException("В файле должно быть не менее двух строк (две окружности).");
        }

        CircleData first = ParseCircleLine(lines[0], 1);
        CircleData second = ParseCircleLine(lines[1], 2);
        return (first, second);
    }

    /// <summary>
    /// Сохраняет результат анализа в текстовый файл.
    /// </summary>
    public static void SaveResult(string filePath, string content)
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

        File.WriteAllText(filePath, content);
    }

    private static CircleData ParseCircleLine(string line, int lineNumber)
    {
        string trimmed = line.Trim();
        if (trimmed.Length == 0)
        {
            throw new InvalidDataException($"Строка {lineNumber} пуста.");
        }

        string[] parts = trimmed.Split(
            new[] { ' ', '\t' },
            StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != ExpectedTokenCount)
        {
            throw new InvalidDataException(
                $"Строка {lineNumber}: ожидаются ровно три числа (X, Y центра и радиус).");
        }

        if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double cx))
        {
            throw new InvalidDataException($"Строка {lineNumber}: неверное число для координаты X центра.");
        }

        if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double cy))
        {
            throw new InvalidDataException($"Строка {lineNumber}: неверное число для координаты Y центра.");
        }

        if (!double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double r))
        {
            throw new InvalidDataException($"Строка {lineNumber}: неверное число для радиуса.");
        }

        return new CircleData(cx, cy, r);
    }
}
