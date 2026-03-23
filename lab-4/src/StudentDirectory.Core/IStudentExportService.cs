namespace StudentDirectory;

/// <summary>
/// Exports student lists to an external text file.
/// </summary>
public interface IStudentExportService
{
    Task ExportAsync(string filePath, IReadOnlyList<StudentRecord> students, CancellationToken cancellationToken = default);
}
