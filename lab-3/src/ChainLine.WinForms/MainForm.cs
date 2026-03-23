using System.Drawing;
using ChainLine;

namespace ChainLine.WinForms;

/// <summary>
/// Main application window for graph plotting and table display.
/// </summary>
public sealed class MainForm : Form
{
    private readonly TextBox _leftBoundaryTextBox;
    private readonly TextBox _rightBoundaryTextBox;
    private readonly TextBox _stepTextBox;
    private readonly TextBox _coefficientTextBox;
    private readonly CheckBox _showWelcomeCheckBox;
    private readonly Button _calculateButton;
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

        Text = "ЛР3 - Цепная линия";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1100, 700);
        Size = new Size(1260, 820);

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
                "Автор: Шунин Михаил Дмитриевич. Программа выводит таблицу значений, график функции и умеет экспортировать данные в Excel.",
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
            Text = "Построить",
            Location = new Point(920, 94),
            Size = new Size(110, 30),
        };
        _calculateButton.Click += (_, _) => CalculateAndRender();

        _exportExcelButton = new Button
        {
            Text = "Экспорт в Excel",
            Location = new Point(1040, 94),
            Size = new Size(130, 30),
            Enabled = false,
        };
        _exportExcelButton.Click += (_, _) => ExportToExcel();

        _clearButton = new Button
        {
            Text = "Очистить",
            Location = new Point(1180, 94),
            Size = new Size(80, 30),
        };
        _clearButton.Click += (_, _) => ClearResult();

        _showWelcomeCheckBox = new CheckBox
        {
            Text = "Показывать приветствие при запуске",
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
        topPanel.Controls.Add(_exportExcelButton);
        topPanel.Controls.Add(_clearButton);
        topPanel.Controls.Add(_showWelcomeCheckBox);

        SplitContainer splitContainer = new()
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical,
            SplitterDistance = 740,
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
            Text = "Готово к построению графика.",
        };
        rootLayout.Controls.Add(_statusLabel, 0, 2);

        Shown += (_, _) => ShowWelcomeMessageIfNeeded();
    }

    private MenuStrip BuildMenu()
    {
        MenuStrip menuStrip = new();

        ToolStripMenuItem fileMenu = new("Файл");
        ToolStripMenuItem exportMenuItem = new("Экспорт в Excel");
        exportMenuItem.Click += (_, _) => ExportToExcel();
        ToolStripMenuItem exitMenuItem = new("Выход");
        exitMenuItem.Click += (_, _) => Close();
        fileMenu.DropDownItems.Add(exportMenuItem);
        fileMenu.DropDownItems.Add(exitMenuItem);

        ToolStripMenuItem helpMenu = new("Справка");
        ToolStripMenuItem aboutMenuItem = new("О программе");
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
            Size = new Size(250, 23),
        };
    }

    private void ShowWelcomeMessageIfNeeded()
    {
        if (!_preferences.ShowWelcomeMessage)
        {
            return;
        }

        ShowAboutDialog();
    }

    private void ShowAboutDialog()
    {
        MessageBox.Show(
            "Практическая работа №3\n" +
            "Автор: Шунин Михаил Дмитриевич\n" +
            "Вариант: 8 (цепная линия)\n" +
            "Функция: y = a / 2 * (e^(x / a) + e^(-x / a))\n" +
            "Результат: график функции, таблица значений и экспорт исходных данных с результатами в Excel.",
            "О программе",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void CalculateAndRender()
    {
        try
        {
            FunctionParameters parameters = new(
                ParseDouble(_leftBoundaryTextBox.Text, "левая граница"),
                ParseDouble(_rightBoundaryTextBox.Text, "правая граница"),
                ParseDouble(_stepTextBox.Text, "шаг"),
                ParseDouble(_coefficientTextBox.Text, "коэффициент a"));

            _lastResult = ChainLineCalculator.Compute(parameters);
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
                    "Предупреждение",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            _lastResult = null;
            _exportExcelButton.Enabled = false;
            _valuesGrid.DataSource = null;
            _graphPanel.Invalidate();
            _statusLabel.Text = "Ошибка вычисления.";

            MessageBox.Show(
                ex.Message,
                "Ошибка",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void ExportToExcel()
    {
        if (_lastResult is null)
        {
            MessageBox.Show(
                "Сначала необходимо построить график и получить таблицу значений.",
                "Предупреждение",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        using SaveFileDialog saveFileDialog = new()
        {
            Filter = "Excel Workbook (*.xlsx)|*.xlsx",
            FileName = "chain-line-results.xlsx",
            Title = "Сохранение результатов в Excel",
        };

        if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            ExcelExportService.Export(saveFileDialog.FileName, _lastResult);
            MessageBox.Show(
                "Экспорт завершён успешно.",
                "Экспорт",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Ошибка экспорта",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void ClearResult()
    {
        _lastResult = null;
        _valuesGrid.DataSource = null;
        _exportExcelButton.Enabled = false;
        _statusLabel.Text = "Данные очищены.";
        _graphPanel.Invalidate();
    }

    private void DrawGraph(Graphics graphics)
    {
        graphics.Clear(Color.White);

        Rectangle plotArea = new(50, 20, Math.Max(100, _graphPanel.ClientSize.Width - 80), Math.Max(100, _graphPanel.ClientSize.Height - 60));
        using Pen borderPen = new(Color.LightGray);
        graphics.DrawRectangle(borderPen, plotArea);

        if (_lastResult is null || _lastResult.Points.Count == 0)
        {
            using Brush textBrush = new SolidBrush(Color.DimGray);
            graphics.DrawString("Нет данных для отображения.", Font, textBrush, 70, 40);
            return;
        }

        double minX = _lastResult.Points.Min(point => point.X);
        double maxX = _lastResult.Points.Max(point => point.X);
        double minY = _lastResult.Points.Min(point => point.Y);
        double maxY = _lastResult.Points.Max(point => point.Y);

        if (Math.Abs(maxX - minX) < 1e-12)
        {
            minX -= 1;
            maxX += 1;
        }

        if (Math.Abs(maxY - minY) < 1e-12)
        {
            minY -= 1;
            maxY += 1;
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
}
