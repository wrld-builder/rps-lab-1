// Назначение модуля: интерфейс Windows Forms для лабораторной работы №3.
// Автор: Шунин Михаил Дмитриевич.
// Используемые алгоритмы: построение графика, импорт/экспорт файлов и сохранение настройки приветствия.
using System.Drawing;
using ChainLine;

namespace ChainLine.WinForms;

/// <summary>
/// Главное окно приложения для построения графика и отображения таблицы.
/// </summary>
public sealed class MainForm : Form
{
    private const string ApplicationTitle = "ЛР3 - Цепная линия";
    private const string AboutDialogTitle = "О программе";
    private const string FileMenuTitle = "Файл";
    private const string HelpMenuTitle = "Справка";
    private const string LoadInputMenuText = "Загрузить исходные данные";
    private const string SaveInputMenuText = "Сохранить исходные данные";
    private const string ExportExcelMenuText = "Экспорт в Excel";
    private const string ExitMenuText = "Выход";
    private const string AboutMenuText = "О программе";
    private const string BuildButtonText = "Построить";
    private const string ExportButtonText = "Экспорт в Excel";
    private const string ClearButtonText = "Очистить";
    private const string LoadButtonText = "Загрузить";
    private const string SaveButtonText = "Сохранить ввод";
    private const string WelcomeCheckBoxText = "Показывать приветствие при запуске";
    private const string ParametersFileFilter = "Text files (*.txt)|*.txt|Config files (*.cfg)|*.cfg|All files (*.*)|*.*";
    private const string ExcelFileFilter = "Excel Workbook (*.xlsx)|*.xlsx";
    private const string ParametersDefaultFileName = "chain-line-input.txt";
    private const string ExcelDefaultFileName = "chain-line-results.xlsx";
    private const string LoadParametersDialogTitle = "Загрузка исходных данных";
    private const string SaveParametersDialogTitle = "Сохранение исходных данных";
    private const string SaveExcelDialogTitle = "Сохранение результатов в Excel";
    private const string WarningDialogTitle = "Предупреждение";
    private const string ErrorDialogTitle = "Ошибка";
    private const string InfoDialogTitle = "Информация";
    private const string NoDataMessage = "Сначала необходимо построить график и получить таблицу значений.";
    private const string ExportSuccessMessage = "Экспорт завершён успешно.";
    private const string ParametersSavedMessage = "Исходные данные сохранены.";
    private const string ParametersLoadedMessage = "Исходные данные загружены из файла.";
    private const string ReadyStatusText = "Готово к построению графика.";
    private const string ComputeErrorStatusText = "Ошибка вычисления.";
    private const string ClearedStatusText = "Данные очищены.";
    private const string NoGraphDataText = "Нет данных для отображения.";
    private const int FormMinimumWidth = 1100;
    private const int FormMinimumHeight = 700;
    private const int FormWidth = 1260;
    private const int FormHeight = 820;
    private const int DefaultSplitterDistance = 740;
    private const int DefaultInputWidth = 250;
    private const int DefaultButtonHeight = 30;
    private const int BuildButtonWidth = 110;
    private const int ExportButtonWidth = 130;
    private const int AuxiliaryButtonWidth = 100;
    private const int ClearButtonWidth = 80;
    private const double ZeroRangePadding = 1d;
    private const double ZeroRangeEpsilon = 1e-12;

    private readonly TextBox _leftBoundaryTextBox;
    private readonly TextBox _rightBoundaryTextBox;
    private readonly TextBox _stepTextBox;
    private readonly TextBox _coefficientTextBox;
    private readonly CheckBox _showWelcomeCheckBox;
    private readonly Button _calculateButton;
    private readonly Button _loadInputButton;
    private readonly Button _saveInputButton;
    private readonly Button _exportExcelButton;
    private readonly Button _clearButton;
    private readonly DataGridView _valuesGrid;
    private readonly Panel _graphPanel;
    private readonly Label _statusLabel;

    private FunctionComputationResult? _lastResult;
    private UserPreferences _preferences;

