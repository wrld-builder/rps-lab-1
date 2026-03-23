using StudentDirectory;

namespace StudentDirectory.WinForms;

/// <summary>
/// Абстракция над диалогом добавления и редактирования студента.
/// </summary>
public interface IStudentEditorView
{
    StudentRecordInput GetInput();

    void SetInput(StudentRecord student);

    DialogResult ShowDialog(IWin32Window owner);

    void ShowValidationError(string message);
}
