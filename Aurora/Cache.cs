using System.Text.Json;

namespace Aurora;

internal static class Cache
{
    public const string NORTHSTAR_LATEST = "ns-repo";

    private static readonly string _localPath =
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    public static readonly string CacheDirectory = Path.Combine(_localPath, Config.PathName);
    private static readonly string _cachePath = Path.Combine(CacheDirectory, "cache.json");
    private static Dictionary<string, string> _cache;

    static Cache()
    {
        _cache = new Dictionary<string, string>();
        LoadCache();
        EnsurePathValid();
    }

    public static string Get(string key) => _cache[key];

    public static bool TryGet(string key, out string value)
    {
        if (_cache.ContainsKey(key))
        {
            value = _cache[key];
            return true;
        }

        value = null!;
        return false;
    }

    public static void Set(string key, string value)
    {
        _cache[key] = value;
        SerializeCache();
    }

    private static void LoadCache()
    {
        _cache = JsonSerializer.Deserialize<Dictionary<string, string>>(
            File.ReadAllText(
                _cachePath
            )
        );
    }

    private static void SerializeCache()
    {
        File.WriteAllText(_cachePath,
            JsonSerializer.Serialize(
                _cache,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                }
            )
        );
    }

    private static void EnsurePathValid()
    {
        if (!Directory.Exists(CacheDirectory))
            Directory.CreateDirectory(CacheDirectory);

        if (!File.Exists(_cachePath))
            File.WriteAllText(_cachePath, "{}");
    }
}