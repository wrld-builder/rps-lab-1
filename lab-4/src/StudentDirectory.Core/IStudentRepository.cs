namespace StudentDirectory;

/// <summary>
/// Обеспечивает асинхронный доступ к записям о студентах, хранящимся в SQLite.
/// </summary>
public interface IStudentRepository
{
    Task InitializeAsync();

    Task<IReadOnlyList<StudentRecord>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<StudentRecord?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> SaveAsync(StudentRecord student, CancellationToken cancellationToken = default);
}
