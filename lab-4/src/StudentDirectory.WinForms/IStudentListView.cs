using StudentDirectory;

namespace StudentDirectory.WinForms;

/// <summary>
/// Abstraction over the main WinForms student list screen.
/// </summary>
public interface IStudentListView : IWin32Window
{
    event EventHandler? LoadRequested;
    event EventHandler? AddRequested;
    event EventHandler<StudentRecord>? EditRequested;
    event EventHandler? ExportRequested;

    void ShowStudents(IReadOnlyList<StudentRecord> students);

    StudentRecord? GetSelectedStudent();

    string? ChooseExportPath();

    void ShowError(string message);

    void ShowInfo(string message);

    void SetBusy(bool isBusy);
}
