namespace CircleIntersection;

/// <summary>
/// Единая точка вывода диагностических сообщений (унифицированная обработка ошибок программы).
/// </summary>
public static class ProgramErrorReporter
{
    public static void ReportError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void ReportInfo(string message)
    {
        Console.WriteLine(message);
    }
}
