using StudentDirectory;

namespace StudentDirectory.WinForms;

/// <summary>
/// Main form displaying the student directory.
/// </summary>
public sealed class StudentListForm : Form, IStudentListView
{
    private readonly DataGridView _studentsGrid;
    private readonly BindingSource _bindingSource;
    private readonly Button _addButton;
    private readonly Button _editButton;
    private readonly Button _exportButton;
    private readonly Button _refreshButton;
    private readonly Label _statusLabel;
    private StudentListPresenter? _presenter;

    public StudentListForm()
    {
        Text = "ЛР4 - Список студентов";
        StartPosition = FormStartPosition.CenterScreen;
        Width = 1100;
        Height = 700;

        MenuStrip menuStrip = new();
        ToolStripMenuItem fileMenu = new("Файл");
        ToolStripMenuItem exportMenuItem = new("Сохранить список в файл");
        exportMenuItem.Click += (_, _) => ExportRequested?.Invoke(this, EventArgs.Empty);
        ToolStripMenuItem exitMenuItem = new("Выход");
        exitMenuItem.Click += (_, _) => Close();
        fileMenu.DropDownItems.Add(exportMenuItem);
        fileMenu.DropDownItems.Add(exitMenuItem);
        menuStrip.Items.Add(fileMenu);
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
                "Автор: Шунин Михаил Дмитриевич. Данные сохраняются в SQLite, поддерживаются добавление, редактирование и экспорт списка в CSV.",
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

        _addButton = new Button { Text = "Добавить", Width = 120, Height = 30 };
        _editButton = new Button { Text = "Редактировать", Width = 120, Height = 30 };
        _exportButton = new Button { Text = "Экспорт", Width = 120, Height = 30 };
        _refreshButton = new Button { Text = "Обновить", Width = 120, Height = 30 };

        toolbar.Controls.Add(_addButton);
        toolbar.Controls.Add(_editButton);
        toolbar.Controls.Add(_exportButton);
        toolbar.Controls.Add(_refreshButton);

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
            Text = "Готово.",
        };
        rootLayout.Controls.Add(_statusLabel, 0, 2);

        _addButton.Click += (_, _) => AddRequested?.Invoke(this, EventArgs.Empty);
        _refreshButton.Click += (_, _) => LoadRequested?.Invoke(this, EventArgs.Empty);
        _exportButton.Click += (_, _) => ExportRequested?.Invoke(this, EventArgs.Empty);
        _editButton.Click += (_, _) => RaiseEditRequested();
        _studentsGrid.CellDoubleClick += (_, _) => RaiseEditRequested();
        Shown += async (_, _) =>
        {
            if (_presenter is not null)
            {
                await _presenter.InitializeAsync().ConfigureAwait(true);
            }
        };
    }

    public event EventHandler? LoadRequested;
    public event EventHandler? AddRequested;
    public event EventHandler<StudentRecord>? EditRequested;
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

    public string? ChooseExportPath()
    {
        using SaveFileDialog dialog = new()
        {
            Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*",
            FileName = "students.csv",
            Title = "Сохранение списка студентов",
        };

        return dialog.ShowDialog(this) == DialogResult.OK ? dialog.FileName : null;
    }

    public void ShowError(string message)
    {
        MessageBox.Show(this, message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public void ShowInfo(string message)
    {
        MessageBox.Show(this, message, "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public void SetBusy(bool isBusy)
    {
        UseWaitCursor = isBusy;
        _addButton.Enabled = !isBusy;
        _editButton.Enabled = !isBusy;
        _exportButton.Enabled = !isBusy;
        _refreshButton.Enabled = !isBusy;
        _studentsGrid.Enabled = !isBusy;
        _statusLabel.Text = isBusy ? "Выполняется операция..." : _statusLabel.Text;
    }

    private void RaiseEditRequested()
    {
        StudentRecord? selectedStudent = GetSelectedStudent();
        if (selectedStudent is null)
        {
            ShowError("Выберите студента для редактирования.");
            return;
        }

        EditRequested?.Invoke(this, selectedStudent);
    }
}
