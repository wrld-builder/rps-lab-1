using SQLite;

namespace StudentDirectory;

/// <summary>
/// Represents one student stored in the application database.
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
