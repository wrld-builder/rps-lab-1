// Назначение модуля: главное окно Windows Forms для лабораторной работы №4.
// Автор: Шунин Михаил Дмитриевич.
// Используемые алгоритмы: перенаправление событий представления в MVP, обработка приветствия и диалоги импорта/экспорта.
using StudentDirectory;

namespace StudentDirectory.WinForms;

/// <summary>
/// Главная форма, отображающая каталог студентов.
/// </summary>
public sealed class StudentListForm : Form, IStudentListView
{
    private const string ApplicationTitle = "ЛР4 - Список студентов";
    private const string FileMenuTitle = "Файл";
    private const string HelpMenuTitle = "Справка";
    private const string ImportMenuText = "Импорт из файла";
    private const string ExportMenuText = "Сохранить список в файл";
    private const string AboutMenuText = "О программе";
    private const string ExitMenuText = "Выход";
    private const string AddButtonText = "Добавить";
    private const string EditButtonText = "Редактировать";
    private const string ExportButtonText = "Экспорт";
    private const string ImportButtonText = "Импорт";
    private const string RefreshButtonText = "Обновить";
    private const string WelcomeCheckBoxText = "Показывать приветствие при запуске";
    private const string ImportFileFilter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
    private const string ExportFileFilter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
    private const string ImportDialogTitle = "Импорт списка студентов";
    private const string ExportDialogTitle = "Сохранение списка студентов";
    private const string DefaultExportFileName = "students.csv";
    private const string ReadyText = "Готово.";
    private const string BusyText = "Выполняется операция...";
    private const string AboutTitle = "О программе";
    private const string NoSelectionMessage = "Выберите студента для редактирования.";
    private const string ErrorTitle = "Ошибка";
    private const string InfoTitle = "Информация";
    private const int FormWidth = 1100;
    private const int FormHeight = 700;
    private const int ButtonWidth = 120;
    private const int ButtonHeight = 30;

    private readonly DataGridView _studentsGrid;
    private readonly BindingSource _bindingSource;
    private readonly Button _addButton;
    private readonly Button _editButton;
    private readonly Button _exportButton;
    private readonly Button _importButton;
    private readonly Button _refreshButton;
    private readonly CheckBox _showWelcomeCheckBox;
    private readonly Label _statusLabel;
    private readonly UserPreferences _preferences;
    private StudentListPresenter? _presenter;

    public StudentListForm()
    {
        _preferences = UserPreferencesService.Load();

        Text = ApplicationTitle;
        StartPosition = FormStartPosition.CenterScreen;
        Width = FormWidth;
        Height = FormHeight;

        MenuStrip menuStrip = new();
        ToolStripMenuItem fileMenu = new(FileMenuTitle);
        ToolStripMenuItem importMenuItem = new(ImportMenuText);
        importMenuItem.Click += (_, _) => ImportRequested?.Invoke(this, EventArgs.Empty);
        ToolStripMenuItem exportMenuItem = new(ExportMenuText);
        exportMenuItem.Click += (_, _) => ExportRequested?.Invoke(this, EventArgs.Empty);
        ToolStripMenuItem exitMenuItem = new(ExitMenuText);
        exitMenuItem.Click += (_, _) => Close();
        fileMenu.DropDownItems.Add(importMenuItem);
        fileMenu.DropDownItems.Add(exportMenuItem);
        fileMenu.DropDownItems.Add(exitMenuItem);

        ToolStripMenuItem helpMenu = new(HelpMenuTitle);
        ToolStripMenuItem aboutMenuItem = new(AboutMenuText);
        aboutMenuItem.Click += (_, _) => ShowAboutDialog();
        helpMenu.DropDownItems.Add(aboutMenuItem);

        menuStrip.Items.Add(fileMenu);
        menuStrip.Items.Add(helpMenu);
        Controls.Add(menuStrip);
        MainMenuStrip = menuStrip;

        TableLayoutPanel rootLayout = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(8, 0, 8, 8),
        };
        rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        Controls.Add(rootLayout);

        Label infoLabel = new()
        {
            AutoSize = true,
            Padding = new Padding(0, 8, 0, 8),
            Text =
                "Практическая работа №4. Хранение списка студентов.\n" +
                "Автор: Шунин Михаил Дмитриевич. Данные сохраняются в SQLite, поддерживаются добавление, редактирование, импорт и экспорт списка.",
        };
        rootLayout.Controls.Add(infoLabel, 0, 0);

