using Mullai.Abstractions.Configuration;
using Mullai.Providers.Models;
using System.Text.Json;

namespace Mullai.CLI.Controllers;

public class ConfigController
{
    private readonly ICredentialStorage _credentialStorage;
    private readonly HttpClient _httpClient;
    private MullaiProvidersConfig? _config;

    public ConfigController(ICredentialStorage credentialStorage, HttpClient httpClient)
    {
        _credentialStorage = credentialStorage;
        _httpClient = httpClient;
    }

    public List<MullaiProviderDescriptor> LoadProviders()
    {
        if (_config == null)
        {
            _config = Mullai.Providers.MullaiChatClientFactory.LoadConfig();
        }
        return _config.Providers;
    }

    public void SaveProviders()
    {
        if (_config != null)
        {
            Mullai.Providers.MullaiChatClientFactory.SaveConfig(_config);
        }
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

    public async Task<List<MullaiModelDescriptor>> GetModelsAsync(string providerName)
    {
        var apiKey = _credentialStorage.GetApiKey(providerName);
        return await Mullai.Providers.MullaiChatClientFactory.GetModelsForProviderAsync(providerName, _httpClient, apiKey);
    }
}

