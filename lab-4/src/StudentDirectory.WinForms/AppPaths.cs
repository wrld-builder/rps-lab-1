namespace StudentDirectory.WinForms;

/// <summary>
/// Определяет пути файловой системы, используемые приложением.
/// </summary>
public static class AppPaths
{
    public static string GetDatabasePath()
    {
        string root = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "StudentDirectory.WinForms");
        Directory.CreateDirectory(root);
        return Path.Combine(root, "students.db3");
    }
}
