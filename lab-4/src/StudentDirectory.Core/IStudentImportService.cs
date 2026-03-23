namespace StudentDirectory;

/// <summary>
/// Импортирует списки студентов из файла, указанного пользователем.
/// </summary>
public interface IStudentImportService
{
    Task<IReadOnlyList<StudentRecordInput>> ImportAsync(string filePath, CancellationToken cancellationToken = default);
}
