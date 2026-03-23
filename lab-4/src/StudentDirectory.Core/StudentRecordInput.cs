namespace StudentDirectory;

/// <summary>
/// Input model used for student creation and editing.
/// </summary>
public sealed class StudentRecordInput
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string GroupName { get; set; } = string.Empty;

    public string Faculty { get; set; } = string.Empty;

    public int EnrollmentYear { get; set; }

    public string Notes { get; set; } = string.Empty;
}
