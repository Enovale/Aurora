using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Aurora.Exceptions;

namespace Aurora;

public static class Northstar
{
    public static readonly string[] DefaultNorthstarMods = new[]
    {
        "Northstar.Client",
        "Northstar.Custom",
        "Northstar.CustomServers"
    };

    public static async Task<bool> DownloadAndUpdateNorthstar(string gamePath, IProgress<float>? progress = null, CancellationToken c = default)
    {
        var latestVersion = Updater.GetLatestNorthstarVersion(c);
        var updatePath = Path.Combine(Cache.CacheDirectory, $"Northstar {latestVersion}.zip");
        if (!new DirectoryInfo(Cache.CacheDirectory)
            .EnumerateFiles()
            .Contains(new FileInfo(updatePath)))
        {
            updatePath = await Updater.DownloadNorthstar(progress, c);
        }

        try
        {
            return await TryUpdateNorthstar(updatePath, gamePath, c);
        }
        catch (Exception e) when (e is IOException or InvalidDataException)
        {
            File.Delete(updatePath);
            return await DownloadAndUpdateNorthstar(gamePath, progress, c);
        }
    }

    public static async Task<bool> TryUpdateNorthstar(string updatePath, string gamePath, CancellationToken c = default)
    {
        if (IsNorthstarInstalled(gamePath))
        {
            if (await GetInstalledNorthstarVersion(gamePath, c) != await Updater.GetLatestNorthstarVersion(c))
            {
                InstallNorthstar(updatePath, gamePath, c);
                return true;
            }
        }

        return false;
    }

    public static async void InstallNorthstar(string updatePath, string gamePath, CancellationToken c = default)
    {
        PreserveFiles(Config.CoreConfig.PreservedPaths);
        await using var file = new FileStream(updatePath, FileMode.Open, FileAccess.Read, FileShare.None);
        var zip = new ZipArchive(file, ZipArchiveMode.Read);
        if (!c.IsCancellationRequested)
        {
            zip.ExtractToDirectory(gamePath, true);
        }
        RestoreFiles(Config.CoreConfig.PreservedPaths);
    }

    public static void PreserveFiles(string[] paths)
    {
        foreach (var path in paths)
        {
            if (File.Exists(path))
            {
                var newName = path + ".preserve";
                File.Move(path, newName, true);
            }
        }
    }

    public static void RestoreFiles(string[] paths)
    {
        foreach (var path in paths)
        {
            var oldName = path + ".preserve";
            if (File.Exists(oldName))
                File.Move(oldName, path, true);
        }
    }

    public static async Task<string> GetInstalledNorthstarVersion(string gamePath, CancellationToken c = default)
    {
        if (!IsNorthstarInstalled(gamePath))
            return string.Empty;

        var versions = new string[DefaultNorthstarMods.Length];

        for (var i = 0; i < DefaultNorthstarMods.Length; i++)
        {
            var manifestPath = Path.Combine(gamePath, "R2Northstar/mods", DefaultNorthstarMods[i], "mod.json");
            if (File.Exists(manifestPath))
            {
                var json = JsonNode.Parse(await File.ReadAllTextAsync(manifestPath, c));
                versions[i] = json?["Version"]?.ToString()
                              ?? throw new JsonException($"Could not read version number from {manifestPath}");
            }

            throw new FileNotFoundException("Could not find a required Northstar mod");
        }

        var version = versions.First();
        foreach (var s in versions)
        {
            if (s != version)
                throw new VersionMismatchException(
                    "Default Northstar mods have mismatching versions. Make sure not to preserve those files."
                );
        }

        return version;
    }

    public static string GetTitanfallVersion(string gamePath)
    {
        var file = Path.Combine(gamePath, "gameversion.txt");
        if (File.Exists(file))
            return File.ReadAllText(file, Encoding.UTF8);
        throw new FileNotFoundException("No Titanfall gameversion.txt file exists.");
    }

    public static bool IsNorthstarInstalled(string gamePath)
    {
        var modsPath = Path.Combine(gamePath, "R2Northstar", "mods");
        if (File.Exists(Path.Combine(gamePath, "NorthstarLauncher.exe")) &&
            Directory.Exists(modsPath) &&
            Directory.Exists(Path.Combine(modsPath, "")))
        {
            return true;
        }

        return false;
    }
}