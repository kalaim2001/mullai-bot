using Mullai.Abstractions.Configuration;
using Mullai.Providers.Models;
using System.Text.Json;

namespace Mullai.CLI.Controllers;

public class ConfigController
{
    private readonly ICredentialStorage _credentialStorage;
    private readonly string _modelsJsonPath;

    public ConfigController(ICredentialStorage credentialStorage)
    {
        _credentialStorage = credentialStorage;
        _modelsJsonPath = Path.Combine(AppContext.BaseDirectory, "models.json");
    }

    public List<MullaiProviderDescriptor> LoadProviders()
    {
        if (!File.Exists(_modelsJsonPath)) return new List<MullaiProviderDescriptor>();

        try
        {
            var json = File.ReadAllText(_modelsJsonPath);
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("MullaiProviders", out var providersElement))
            {
                var config = JsonSerializer.Deserialize<MullaiProvidersConfig>(providersElement, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return config?.Providers ?? new List<MullaiProviderDescriptor>();
            }
        }
        catch
        {
            // Ignore
        }

        return new List<MullaiProviderDescriptor>();
    }

    public bool IsProviderEnabled(string providerName, bool defaultValue) => 
        _credentialStorage.IsProviderEnabled(providerName, defaultValue);

    public void SetProviderEnabled(string providerName, bool enabled) => 
        _credentialStorage.SetProviderEnabled(providerName, enabled);

    public bool IsModelEnabled(string providerName, string modelId, bool defaultValue) => 
        _credentialStorage.IsModelEnabled(providerName, modelId, defaultValue);

    public void SetModelEnabled(string providerName, string modelId, bool enabled) => 
        _credentialStorage.SetModelEnabled(providerName, modelId, enabled);

    public string? GetApiKey(string providerName) => 
        _credentialStorage.GetApiKey(providerName);

    public void SaveApiKey(string providerName, string key) => 
        _credentialStorage.SaveApiKey(providerName, key);

    public void DeleteApiKey(string providerName) => 
        _credentialStorage.DeleteApiKey(providerName);
}
