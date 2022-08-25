using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using Aurora.Extensions;

namespace Aurora;

public class Updater
{
    public static async Task<string> DownloadNorthstar(IProgress<float> progress, CancellationToken c = default)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Aurora");
        var uri = new Uri(await GetNorthstarReleaseUrl(c));
        client.Timeout = TimeSpan.FromMinutes(5);
        var path = Path.Combine(Cache.CacheDirectory, $"Northstar {await GetLatestNorthstarVersion(c)}.zip");
        using (var file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await client.DownloadAsync(uri, file, progress, c);
        }

        return path;
    }

    public static async Task<string> GetNorthstarReleaseUrl(CancellationToken c = default)
    {
        var str = await GetLatestNorthstarRelease(c);
        var json = JsonNode.Parse(str);
        return json?["assets"]?[0]?["browser_download_url"]?.ToString()
               ?? throw new Exception("Failed to grab latest release ZIP url");
    }

    public static async Task<string> GetLatestNorthstarVersion(CancellationToken c = default)
    {
        var str = await GetLatestNorthstarRelease(c);
        var json = JsonNode.Parse(str);
        return json?["tag_name"]?.ToString() ?? throw new Exception("Failed to read tag name from JSON");
    }

    public static async Task<string> GetLatestNorthstarRelease(CancellationToken c = default)
    {
        if (!Cache.TryGet(Cache.NORTHSTAR_LATEST, out var str))
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Aurora");
            using var response =
                await client.GetAsync(new Uri("https://api.github.com/repos/R2Northstar/Northstar/releases/latest"), c);
            using var content = response.Content;
            str = await content.ReadAsStringAsync(c);
            Cache.Set(Cache.NORTHSTAR_LATEST, str);
        }

        return str;
    }
}