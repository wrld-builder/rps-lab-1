namespace StudentDirectory;

/// <summary>
/// Asynchronous access to student records stored in SQLite.
/// </summary>
public interface IStudentRepository
{
    Task InitializeAsync();

    Task<IReadOnlyList<StudentRecord>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<StudentRecord?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> SaveAsync(StudentRecord student, CancellationToken cancellationToken = default);
}
