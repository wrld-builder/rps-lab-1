namespace StudentDirectory;

/// <summary>
/// Проверяет и сохраняет записи о студентах.
/// </summary>
public interface IStudentService
{
    Task<IReadOnlyList<StudentRecord>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<StudentRecord> SaveAsync(StudentRecordInput input, CancellationToken cancellationToken = default);
}
