namespace StudentDirectory;

/// <summary>
/// Validates and persists student records.
/// </summary>
public interface IStudentService
{
    Task<IReadOnlyList<StudentRecord>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<StudentRecord> SaveAsync(StudentRecordInput input, CancellationToken cancellationToken = default);
}
