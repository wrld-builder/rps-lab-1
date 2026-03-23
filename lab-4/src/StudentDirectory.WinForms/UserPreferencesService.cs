using System.Text.Json;

namespace StudentDirectory.WinForms;

/// <summary>
/// Назначение модуля: сохранение настроек запуска WinForms-приложения.
/// Автор: Шунин Михаил Дмитриевич.
/// Алгоритм: сериализация JSON в каталог LocalApplicationData.
/// </summary>
public static class UserPreferencesService
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };
    private const string PreferencesFileName = "preferences.json";

    /// <summary>
    /// Загружает сохранённые настройки или возвращает значения по умолчанию, если их ещё нет.
    /// </summary>
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

    /// <summary>
    /// Сохраняет настройки приложения в локальный каталог данных пользователя.
    /// </summary>
    public static void Save(UserPreferences preferences)
    {
        ArgumentNullException.ThrowIfNull(preferences);

        string filePath = GetPreferencesFilePath();
        string? directoryPath = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        File.WriteAllText(filePath, JsonSerializer.Serialize(preferences, SerializerOptions));
    }

    private static string GetPreferencesFilePath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "StudentDirectory.WinForms",
            PreferencesFileName);
    }
}
