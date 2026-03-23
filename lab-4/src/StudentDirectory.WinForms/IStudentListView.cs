using StudentDirectory;

namespace StudentDirectory.WinForms;

/// <summary>
/// Абстракция над главным экраном WinForms со списком студентов.
/// </summary>
public interface IStudentListView : IWin32Window
{
    event EventHandler? LoadRequested;
    event EventHandler? AddRequested;
    event EventHandler<StudentRecord>? EditRequested;
    event EventHandler? ImportRequested;
    event EventHandler? ExportRequested;

    void ShowStudents(IReadOnlyList<StudentRecord> students);

    StudentRecord? GetSelectedStudent();

    string? ChooseImportPath();

    string? ChooseExportPath();

    void ShowError(string message);

    void ShowInfo(string message);

    void SetBusy(bool isBusy);
}
