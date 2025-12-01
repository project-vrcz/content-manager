using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;

namespace VRChatContentManager.Core.Settings;

// https://stackoverflow.com/a/42705862
public sealed class OptionsWriter(
    IConfigurationRoot configuration,
    string file)
{
    public async Task UpdateOptionsAsync(Action<JsonNode> callback, bool reload = true)
    {
        var jsonRaw = File.Exists(file) ? await File.ReadAllTextAsync(file) : "{}";
        var config = JsonNode.Parse(jsonRaw);

        if (config is null)
            throw new InvalidOperationException("Could not parse configuration file.");

        callback(config);
        await File.WriteAllTextAsync(file, config.ToJsonString());

        configuration.Reload();
    }
}