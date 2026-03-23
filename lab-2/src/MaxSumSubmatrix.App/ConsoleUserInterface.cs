using System.Globalization;

namespace MaxSumSubmatrix;

/// <summary>
/// Console user interaction layer. Computational logic is delegated to dedicated services.
/// </summary>
public sealed class ConsoleUserInterface
{
    private const string MenuKeyboardInput = "1";
    private const string MenuLoadFromFile = "2";
    private const string MenuSaveSampleInput = "3";
    private const string MenuSetPaths = "4";
    private const string MenuSaveCurrentMatrix = "5";
    private const string MenuExit = "0";

    private readonly InputOutputPaths _paths;
    private MatrixData? _currentMatrix;

    public ConsoleUserInterface(InputOutputPaths paths)
    {
        _paths = paths;
    }

    public void RunMainLoop()
    {
        bool isRunning = true;
        while (isRunning)
        {
            PrintMenu();
            string? choice = Console.ReadLine();

            try
            {
                isRunning = DispatchMenu(choice);
            }
            catch (Exception ex)
            {
                ProgramErrorReporter.ReportError($"Ошибка: {ex.Message}");
            }
        }
    }

    public static void PrintWelcomeBanner()
    {
        Console.WriteLine("=== Максимальная сумма подматрицы (лаб. 2, вариант 1) ===");
        Console.WriteLine("Автор: Шунин Михаил Дмитриевич");
        Console.WriteLine("Задача: для заданной матрицы вещественных чисел найти подматрицу,");
        Console.WriteLine("        сумма элементов которой максимальна.");
        Console.WriteLine("Результат: максимальная сумма, координаты подматрицы и её элементы.");
        Console.WriteLine();
    }

    private void PrintMenu()
    {
        Console.WriteLine();
        Console.WriteLine("Меню:");
        Console.WriteLine($" {MenuKeyboardInput} — Ввести матрицу с клавиатуры и вычислить");
        Console.WriteLine($" {MenuLoadFromFile} — Загрузить матрицу из файла и вычислить");
        Console.WriteLine($" {MenuSaveSampleInput} — Сохранить пример входного файла");
        Console.WriteLine($" {MenuSetPaths} — Задать пути к файлам ввода и результата");
        Console.WriteLine($" {MenuSaveCurrentMatrix} — Сохранить текущую матрицу в файл");
        Console.WriteLine($" {MenuExit} — Выход");
        Console.Write("Выбор: ");
    }

    private bool DispatchMenu(string? choice)
    {
        switch (choice?.Trim())
        {
            case MenuKeyboardInput:
                RunKeyboardComputation();
                return true;
            case MenuLoadFromFile:
                RunFileComputation();
                return true;
            case MenuSaveSampleInput:
                SaveSampleInputFile();
                return true;
            case MenuSetPaths:
                ConfigurePathsInteractive();
                return true;
            case MenuSaveCurrentMatrix:
                SaveCurrentMatrix();
                return true;
            case MenuExit:
                return false;
            default:
                ProgramErrorReporter.ReportError("Неизвестный пункт меню.");
                return true;
        }
    }

    private void RunKeyboardComputation()
    {
        MatrixData matrix = ReadMatrixFromKeyboard();
        _currentMatrix = matrix;
        ShowComputationResult(matrix);
    }

    private void RunFileComputation()
    {
        string path = RequireInputPath();
        MatrixData matrix = MatrixFileService.LoadMatrix(path);
        _currentMatrix = matrix;
        ShowComputationResult(matrix);
    }

    private void SaveSampleInputFile()
    {
        Console.Write("Путь для сохранения примера входного файла: ");
        string? path = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(path))
        {
            ProgramErrorReporter.ReportError("Путь к файлу не может быть пустым.");
            return;
        }

        MatrixData sample = new(new[,]
        {
            { 1.0, 2.0, -1.0, -4.0 },
            { -8.0, -3.0, 4.0, 2.0 },
            { 3.0, 8.0, 10.0, 1.0 },
            { -4.0, -1.0, 1.0, 7.0 },
        });

