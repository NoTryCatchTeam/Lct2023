using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace Lct2023.Services.Implementation;

public class SecureStorageService : ISecureStorageService
{
    private readonly ILogger<SecureStorageService> _logger;

    public SecureStorageService(ILogger<SecureStorageService> logger)
    {
        _logger = logger;
    }

    public Task<string> GetValueAsync(string key) =>
        SecureStorage.GetAsync(key);

    public async Task<TValue> GetValueAsync<TValue>(string key)
    {
        if (await SecureStorage.GetAsync(key) is not { } value ||
            string.IsNullOrEmpty(value))
        {
            return default;
        }

        return JsonConvert.DeserializeObject<TValue>(value);
    }

    public async Task SetValueAsync(string key, string value)
    {
        _logger.LogDebug($"Inserting key={key} with value={value}");

        await SecureStorage.SetAsync(key, value);

        _logger.LogDebug($"Inserted key={key} with value={value}");
    }

    public Task SetValueAsync<TValue>(string key, TValue value) =>
        SetValueAsync(key, JsonConvert.SerializeObject(value));

    public bool Remove(string key)
    {
        _logger.LogDebug($"Removing key={key}");

        var result = SecureStorage.Remove(key);

        _logger.LogDebug($"{(result ? "Successfully" : "Unsuccessfully")} removed key={key}");

        return result;
    }

    public void RemoveAll()
    {
        _logger.LogDebug("Removing all");

        SecureStorage.RemoveAll();

        _logger.LogDebug("Removed all");
    }
}