        SplitContainer splitContainer = new()
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Horizontal,
            FixedPanel = FixedPanel.Panel1,
            SplitterDistance = 60,
        };
        rootLayout.Controls.Add(splitContainer, 0, 1);

        FlowLayoutPanel toolbar = new()
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(4),
        };
        splitContainer.Panel1.Controls.Add(toolbar);

        _addButton = new Button { Text = AddButtonText, Width = ButtonWidth, Height = ButtonHeight };
        _editButton = new Button { Text = EditButtonText, Width = ButtonWidth, Height = ButtonHeight };
        _importButton = new Button { Text = ImportButtonText, Width = ButtonWidth, Height = ButtonHeight };
        _exportButton = new Button { Text = ExportButtonText, Width = ButtonWidth, Height = ButtonHeight };
        _refreshButton = new Button { Text = RefreshButtonText, Width = ButtonWidth, Height = ButtonHeight };
        _showWelcomeCheckBox = new CheckBox
        {
            Text = WelcomeCheckBoxText,
            AutoSize = true,
            Checked = _preferences.ShowWelcomeMessage,
            Padding = new Padding(8, 6, 0, 0),
        };
        _showWelcomeCheckBox.CheckedChanged += (_, _) =>
        {
            _preferences.ShowWelcomeMessage = _showWelcomeCheckBox.Checked;
            UserPreferencesService.Save(_preferences);
        };

        toolbar.Controls.Add(_addButton);
        toolbar.Controls.Add(_editButton);
        toolbar.Controls.Add(_importButton);
        toolbar.Controls.Add(_exportButton);
        toolbar.Controls.Add(_refreshButton);
        toolbar.Controls.Add(_showWelcomeCheckBox);

        _bindingSource = new BindingSource();
        _studentsGrid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoGenerateColumns = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            DataSource = _bindingSource,
        };
        _studentsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID", DataPropertyName = nameof(StudentRecord.Id), Width = 60 });
        _studentsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ФИО", DataPropertyName = nameof(StudentRecord.FullName), Width = 240 });
        _studentsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Группа", DataPropertyName = nameof(StudentRecord.GroupName), Width = 140 });
        _studentsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Факультет", DataPropertyName = nameof(StudentRecord.Faculty), Width = 180 });
        _studentsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Год поступления", DataPropertyName = nameof(StudentRecord.EnrollmentYear), Width = 130 });
        _studentsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Примечание", DataPropertyName = nameof(StudentRecord.Notes), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
        splitContainer.Panel2.Controls.Add(_studentsGrid);

        _statusLabel = new Label
        {
            AutoSize = true,
            Padding = new Padding(0, 8, 0, 0),
            Text = ReadyText,
        };
        rootLayout.Controls.Add(_statusLabel, 0, 2);

        _addButton.Click += (_, _) => AddRequested?.Invoke(this, EventArgs.Empty);
        _refreshButton.Click += (_, _) => LoadRequested?.Invoke(this, EventArgs.Empty);
        _importButton.Click += (_, _) => ImportRequested?.Invoke(this, EventArgs.Empty);
        _exportButton.Click += (_, _) => ExportRequested?.Invoke(this, EventArgs.Empty);
        _editButton.Click += (_, _) => RaiseEditRequested();
        _studentsGrid.CellDoubleClick += (_, _) => RaiseEditRequested();
        Shown += async (_, _) =>
        {
            if (_presenter is not null)
            {
                await _presenter.InitializeAsync().ConfigureAwait(true);
            }

            ShowWelcomeMessageIfNeeded();
        };
    }

    public event EventHandler? LoadRequested;
    public event EventHandler? AddRequested;
    public event EventHandler<StudentRecord>? EditRequested;
    public event EventHandler? ImportRequested;
    public event EventHandler? ExportRequested;

    public void AttachPresenter(StudentListPresenter presenter)
    {
        _presenter = presenter;
    }

    public void ShowStudents(IReadOnlyList<StudentRecord> students)
    {
        _bindingSource.DataSource = students.ToList();
        _statusLabel.Text = $"Записей: {students.Count}.";
    }

    public StudentRecord? GetSelectedStudent()
    {
        return _bindingSource.Current as StudentRecord;
    }

    public string? ChooseImportPath()
    {
        using OpenFileDialog dialog = new()
        {
            Filter = ImportFileFilter,
            Title = ImportDialogTitle,
        };

        return dialog.ShowDialog(this) == DialogResult.OK ? dialog.FileName : null;
    }

    public string? ChooseExportPath()
    {
        using SaveFileDialog dialog = new()
        {
            Filter = ExportFileFilter,
            FileName = DefaultExportFileName,
            Title = ExportDialogTitle,
        };

        return dialog.ShowDialog(this) == DialogResult.OK ? dialog.FileName : null;
    }

    public void ShowError(string message)
    {
        MessageBox.Show(this, message, ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public void ShowInfo(string message)
    {
        MessageBox.Show(this, message, InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public void SetBusy(bool isBusy)
    {
        UseWaitCursor = isBusy;
        _addButton.Enabled = !isBusy;
        _editButton.Enabled = !isBusy;
        _importButton.Enabled = !isBusy;
        _exportButton.Enabled = !isBusy;
        _refreshButton.Enabled = !isBusy;
        _studentsGrid.Enabled = !isBusy;
        _statusLabel.Text = isBusy ? BusyText : _statusLabel.Text;
    }

    private void RaiseEditRequested()
    {
        StudentRecord? selectedStudent = GetSelectedStudent();
        if (selectedStudent is null)
        {
            ShowError(NoSelectionMessage);
            return;
        }

        EditRequested?.Invoke(this, selectedStudent);
    }

    private void ShowWelcomeMessageIfNeeded()
    {
        if (_preferences.ShowWelcomeMessage)
        {
            ShowAboutDialog();
        }
    }

    private void ShowAboutDialog()
    {
        MessageBox.Show(
            this,
            "Практическая работа №4\n" +
            "Автор: Шунин Михаил Дмитриевич\n" +
            "Вариант: 2 (список студентов)\n" +
            "Результат: список студентов с хранением в SQLite, добавлением, редактированием, импортом и экспортом в файл.",
            AboutTitle,
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}
