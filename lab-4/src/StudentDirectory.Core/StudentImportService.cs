using System.Text;

namespace StudentDirectory;

/// <summary>
/// Назначение модуля: импорт записей о студентах из CSV-файлов для лабораторной работы №4.
/// Автор: Шунин Михаил Дмитриевич.
/// Алгоритм: разбор CSV с учётом заголовка и поддержкой значений в кавычках.
/// </summary>
public sealed class StudentImportService : IStudentImportService
{
    private const string ExpectedHeader = "Id,FullName,GroupName,Faculty,EnrollmentYear,Notes";
    private const int MinimumColumnCount = 6;

    /// <summary>
    /// Загружает записи о студентах из CSV-файла.
    /// Входные данные: путь к CSV-файлу в формате экспорта.
    /// Результат: последовательность моделей ввода студентов, готовых к проверке и сохранению.
    /// </summary>
    public async Task<IReadOnlyList<StudentRecordInput>> ImportAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Путь к файлу импорта не может быть пустым.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Файл для импорта не найден.", filePath);
        }

        cancellationToken.ThrowIfCancellationRequested();
        string[] lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8, cancellationToken).ConfigureAwait(false);
        if (lines.Length == 0)
        {
            throw new InvalidDataException("Файл импорта пуст.");
        }

        if (!string.Equals(lines[0].Trim(), ExpectedHeader, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException("Файл импорта имеет неверный заголовок.");
        }

        List<StudentRecordInput> importedStudents = new();
        for (int index = 1; index < lines.Length; index++)
        {
            string line = lines[index];
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            string[] columns = ParseCsvLine(line);
            if (columns.Length < MinimumColumnCount)
            {
                throw new InvalidDataException($"Строка {index + 1} содержит недостаточно столбцов.");
            }

            if (!int.TryParse(columns[4], out int enrollmentYear))
            {
                throw new InvalidDataException($"Строка {index + 1}: некорректный год поступления.");
            }

            importedStudents.Add(new StudentRecordInput
            {
                FullName = columns[1],
                GroupName = columns[2],
                Faculty = columns[3],
                EnrollmentYear = enrollmentYear,
                Notes = columns[5],
            });
        }

        return importedStudents;
    }

    private static string[] ParseCsvLine(string line)
    {
        List<string> values = new();
        StringBuilder currentValue = new();
        bool insideQuotes = false;

        for (int index = 0; index < line.Length; index++)
        {
            char currentChar = line[index];

            if (currentChar == '"')
            {
                if (insideQuotes && index + 1 < line.Length && line[index + 1] == '"')
                {
                    currentValue.Append('"');
                    index++;
                }
                else
                {
                    insideQuotes = !insideQuotes;
                }

                continue;
            }

            if (currentChar == ',' && !insideQuotes)
            {
                values.Add(currentValue.ToString());
                currentValue.Clear();
                continue;
            }

            currentValue.Append(currentChar);
        }

        values.Add(currentValue.ToString());
        return values.ToArray();
    }
}
