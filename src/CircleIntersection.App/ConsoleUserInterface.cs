using System.Globalization;

namespace CircleIntersection;

/// <summary>
/// Console menus and prompts; no computational logic.
/// </summary>
public sealed class ConsoleUserInterface
{
    private const string MenuCompute = "1";
    private const string MenuLoadFile = "2";
    private const string MenuSaveInputTemplate = "3";
    private const string MenuSetPaths = "4";
    private const string MenuExit = "0";

    private readonly InputOutputPaths _paths;

    public ConsoleUserInterface(InputOutputPaths paths)
    {
        _paths = paths;
    }

    public void RunMainLoop()
    {
        bool running = true;
        while (running)
        {
            PrintMenu();
            string? choice = Console.ReadLine();
            try
            {
                running = DispatchMenu(choice);
            }
            catch (Exception ex)
            {
                ProgramErrorReporter.ReportError($"Ошибка: {ex.Message}");
            }
        }
    }

    public static void PrintWelcomeBanner()
    {
        Console.WriteLine("=== Пересечение окружностей (лаб. 1, вариант 11) ===");
        Console.WriteLine("Автор: Шунин Михаил Дмитриевич");
        Console.WriteLine("Задача: для двух окружностей на плоскости определить, есть ли общая");
        Console.WriteLine("        область положительной площади, и вычислить площадь пересечения.");
        Console.WriteLine("Результат: признак наличия пересечения и площадь (в квадратных единицах).");
        Console.WriteLine();
    }

    private void PrintMenu()
    {
        Console.WriteLine();
        Console.WriteLine("Меню:");
        Console.WriteLine($" {MenuCompute} — Ввести окружности с клавиатуры и вычислить");
        Console.WriteLine($" {MenuLoadFile} — Загрузить окружности из файла и вычислить");
        Console.WriteLine($" {MenuSaveInputTemplate} — Сохранить пример входного файла");
        Console.WriteLine($" {MenuSetPaths} — Задать пути к файлам ввода и результата");
        Console.WriteLine($" {MenuExit} — Выход");
        Console.Write("Выбор: ");
    }

    private bool DispatchMenu(string? choice)
    {
        switch (choice?.Trim())
        {
            case MenuCompute:
                RunKeyboardComputation();
                return true;
            case MenuLoadFile:
                RunFileComputation();
                return true;
            case MenuSaveInputTemplate:
                SaveSampleInputFile();
                return true;
            case MenuSetPaths:
                ConfigurePathsInteractive();
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
        CircleData c1 = ReadCircle(1);
        CircleData c2 = ReadCircle(2);
        double area = CircleIntersectionCalculator.ComputeIntersectionArea(
            c1.CenterX,
            c1.CenterY,
            c1.Radius,
            c2.CenterX,
            c2.CenterY,
            c2.Radius);
        string report = CircleAnalysisFormatter.FormatAnalysis(c1, c2, area);
        Console.WriteLine();
        Console.WriteLine(report);
        OfferSaveResult(report);
    }

    private void RunFileComputation()
    {
        string path = RequireInputPath();
        (CircleData first, CircleData second) = CircleFileService.LoadTwoCircles(path);
        double area = CircleIntersectionCalculator.ComputeIntersectionArea(
            first.CenterX,
            first.CenterY,
            first.Radius,
            second.CenterX,
            second.CenterY,
            second.Radius);
        string report = CircleAnalysisFormatter.FormatAnalysis(first, second, area);
        Console.WriteLine();
        Console.WriteLine(report);
        OfferSaveResult(report);
    }

    private void SaveSampleInputFile()
    {
        Console.Write("Путь для сохранения примера (две строки: cx cy r): ");
        string? path = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(path))
        {
            ProgramErrorReporter.ReportError("Путь к файлу не может быть пустым.");
            return;
        }

        const string sample = "0 0 2\n3 0 2\n";
        CircleFileService.SaveResult(path.Trim(), sample);
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

    private string RequireInputPath()
    {
        if (!string.IsNullOrWhiteSpace(_paths.InputFilePath))
        {
            Console.Write($"Использовать заданный путь ввода [{_paths.InputFilePath}]? (д/н): ");
            string? yn = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(yn) && IsAffirmative(yn))
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

    private void OfferSaveResult(string report)
    {
        Console.Write("Сохранить результат в файл? (д/н): ");
        string? yn = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(yn) || !IsAffirmative(yn))
        {
            return;
        }

        string targetPath;
        if (!string.IsNullOrWhiteSpace(_paths.ResultFilePath))
        {
            Console.Write($"Использовать заданный путь результата [{_paths.ResultFilePath}]? (д/н): ");
            string? useCfg = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(useCfg) && IsAffirmative(useCfg))
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

        CircleFileService.SaveResult(targetPath, report);
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

    private static CircleData ReadCircle(int circleIndex)
    {
        string ordinal = circleIndex == 1 ? "первой" : "второй";
        double cx = ReadDouble($"Введите координату X центра {ordinal} окружности: ");
        double cy = ReadDouble($"Введите координату Y центра {ordinal} окружности: ");
        double r = ReadPositiveDouble($"Введите радиус {ordinal} окружности: ");
        return new CircleData(cx, cy, r);
    }

    private static double ReadDouble(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? line = Console.ReadLine();
            if (double.TryParse(line, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                return value;
            }

            ProgramErrorReporter.ReportError("Введите корректное вещественное число.");
        }
    }

    private static double ReadPositiveDouble(string prompt)
    {
        while (true)
        {
            double v = ReadDouble(prompt);
            if (v > 0)
            {
                return v;
            }

            ProgramErrorReporter.ReportError("Радиус должен быть положительным числом.");
        }
    }

    /// <summary>
    /// Принимает «да», «д», «y» (лат.) как согласие.
    /// </summary>
    private static bool IsAffirmative(string answer)
    {
        string t = answer.Trim();
        if (t.Length == 0)
        {
            return false;
        }

        char c = char.ToLowerInvariant(t[0]);
        return c == 'д' || c == 'y';
    }
}
