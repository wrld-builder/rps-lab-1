// Назначение модуля: связывание главного представления списка студентов с сервисами и презентерами.
// Автор: Шунин Михаил Дмитриевич.
// Используемые алгоритмы: асинхронная координация интерфейса, маршрутизация событий MVP и сценарии импорта/экспорта.
using StudentDirectory;

namespace StudentDirectory.WinForms;

/// <summary>
/// Презентер, координирующий работу главного экрана со списком студентов.
/// </summary>
public sealed class StudentListPresenter
{
    private readonly IStudentListView _view;
    private readonly IStudentService _studentService;
    private readonly IStudentImportService _importService;
    private readonly IStudentExportService _exportService;
    private readonly Func<IStudentEditorView> _editorViewFactory;

    public StudentListPresenter(
        IStudentListView view,
        IStudentService studentService,
        IStudentImportService importService,
        IStudentExportService exportService,
        Func<IStudentEditorView> editorViewFactory)
    {
        _view = view;
        _studentService = studentService;
        _importService = importService;
        _exportService = exportService;
        _editorViewFactory = editorViewFactory;

        _view.LoadRequested += async (_, _) => await LoadStudentsAsync().ConfigureAwait(true);
        _view.AddRequested += async (_, _) => await AddStudentAsync().ConfigureAwait(true);
        _view.EditRequested += async (_, student) => await EditStudentAsync(student).ConfigureAwait(true);
        _view.ImportRequested += async (_, _) => await ImportAsync().ConfigureAwait(true);
        _view.ExportRequested += async (_, _) => await ExportAsync().ConfigureAwait(true);
    }

    public async Task InitializeAsync()
    {
        await LoadStudentsAsync().ConfigureAwait(true);
    }

    private async Task LoadStudentsAsync()
    {
        try
        {
            _view.SetBusy(true);
            IReadOnlyList<StudentRecord> students = await _studentService.GetAllAsync().ConfigureAwait(true);
            _view.ShowStudents(students);
        }
        catch (Exception ex)
        {
            _view.ShowError(ex.Message);
        }
        finally
        {
            _view.SetBusy(false);
        }
    }

    private async Task AddStudentAsync()
    {
        IStudentEditorView editorView = _editorViewFactory();
        StudentEditorPresenter presenter = new(editorView, _studentService);
        StudentRecord? student = await presenter.RunAsync((IWin32Window)_view).ConfigureAwait(true);
        if (student is not null)
        {
            _view.ShowInfo("Студент сохранён.");
            await LoadStudentsAsync().ConfigureAwait(true);
        }
    }

    private async Task EditStudentAsync(StudentRecord student)
    {
        IStudentEditorView editorView = _editorViewFactory();
        StudentEditorPresenter presenter = new(editorView, _studentService);
        presenter.SetExistingStudent(student);
        StudentRecord? savedStudent = await presenter.RunAsync((IWin32Window)_view).ConfigureAwait(true);
        if (savedStudent is not null)
        {
            _view.ShowInfo("Изменения сохранены.");
            await LoadStudentsAsync().ConfigureAwait(true);
        }
    }

    private async Task ImportAsync()
    {
        string? filePath = _view.ChooseImportPath();
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return;
        }

        try
        {
            _view.SetBusy(true);
            IReadOnlyList<StudentRecordInput> students = await _importService.ImportAsync(filePath).ConfigureAwait(true);
            foreach (StudentRecordInput student in students)
            {
                await _studentService.SaveAsync(student).ConfigureAwait(true);
            }

            _view.ShowInfo($"Импортировано записей: {students.Count}.");
            await LoadStudentsAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            _view.ShowError(ex.Message);
        }
        finally
        {
            _view.SetBusy(false);
        }
    }

    private async Task ExportAsync()
    {
        string? filePath = _view.ChooseExportPath();
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return;
        }

        try
        {
            _view.SetBusy(true);
            IReadOnlyList<StudentRecord> students = await _studentService.GetAllAsync().ConfigureAwait(true);
            await _exportService.ExportAsync(filePath, students).ConfigureAwait(true);
            _view.ShowInfo("Список студентов экспортирован.");
        }
        catch (Exception ex)
        {
            _view.ShowError(ex.Message);
        }
        finally
        {
            _view.SetBusy(false);
        }
    }
}
