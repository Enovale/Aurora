using System.Text.Json;

namespace Aurora;

public static class Config
{
    public const string PathName = "aurora-ns";

    private static readonly string _appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    public static readonly string ConfigDirectory = Path.Combine(_appdataPath, PathName);
    public static readonly string ConfigPath = Path.Combine(ConfigDirectory, "core.json");

    public static CoreConfig CoreConfig { get; private set; }

    static Config()
    {
        ReloadConfig();
        Save();
    }

    private static void EnsurePathValid()
    {
        if (!Directory.Exists(ConfigDirectory))
            Directory.CreateDirectory(ConfigDirectory);
        
        if(!File.Exists(ConfigPath))
            File.WriteAllText(ConfigPath, "{}");
    }

    public static void ReloadConfig()
    {
        EnsurePathValid();
        var str = File.ReadAllText(ConfigPath);
        CoreConfig = JsonSerializer.Deserialize<CoreConfig>(str) ??
                     throw new JsonException("Failed to read core config");
    }

    public static void Reset()
    {
        CoreConfig = new();
        Save();
    }

    public static void Save()
    {
        EnsurePathValid();
        File.WriteAllText(
            ConfigPath,
            JsonSerializer.Serialize(
                CoreConfig,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                }
            )
        );
    }
}