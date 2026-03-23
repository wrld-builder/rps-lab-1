// Назначение модуля: модальный диалог редактирования сущностей студентов для лабораторной работы №4.
// Автор: Шунин Михаил Дмитриевич.
// Алгоритм: сбор данных из интерфейса и передача проверенных значений обратно презентеру.
using StudentDirectory;

namespace StudentDirectory.WinForms;

/// <summary>
/// Диалоговое окно для создания и редактирования записей о студентах.
/// </summary>
public sealed class StudentEditorForm : Form, IStudentEditorView
{
    private const string DialogTitle = "Студент";
    private const int DialogWidth = 520;
    private const int DialogHeight = 360;
    private const decimal MinimumEnrollmentYear = 1900m;
    private const decimal MaximumEnrollmentYear = 3000m;
    private const int NotesHeight = 90;
    private const int ButtonWidth = 100;

    private readonly TextBox _fullNameTextBox;
    private readonly TextBox _groupTextBox;
    private readonly TextBox _facultyTextBox;
    private readonly NumericUpDown _enrollmentYearNumeric;
    private readonly TextBox _notesTextBox;
    private int _studentId;

    public StudentEditorForm()
    {
        Text = DialogTitle;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        Width = DialogWidth;
        Height = DialogHeight;

        TableLayoutPanel layout = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 6,
            Padding = new Padding(10),
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        Controls.Add(layout);

        _fullNameTextBox = AddTextBoxRow(layout, 0, "ФИО:");
        _groupTextBox = AddTextBoxRow(layout, 1, "Группа:");
        _facultyTextBox = AddTextBoxRow(layout, 2, "Факультет:");

        layout.Controls.Add(new Label { Text = "Год поступления:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 3);
        _enrollmentYearNumeric = new NumericUpDown
        {
            Minimum = MinimumEnrollmentYear,
            Maximum = MaximumEnrollmentYear,
            Value = DateTime.Now.Year,
            Width = 120,
            Anchor = AnchorStyles.Left | AnchorStyles.Right,
        };
        layout.Controls.Add(_enrollmentYearNumeric, 1, 3);

        layout.Controls.Add(new Label { Text = "Примечание:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 4);
        _notesTextBox = new TextBox
        {
            Multiline = true,
            Height = NotesHeight,
            Dock = DockStyle.Fill,
            ScrollBars = ScrollBars.Vertical,
        };
        layout.Controls.Add(_notesTextBox, 1, 4);

        FlowLayoutPanel buttonsPanel = new()
        {
            FlowDirection = FlowDirection.RightToLeft,
            Dock = DockStyle.Fill,
        };
        Button okButton = new() { Text = "Сохранить", DialogResult = DialogResult.OK, Width = ButtonWidth };
        Button cancelButton = new() { Text = "Отмена", DialogResult = DialogResult.Cancel, Width = ButtonWidth };
        buttonsPanel.Controls.Add(okButton);
        buttonsPanel.Controls.Add(cancelButton);
        layout.Controls.Add(new Label(), 0, 5);
        layout.Controls.Add(buttonsPanel, 1, 5);

        AcceptButton = okButton;
        CancelButton = cancelButton;
    }

    /// <summary>
    /// Возвращает текущие значения, введённые пользователем в диалоге.
    /// </summary>
    public StudentRecordInput GetInput()
    {
        return new StudentRecordInput
        {
            Id = _studentId,
            FullName = _fullNameTextBox.Text,
            GroupName = _groupTextBox.Text,
            Faculty = _facultyTextBox.Text,
            EnrollmentYear = (int)_enrollmentYearNumeric.Value,
            Notes = _notesTextBox.Text,
        };
    }

    /// <summary>
    /// Заполняет элементы диалога данными существующей записи о студенте.
    /// </summary>
    public void SetInput(StudentRecord student)
    {
        _studentId = student.Id;
        _fullNameTextBox.Text = student.FullName;
        _groupTextBox.Text = student.GroupName;
        _facultyTextBox.Text = student.Faculty;
        _enrollmentYearNumeric.Value = student.EnrollmentYear;
        _notesTextBox.Text = student.Notes;
    }

    public new DialogResult ShowDialog(IWin32Window owner)
    {
        return base.ShowDialog(owner);
    }

    /// <summary>
    /// Показывает унифицированное сообщение об ошибке валидации.
    /// </summary>
    public void ShowValidationError(string message)
    {
        MessageBox.Show(this, message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private static TextBox AddTextBoxRow(TableLayoutPanel layout, int rowIndex, string labelText)
    {
        layout.Controls.Add(new Label { Text = labelText, AutoSize = true, Anchor = AnchorStyles.Left }, 0, rowIndex);
        TextBox textBox = new() { Dock = DockStyle.Fill };
        layout.Controls.Add(textBox, 1, rowIndex);
        return textBox;
    }
}
