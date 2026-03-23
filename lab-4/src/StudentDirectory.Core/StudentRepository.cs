using SQLite;

namespace StudentDirectory;

/// <summary>
/// Асинхронный репозиторий записей о студентах на базе SQLite.
/// </summary>
public sealed class StudentRepository : IStudentRepository
{
    private readonly SQLiteAsyncConnection _connection;
    private bool _isInitialized;

    public StudentRepository(string databasePath)
    {
        if (string.IsNullOrWhiteSpace(databasePath))
        {
            throw new ArgumentException("Путь к базе данных не может быть пустым.", nameof(databasePath));
        }

        string? directoryPath = Path.GetDirectoryName(databasePath);
        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        _connection = new SQLiteAsyncConnection(databasePath);
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        await _connection.CreateTableAsync<StudentRecord>().ConfigureAwait(false);
        _isInitialized = true;
    }

    public async Task<IReadOnlyList<StudentRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await InitializeAsync().ConfigureAwait(false);
        List<StudentRecord> students = await _connection.Table<StudentRecord>()
            .OrderBy(student => student.FullName)
            .ToListAsync()
            .ConfigureAwait(false);
        return students;
    }

    public async Task<StudentRecord?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await InitializeAsync().ConfigureAwait(false);
        return await _connection.FindAsync<StudentRecord>(id).ConfigureAwait(false);
    }

    public async Task<int> SaveAsync(StudentRecord student, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(student);
        cancellationToken.ThrowIfCancellationRequested();
        await InitializeAsync().ConfigureAwait(false);

        if (student.Id == 0)
        {
            return await _connection.InsertAsync(student).ConfigureAwait(false);
        }

        return await _connection.UpdateAsync(student).ConfigureAwait(false);
    }
}
