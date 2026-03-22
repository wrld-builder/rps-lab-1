namespace CircleIntersection;

/// <summary>
/// Single place for diagnostic messages (unified error handling for the program).
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