        MatrixFileService.SaveMatrix(path.Trim(), sample);
        ProgramErrorReporter.ReportInfo($"Пример записан в {path.Trim()}");
    }

    private void ConfigurePathsInteractive()
    {
        Console.Write($"Путь к файлу ввода [{_paths.InputFilePath ?? ""}]: ");
        string? input = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(input))
        {
            _paths.InputFilePath = input.Trim();
        }

        Console.Write($"Путь к файлу результата [{_paths.ResultFilePath ?? ""}]: ");
        string? output = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(output))
        {
            _paths.ResultFilePath = output.Trim();
        }
    }

    private void SaveCurrentMatrix()
    {
        if (_currentMatrix is null)
        {
            ProgramErrorReporter.ReportError("Сначала необходимо ввести или загрузить матрицу.");
            return;
        }

        string targetPath = RequirePathForSavingMatrix();
        MatrixFileService.SaveMatrix(targetPath, _currentMatrix);
        ProgramErrorReporter.ReportInfo($"Матрица сохранена в {targetPath}");
    }

    private void ShowComputationResult(MatrixData matrix)
    {
        SubmatrixResult result = MaximumSumSubmatrixFinder.Find(matrix);
        string report = SubmatrixReportFormatter.Format(matrix, result);
        Console.WriteLine();
        Console.WriteLine(report);
        OfferSaveResult(report);
    }

    private string RequireInputPath()
    {
        if (!string.IsNullOrWhiteSpace(_paths.InputFilePath))
        {
            Console.Write($"Использовать заданный путь ввода [{_paths.InputFilePath}]? (д/н): ");
            string? answer = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(answer) && IsAffirmative(answer))
            {
                return _paths.InputFilePath!;
            }
        }

        Console.Write("Путь к файлу ввода: ");
        string? path = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new InvalidOperationException("Путь к файлу ввода не может быть пустым.");
        }

        return path.Trim();
    }

    private string RequirePathForSavingMatrix()
    {
        if (!string.IsNullOrWhiteSpace(_paths.InputFilePath))
        {
            Console.Write($"Использовать заданный путь ввода [{_paths.InputFilePath}]? (д/н): ");
            string? answer = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(answer) && IsAffirmative(answer))
            {
                return _paths.InputFilePath!;
            }
        }

        Console.Write("Путь к файлу для сохранения матрицы: ");
        string? path = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new InvalidOperationException("Путь к файлу матрицы не может быть пустым.");
        }

        return path.Trim();
    }

    private void OfferSaveResult(string report)
    {
        Console.Write("Сохранить результат в файл? (д/н): ");
        string? answer = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(answer) || !IsAffirmative(answer))
        {
            return;
        }

        string targetPath;
        if (!string.IsNullOrWhiteSpace(_paths.ResultFilePath))
        {
            Console.Write($"Использовать заданный путь результата [{_paths.ResultFilePath}]? (д/н): ");
            string? useConfiguredPath = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(useConfiguredPath) && IsAffirmative(useConfiguredPath))
            {
                targetPath = _paths.ResultFilePath!;
            }
            else
            {
                targetPath = ReadResultPath();
            }
        }
        else
        {
            targetPath = ReadResultPath();
        }

        MatrixFileService.SaveText(targetPath, report);
        ProgramErrorReporter.ReportInfo($"Результат сохранён в {targetPath}");
    }

    private static string ReadResultPath()
    {
        Console.Write("Путь к файлу результата: ");
        string? path = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new InvalidOperationException("Путь к файлу результата не может быть пустым.");
        }

        return path.Trim();
    }

    private static MatrixData ReadMatrixFromKeyboard()
    {
        int rowCount = ReadPositiveInt("Введите число строк матрицы: ");
        int columnCount = ReadPositiveInt("Введите число столбцов матрицы: ");

        double[,] values = new double[rowCount, columnCount];
        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            double[] rowValues = ReadMatrixRow(rowIndex, columnCount);
            for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {
                values[rowIndex, columnIndex] = rowValues[columnIndex];
            }
        }

        return new MatrixData(values);
    }

    private static double[] ReadMatrixRow(int rowIndex, int expectedColumnCount)
    {
        while (true)
        {
            Console.Write($"Введите строку {rowIndex + 1} ({expectedColumnCount} вещественных чисел через пробел): ");
            string? line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                ProgramErrorReporter.ReportError("Строка матрицы не может быть пустой.");
                continue;
            }

            string[] parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != expectedColumnCount)
            {
                ProgramErrorReporter.ReportError($"Необходимо ввести ровно {expectedColumnCount} элементов.");
                continue;
            }

            double[] values = new double[expectedColumnCount];
            bool allParsed = true;
            for (int columnIndex = 0; columnIndex < expectedColumnCount; columnIndex++)
            {
                if (!double.TryParse(parts[columnIndex], NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                {
                    ProgramErrorReporter.ReportError(
                        $"Элемент {columnIndex + 1} в строке {rowIndex + 1} не является корректным вещественным числом.");
                    allParsed = false;
                    break;
                }

                values[columnIndex] = value;
            }

            if (allParsed)
            {
                return values;
            }
        }
    }

    private static int ReadPositiveInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? line = Console.ReadLine();
            if (!int.TryParse(line, out int value))
            {
                ProgramErrorReporter.ReportError("Введите корректное целое число.");
                continue;
            }

            if (value <= 0)
            {
                ProgramErrorReporter.ReportError("Число должно быть положительным.");
                continue;
            }

            return value;
        }
    }

    private static bool IsAffirmative(string answer)
    {
        string normalized = answer.Trim().ToLowerInvariant();
        return normalized is "д" or "да" or "y" or "yes";
    }
}
