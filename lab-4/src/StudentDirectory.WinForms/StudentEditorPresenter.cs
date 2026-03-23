using StudentDirectory;

namespace StudentDirectory.WinForms;

/// <summary>
/// Презентер диалога добавления и редактирования студента.
/// </summary>
public sealed class StudentEditorPresenter
{
    private readonly IStudentEditorView _view;
    private readonly IStudentService _studentService;

    public StudentEditorPresenter(IStudentEditorView view, IStudentService studentService)
    {
        _view = view;
        _studentService = studentService;
    }

    public void SetExistingStudent(StudentRecord student)
    {
        _view.SetInput(student);
    }

    public async Task<StudentRecord?> RunAsync(IWin32Window owner, CancellationToken cancellationToken = default)
    {
        while (true)
        {
            if (_view.ShowDialog(owner) != DialogResult.OK)
            {
                return null;
            }

            try
            {
                return await _studentService.SaveAsync(_view.GetInput(), cancellationToken).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                _view.ShowValidationError(ex.Message);
            }
        }
    }
}
