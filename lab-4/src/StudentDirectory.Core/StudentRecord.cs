using SQLite;

namespace StudentDirectory;

/// <summary>
/// Представляет одного студента, хранящегося в базе данных приложения.
/// </summary>
public sealed class StudentRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(200), NotNull]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(100), NotNull]
    public string GroupName { get; set; } = string.Empty;

    [MaxLength(200), NotNull]
    public string Faculty { get; set; } = string.Empty;

    public int EnrollmentYear { get; set; }

    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
}
