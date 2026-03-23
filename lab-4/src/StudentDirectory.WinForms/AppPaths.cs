namespace StudentDirectory.WinForms;

/// <summary>
/// Resolves file system locations used by the application.
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
