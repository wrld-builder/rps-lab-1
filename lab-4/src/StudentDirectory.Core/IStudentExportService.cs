namespace StudentDirectory;

/// <summary>
/// Экспортирует списки студентов во внешний текстовый файл.
/// </summary>
public interface IStudentExportService
{
    Task ExportAsync(string filePath, IReadOnlyList<StudentRecord> students, CancellationToken cancellationToken = default);
}
