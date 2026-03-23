using System.Text.Json;

namespace ChainLine.WinForms;

/// <summary>
/// Считывает и сохраняет пользовательские настройки WinForms-приложения.
/// </summary>
public static class UserPreferencesService
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    public static UserPreferences Load()
    {
        string filePath = GetPreferencesFilePath();
        if (!File.Exists(filePath))
        {
            return new UserPreferences();
        }

        string content = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<UserPreferences>(content) ?? new UserPreferences();
    }

    public static void Save(UserPreferences preferences)
    {
        ArgumentNullException.ThrowIfNull(preferences);

        string filePath = GetPreferencesFilePath();
        string? directoryPath = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string content = JsonSerializer.Serialize(preferences, SerializerOptions);
        File.WriteAllText(filePath, content);
    }

    private static string GetPreferencesFilePath()
    {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(appDataPath, "ChainLine.WinForms", "preferences.json");
    }
}
