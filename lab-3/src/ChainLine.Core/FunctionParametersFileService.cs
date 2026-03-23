using System.Globalization;
using System.Text;

namespace ChainLine;

/// <summary>
/// Назначение модуля: загрузка и сохранение исходных параметров для лабораторной работы №3.
/// Автор: Шунин Михаил Дмитриевич.
/// Алгоритм: текстовый формат key-value с разбором чисел в инвариантной культуре.
/// </summary>
public static class FunctionParametersFileService
{
    private const string LeftBoundaryKey = "LeftBoundary";
    private const string RightBoundaryKey = "RightBoundary";
    private const string StepKey = "Step";
    private const string CoefficientAKey = "CoefficientA";

    /// <summary>
    /// Сохраняет исходные параметры в текстовый файл.
    /// Входные данные: абсолютный или относительный путь к файлу и проверенные параметры.
    /// Результат: файл создан или перезаписан.
    /// </summary>
    public static void Save(string filePath, FunctionParameters parameters)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(filePath));
        }

        ArgumentNullException.ThrowIfNull(parameters);

        string? directoryPath = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        StringBuilder builder = new();
        builder.AppendLine($"{LeftBoundaryKey}={parameters.LeftBoundary.ToString("G17", CultureInfo.InvariantCulture)}");
        builder.AppendLine($"{RightBoundaryKey}={parameters.RightBoundary.ToString("G17", CultureInfo.InvariantCulture)}");
        builder.AppendLine($"{StepKey}={parameters.Step.ToString("G17", CultureInfo.InvariantCulture)}");
        builder.AppendLine($"{CoefficientAKey}={parameters.CoefficientA.ToString("G17", CultureInfo.InvariantCulture)}");

        File.WriteAllText(filePath, builder.ToString(), Encoding.UTF8);
    }

    /// <summary>
    /// Загружает исходные параметры из текстового файла.
    /// Входные данные: путь к файлу в формате key-value.
    /// Результат: проверенные параметры функции, готовые к вычислению.
    /// </summary>
    public static FunctionParameters Load(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Файл с параметрами не найден.", filePath);
        }

        Dictionary<string, string> values = File.ReadAllLines(filePath)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(ParseLine)
            .ToDictionary(item => item.Key, item => item.Value, StringComparer.OrdinalIgnoreCase);

        return new FunctionParameters(
            ParseDouble(values, LeftBoundaryKey),
            ParseDouble(values, RightBoundaryKey),
            ParseDouble(values, StepKey),
            ParseDouble(values, CoefficientAKey));
    }

    private static KeyValuePair<string, string> ParseLine(string line)
    {
        int separatorIndex = line.IndexOf('=');
        if (separatorIndex <= 0 || separatorIndex == line.Length - 1)
        {
            throw new InvalidDataException("Каждая строка файла параметров должна иметь вид key=value.");
        }

        string key = line[..separatorIndex].Trim();
        string value = line[(separatorIndex + 1)..].Trim();
        if (key.Length == 0 || value.Length == 0)
        {
            throw new InvalidDataException("Ключ и значение в файле параметров не могут быть пустыми.");
        }

        return new KeyValuePair<string, string>(key, value);
    }

    private static double ParseDouble(IReadOnlyDictionary<string, string> values, string key)
    {
        if (!values.TryGetValue(key, out string? rawValue))
        {
            throw new InvalidDataException($"В файле параметров отсутствует обязательное поле {key}.");
        }

        if (!double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedValue))
        {
            throw new InvalidDataException($"Поле {key} содержит некорректное вещественное число.");
        }

        return parsedValue;
    }
}
