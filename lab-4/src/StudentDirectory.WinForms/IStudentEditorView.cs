using StudentDirectory;

namespace StudentDirectory.WinForms;

/// <summary>
/// Abstraction over the add/edit student dialog.
/// </summary>
public interface IStudentEditorView
{
    StudentRecordInput GetInput();

    void SetInput(StudentRecord student);

    DialogResult ShowDialog(IWin32Window owner);

    void ShowValidationError(string message);
}
