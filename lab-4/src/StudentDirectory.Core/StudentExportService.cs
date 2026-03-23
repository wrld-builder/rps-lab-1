// Назначение модуля: экспорт текущего списка студентов в CSV-файл.
// Автор: Шунин Михаил Дмитриевич.
// Алгоритм: последовательная генерация CSV с текстовыми полями в кавычках.
using System.Text;

namespace StudentDirectory;

/// <summary>
/// Записывает список студентов в CSV-файл.
/// </summary>
public sealed class StudentExportService : IStudentExportService
{
    private const string HeaderLine = "Id,FullName,GroupName,Faculty,EnrollmentYear,Notes";

    /// <summary>
    /// Сохраняет список студентов в CSV-файл, выбранный пользователем.
    /// Входные данные: целевой путь и список студентов в памяти.
    /// Результат: текстовый файл, созданный на диске.
    /// </summary>
    public async Task ExportAsync(string filePath, IReadOnlyList<StudentRecord> students, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Путь к файлу экспорта не может быть пустым.", nameof(filePath));
        }

        ArgumentNullException.ThrowIfNull(students);
        cancellationToken.ThrowIfCancellationRequested();

        string? directoryPath = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        StringBuilder builder = new();
        builder.AppendLine(HeaderLine);

        foreach (StudentRecord student in students)
        {
            builder.Append(student.Id);
            builder.Append(',');
            builder.Append(Escape(student.FullName));
            builder.Append(',');
            builder.Append(Escape(student.GroupName));
            builder.Append(',');
            builder.Append(Escape(student.Faculty));
            builder.Append(',');
            builder.Append(student.EnrollmentYear);
            builder.Append(',');
            builder.Append(Escape(student.Notes));
            builder.AppendLine();
        }

        await File.WriteAllTextAsync(filePath, builder.ToString(), Encoding.UTF8, cancellationToken).ConfigureAwait(false);
    }

    private static string Escape(string value)
    {
        string prepared = value.Replace("\"", "\"\"");
        return $"\"{prepared}\"";
    }
}
