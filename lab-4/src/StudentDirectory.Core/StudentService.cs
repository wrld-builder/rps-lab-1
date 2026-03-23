namespace StudentDirectory;

/// <summary>
/// Application service for student operations.
/// </summary>
public sealed class StudentService : IStudentService
{
    private const int MinimumEnrollmentYear = 1900;
    private readonly IStudentRepository _repository;

    public StudentService(IStudentRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<StudentRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _repository.GetAllAsync(cancellationToken);
    }

    public async Task<StudentRecord> SaveAsync(StudentRecordInput input, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);

        string fullName = RequireNonEmpty(input.FullName, "ФИО");
        string groupName = RequireNonEmpty(input.GroupName, "Группа");
        string faculty = RequireNonEmpty(input.Faculty, "Факультет");

        int currentYear = DateTime.Now.Year + 1;
        if (input.EnrollmentYear < MinimumEnrollmentYear || input.EnrollmentYear > currentYear)
        {
            throw new InvalidOperationException(
                $"Год поступления должен находиться в диапазоне от {MinimumEnrollmentYear} до {currentYear}.");
        }

        StudentRecord student = new()
        {
            Id = input.Id,
            FullName = fullName,
            GroupName = groupName,
            Faculty = faculty,
            EnrollmentYear = input.EnrollmentYear,
            Notes = input.Notes?.Trim() ?? string.Empty,
        };

        await _repository.SaveAsync(student, cancellationToken).ConfigureAwait(false);
        return student;
    }

    private static string RequireNonEmpty(string? value, string fieldName)
    {
        string trimmed = value?.Trim() ?? string.Empty;
        if (trimmed.Length == 0)
        {
            throw new InvalidOperationException($"Поле \"{fieldName}\" не может быть пустым.");
        }

        return trimmed;
    }
}
