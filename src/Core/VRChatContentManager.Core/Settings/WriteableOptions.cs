using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using VRChatContentManager.Core.Settings.Models;

namespace VRChatContentManager.Core.Settings;

// https://stackoverflow.com/a/42705862
public sealed class WritableOptions<T>(
    string sectionName,
    OptionsWriter writer,
    IOptionsMonitor<T> options)
    : IWritableOptions<T>
    where T : class, new()
{
    public T Value => options.CurrentValue;
    
    public async Task UpdateAsync(Action<T> applyChanges)
    {
        await writer.UpdateOptionsAsync(opt =>
        {
            var jsonTypeInfo = SettingsJsonContext.Default.GetTypeInfo(typeof(T));
            if (jsonTypeInfo is null)
                throw new InvalidOperationException($"No Json type info for type {typeof(T)}");
            
            T sectionObject;
            if (opt[sectionName] is { } sectionNode)
            {
                sectionObject = sectionNode.Deserialize(jsonTypeInfo) as T ?? new T();
            }
            else
            {
                sectionObject = new T();
            }

            applyChanges(sectionObject);

            var json = JsonSerializer.Serialize(sectionObject, jsonTypeInfo);
            opt[sectionName] = JsonNode.Parse(json);
        });
    }
}