    public MainForm()
    {
        _preferences = UserPreferencesService.Load();

        Text = ApplicationTitle;
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(FormMinimumWidth, FormMinimumHeight);
        Size = new Size(FormWidth, FormHeight);

        MenuStrip menuStrip = BuildMenu();
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

        Panel topPanel = new()
        {
            Dock = DockStyle.Fill,
            Height = 170,
            Padding = new Padding(0, 8, 0, 8),
        };
        rootLayout.Controls.Add(topPanel, 0, 0);

        Label descriptionLabel = new()
        {
            AutoSize = true,
            Font = new Font(Font, FontStyle.Bold),
            Text =
                "Практическая работа №3. Построение графика цепной линии y = a / 2 * (e^(x / a) + e^(-x / a)).\n" +
                "Автор: Шунин Михаил Дмитриевич. Программа выводит таблицу значений, график функции и умеет загружать, сохранять и экспортировать данные.",
            Location = new Point(8, 8),
        };
        topPanel.Controls.Add(descriptionLabel);

        Label leftBoundaryLabel = BuildInputLabel("Левая граница x:", 8, 74);
        Label rightBoundaryLabel = BuildInputLabel("Правая граница x:", 280, 74);
        Label stepLabel = BuildInputLabel("Шаг:", 552, 74);
        Label coefficientLabel = BuildInputLabel("Коэффициент a:", 734, 74);

        _leftBoundaryTextBox = BuildInputTextBox("-5", 8, 96);
        _rightBoundaryTextBox = BuildInputTextBox("5", 280, 96);
        _stepTextBox = BuildInputTextBox("0.5", 552, 96);
        _coefficientTextBox = BuildInputTextBox("2", 734, 96);

        _calculateButton = new Button
        {
            Text = BuildButtonText,
            Location = new Point(920, 94),
            Size = new Size(BuildButtonWidth, DefaultButtonHeight),
        };
        _calculateButton.Click += (_, _) => CalculateAndRender();

        _loadInputButton = new Button
        {
            Text = LoadButtonText,
            Location = new Point(920, 130),
            Size = new Size(AuxiliaryButtonWidth, DefaultButtonHeight),
        };
        _loadInputButton.Click += (_, _) => LoadInputFromFile();

        _saveInputButton = new Button
        {
            Text = SaveButtonText,
            Location = new Point(1028, 130),
            Size = new Size(AuxiliaryButtonWidth, DefaultButtonHeight),
        };
        _saveInputButton.Click += (_, _) => SaveInputToFile();

        _exportExcelButton = new Button
        {
            Text = ExportButtonText,
            Location = new Point(1040, 94),
            Size = new Size(ExportButtonWidth, DefaultButtonHeight),
            Enabled = false,
        };
        _exportExcelButton.Click += (_, _) => ExportToExcel();

        _clearButton = new Button
        {
            Text = ClearButtonText,
            Location = new Point(1180, 94),
            Size = new Size(ClearButtonWidth, DefaultButtonHeight),
        };
        _clearButton.Click += (_, _) => ClearResult();

        _showWelcomeCheckBox = new CheckBox
        {
            Text = WelcomeCheckBoxText,
            AutoSize = true,
            Checked = _preferences.ShowWelcomeMessage,
            Location = new Point(8, 132),
        };
        _showWelcomeCheckBox.CheckedChanged += (_, _) =>
        {
            _preferences.ShowWelcomeMessage = _showWelcomeCheckBox.Checked;
            UserPreferencesService.Save(_preferences);
        };

        topPanel.Controls.Add(leftBoundaryLabel);
        topPanel.Controls.Add(rightBoundaryLabel);
        topPanel.Controls.Add(stepLabel);
        topPanel.Controls.Add(coefficientLabel);
        topPanel.Controls.Add(_leftBoundaryTextBox);
        topPanel.Controls.Add(_rightBoundaryTextBox);
        topPanel.Controls.Add(_stepTextBox);
        topPanel.Controls.Add(_coefficientTextBox);
        topPanel.Controls.Add(_calculateButton);
        topPanel.Controls.Add(_loadInputButton);
        topPanel.Controls.Add(_saveInputButton);
        topPanel.Controls.Add(_exportExcelButton);
        topPanel.Controls.Add(_clearButton);
        topPanel.Controls.Add(_showWelcomeCheckBox);

        SplitContainer splitContainer = new()
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical,
            SplitterDistance = DefaultSplitterDistance,
        };
        rootLayout.Controls.Add(splitContainer, 0, 1);

