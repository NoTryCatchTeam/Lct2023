using System.Threading.Tasks;

namespace Lct2023.Services;

public interface ISecureStorageService
{
    Task<string> GetValueAsync(string key);

    Task<TValue> GetValueAsync<TValue>(string key);

    Task SetValueAsync(string key, string value);

    Task SetValueAsync<TValue>(string key, TValue value);

    bool Remove(string key);

    void RemoveAll();
}
