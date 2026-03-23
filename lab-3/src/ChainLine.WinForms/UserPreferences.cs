namespace ChainLine.WinForms;

/// <summary>
/// Хранит пользовательские настройки, сохраняемые между запусками приложения.
/// </summary>
public sealed class UserPreferences
{
    public bool ShowWelcomeMessage { get; set; } = true;
}