        _graphPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
        };
        _graphPanel.Paint += (_, eventArgs) => DrawGraph(eventArgs.Graphics);
        splitContainer.Panel1.Controls.Add(_graphPanel);

        _valuesGrid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        };
        _valuesGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "x",
            Width = 140,
            DataPropertyName = "X",
        });
        _valuesGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "y",
            Width = 180,
            DataPropertyName = "Y",
        });
        splitContainer.Panel2.Controls.Add(_valuesGrid);

        _statusLabel = new Label
        {
            AutoSize = true,
            Padding = new Padding(0, 8, 0, 0),
            Text = ReadyStatusText,
        };
        rootLayout.Controls.Add(_statusLabel, 0, 2);

        Shown += (_, _) => ShowWelcomeMessageIfNeeded();
    }

    private MenuStrip BuildMenu()
    {
        MenuStrip menuStrip = new();

        ToolStripMenuItem fileMenu = new(FileMenuTitle);
        ToolStripMenuItem loadInputMenuItem = new(LoadInputMenuText);
        loadInputMenuItem.Click += (_, _) => LoadInputFromFile();
        ToolStripMenuItem saveInputMenuItem = new(SaveInputMenuText);
        saveInputMenuItem.Click += (_, _) => SaveInputToFile();
        ToolStripMenuItem exportMenuItem = new(ExportExcelMenuText);
        exportMenuItem.Click += (_, _) => ExportToExcel();
        ToolStripMenuItem exitMenuItem = new(ExitMenuText);
        exitMenuItem.Click += (_, _) => Close();
        fileMenu.DropDownItems.Add(loadInputMenuItem);
        fileMenu.DropDownItems.Add(saveInputMenuItem);
        fileMenu.DropDownItems.Add(exportMenuItem);
        fileMenu.DropDownItems.Add(exitMenuItem);

        ToolStripMenuItem helpMenu = new(HelpMenuTitle);
        ToolStripMenuItem aboutMenuItem = new(AboutMenuText);
        aboutMenuItem.Click += (_, _) => ShowAboutDialog();
        helpMenu.DropDownItems.Add(aboutMenuItem);

        menuStrip.Items.Add(fileMenu);
        menuStrip.Items.Add(helpMenu);
        return menuStrip;
    }

    private static Label BuildInputLabel(string text, int x, int y)
    {
        return new Label
        {
            Text = text,
            AutoSize = true,
            Location = new Point(x, y),
        };
    }

    private static TextBox BuildInputTextBox(string value, int x, int y)
    {
        return new TextBox
        {
            Text = value,
            Location = new Point(x, y),
            Size = new Size(DefaultInputWidth, 23),
        };
    }

    /// <summary>
    /// Показывает приветствие при запуске, если оно включено в настройках пользователя.
    /// </summary>
    private void ShowWelcomeMessageIfNeeded()
    {
        if (!_preferences.ShowWelcomeMessage)
        {
            return;
        }

        ShowAboutDialog();
    }

    /// <summary>
    /// Показывает сведения о программе, детали задания и выдаваемые результаты.
    /// </summary>
    private void ShowAboutDialog()
    {
        MessageBox.Show(
            "Практическая работа №3\n" +
            "Автор: Шунин Михаил Дмитриевич\n" +
            "Вариант: 8 (цепная линия)\n" +
            "Функция: y = a / 2 * (e^(x / a) + e^(-x / a))\n" +
            "Результат: график функции, таблица значений, загрузка/сохранение исходных данных и экспорт в Excel.",
            AboutDialogTitle,
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    /// <summary>
    /// Считывает параметры с формы, выполняет вычисления и обновляет элементы интерфейса.
    /// </summary>
    private void CalculateAndRender()
    {
        try
        {
            FunctionParameters parameters = BuildParametersFromInput();
            UpdateResult(ChainLineCalculator.Compute(parameters));
        }
        catch (Exception ex)
        {
            _lastResult = null;
            _exportExcelButton.Enabled = false;
            _valuesGrid.DataSource = null;
            _graphPanel.Invalidate();
            _statusLabel.Text = ComputeErrorStatusText;

            MessageBox.Show(
                ex.Message,
                ErrorDialogTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Сохраняет текущие исходные параметры в выбранный пользователем файл.
    /// </summary>
    private void SaveInputToFile()
    {
        try
        {
            FunctionParameters parameters = BuildParametersFromInput();
            using SaveFileDialog saveFileDialog = new()
            {
                Filter = ParametersFileFilter,
                FileName = ParametersDefaultFileName,
                Title = SaveParametersDialogTitle,
            };

            if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            FunctionParametersFileService.Save(saveFileDialog.FileName, parameters);
            MessageBox.Show(this, ParametersSavedMessage, InfoDialogTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, ErrorDialogTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Загружает исходные параметры из выбранного пользователем файла и обновляет форму.
    /// </summary>
    private void LoadInputFromFile()
    {
        using OpenFileDialog openFileDialog = new()
        {
            Filter = ParametersFileFilter,
            Title = LoadParametersDialogTitle,
        };

        if (openFileDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            FunctionParameters parameters = FunctionParametersFileService.Load(openFileDialog.FileName);
            ApplyParameters(parameters);
            UpdateResult(ChainLineCalculator.Compute(parameters));
            MessageBox.Show(this, ParametersLoadedMessage, InfoDialogTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, ErrorDialogTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Экспортирует текущие входные данные и вычисленные значения в книгу Excel.
    /// </summary>
    private void ExportToExcel()
    {
        if (_lastResult is null)
        {
            MessageBox.Show(
                NoDataMessage,
                WarningDialogTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        using SaveFileDialog saveFileDialog = new()
        {
            Filter = ExcelFileFilter,
            FileName = ExcelDefaultFileName,
            Title = SaveExcelDialogTitle,
        };

        if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            ExcelExportService.Export(saveFileDialog.FileName, _lastResult);
            MessageBox.Show(
                ExportSuccessMessage,
                InfoDialogTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                ErrorDialogTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Очищает отображаемые результаты, не изменяя текущие входные значения.
    /// </summary>
    private void ClearResult()
    {
        _lastResult = null;
        _valuesGrid.DataSource = null;
        _exportExcelButton.Enabled = false;
        _statusLabel.Text = ClearedStatusText;
        _graphPanel.Invalidate();
    }

    /// <summary>
    /// Отрисовывает текущее состояние графика внутри панели графика.
    /// </summary>
    private void DrawGraph(Graphics graphics)
    {
        graphics.Clear(Color.White);

        Rectangle plotArea = new(
            50,
            20,
            Math.Max(100, _graphPanel.ClientSize.Width - 80),
            Math.Max(100, _graphPanel.ClientSize.Height - 60));
        using Pen borderPen = new(Color.LightGray);
        graphics.DrawRectangle(borderPen, plotArea);

        if (_lastResult is null || _lastResult.Points.Count == 0)
        {
            using Brush textBrush = new SolidBrush(Color.DimGray);
            graphics.DrawString(NoGraphDataText, Font, textBrush, 70, 40);
            return;
        }

        double minX = _lastResult.Points.Min(point => point.X);
        double maxX = _lastResult.Points.Max(point => point.X);
        double minY = _lastResult.Points.Min(point => point.Y);
        double maxY = _lastResult.Points.Max(point => point.Y);

        if (Math.Abs(maxX - minX) < ZeroRangeEpsilon)
        {
            minX -= ZeroRangePadding;
            maxX += ZeroRangePadding;
        }

        if (Math.Abs(maxY - minY) < ZeroRangeEpsilon)
        {
            minY -= ZeroRangePadding;
            maxY += ZeroRangePadding;
        }

        DrawAxes(graphics, plotArea, minX, maxX, minY, maxY);

        PointF[] graphPoints = _lastResult.Points
            .Select(point => ToScreenPoint(point, plotArea, minX, maxX, minY, maxY))
            .ToArray();

        using Pen graphPen = new(Color.RoyalBlue, 2);
        using Brush pointBrush = new SolidBrush(Color.Crimson);

        if (graphPoints.Length == 1)
        {
            PointF point = graphPoints[0];
            graphics.FillEllipse(pointBrush, point.X - 4, point.Y - 4, 8, 8);
        }
        else
        {
            graphics.DrawLines(graphPen, graphPoints);
            foreach (PointF point in graphPoints)
            {
                graphics.FillEllipse(pointBrush, point.X - 2.5f, point.Y - 2.5f, 5, 5);
            }
        }
    }

    /// <summary>
    /// Отрисовывает оси X и Y для текущего масштаба графика.
    /// </summary>
    private void DrawAxes(Graphics graphics, Rectangle plotArea, double minX, double maxX, double minY, double maxY)
    {
        using Pen axisPen = new(Color.Gray, 1);
        using Brush textBrush = new SolidBrush(Color.Black);

        float xAxisY = (float)(plotArea.Bottom - ((0 - minY) / (maxY - minY) * plotArea.Height));
        float yAxisX = (float)(plotArea.Left + ((0 - minX) / (maxX - minX) * plotArea.Width));

        if (xAxisY >= plotArea.Top && xAxisY <= plotArea.Bottom)
        {
            graphics.DrawLine(axisPen, plotArea.Left, xAxisY, plotArea.Right, xAxisY);
        }

        if (yAxisX >= plotArea.Left && yAxisX <= plotArea.Right)
        {
            graphics.DrawLine(axisPen, yAxisX, plotArea.Top, yAxisX, plotArea.Bottom);
        }

        graphics.DrawString($"x: [{minX:G4}; {maxX:G4}]", Font, textBrush, plotArea.Left, plotArea.Bottom + 6);
        graphics.DrawString($"y: [{minY:G4}; {maxY:G4}]", Font, textBrush, plotArea.Right - 170, plotArea.Bottom + 6);
    }

    private static PointF ToScreenPoint(
        FunctionPoint point,
        Rectangle plotArea,
        double minX,
        double maxX,
        double minY,
        double maxY)
    {
        float x = (float)(plotArea.Left + (point.X - minX) / (maxX - minX) * plotArea.Width);
        float y = (float)(plotArea.Bottom - (point.Y - minY) / (maxY - minY) * plotArea.Height);
        return new PointF(x, y);
    }

    /// <summary>
    /// Разбирает вещественное число из интерфейса с использованием текущей или инвариантной культуры.
    /// </summary>
    private static double ParseDouble(string text, string parameterName)
    {
        if (double.TryParse(text, out double currentCultureValue))
        {
            return currentCultureValue;
        }

        if (double.TryParse(
            text,
            System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture,
            out double invariantValue))
        {
            return invariantValue;
        }

        throw new InvalidOperationException($"Поле \"{parameterName}\" содержит некорректное число.");
    }

    /// <summary>
    /// Формирует проверенные исходные параметры из текущих значений формы.
    /// </summary>
    private FunctionParameters BuildParametersFromInput()
    {
        return new FunctionParameters(
            ParseDouble(_leftBoundaryTextBox.Text, "левая граница"),
            ParseDouble(_rightBoundaryTextBox.Text, "правая граница"),
            ParseDouble(_stepTextBox.Text, "шаг"),
            ParseDouble(_coefficientTextBox.Text, "коэффициент a"));
    }

    /// <summary>
    /// Применяет загруженные исходные параметры к полям ввода формы.
    /// </summary>
    private void ApplyParameters(FunctionParameters parameters)
    {
        _leftBoundaryTextBox.Text = parameters.LeftBoundary.ToString("G17");
        _rightBoundaryTextBox.Text = parameters.RightBoundary.ToString("G17");
        _stepTextBox.Text = parameters.Step.ToString("G17");
        _coefficientTextBox.Text = parameters.CoefficientA.ToString("G17");
    }

    /// <summary>
    /// Обновляет таблицу, график и строку состояния после успешного вычисления.
    /// </summary>
    private void UpdateResult(FunctionComputationResult result)
    {
        _lastResult = result;
        _exportExcelButton.Enabled = true;

        _valuesGrid.DataSource = _lastResult.Points
            .Select(point => new
            {
                X = point.X.ToString("G17"),
                Y = point.Y.ToString("G17"),
            })
            .ToList();

        _graphPanel.Invalidate();
        _statusLabel.Text = $"Построено точек: {_lastResult.Points.Count}.";

        if (!string.IsNullOrWhiteSpace(_lastResult.WarningMessage))
        {
            MessageBox.Show(
                _lastResult.WarningMessage,
                WarningDialogTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }
}
