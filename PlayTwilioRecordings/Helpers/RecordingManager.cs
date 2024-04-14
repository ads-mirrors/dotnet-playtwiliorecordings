// In Helpers/RecordingManager.cs
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System;
using System.Net.Http;
using System.Threading.Tasks;

public class RecordingManager
{
    private IMemoryCache _cache;
    private HttpClient _httpClient;

    public RecordingManager(IMemoryCache cache)
    {
        _cache = cache;
        _httpClient = new HttpClient();
    }

    public async Task<byte[]> GetRecordingAsync(string uri)
    {
        return await _cache.GetOrCreateAsync(uri, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromHours(1));
            var response = await _httpClient.GetAsync(uri);
            return await response.Content.ReadAsByteArrayAsync();
        });
    }
    
    public byte[] DecryptData(byte[] encryptedData, string base64EncryptedKey, string base64IV)
    {
        var encryptedKeyBytes = Convert.FromBase64String(base64EncryptedKey);
        var ivBytes = Convert.FromBase64String(base64IV);

        // Decrypt the CEK here (details depend on encryption method used by Twilio)
        byte[] decryptedKey = DecryptKey(encryptedKeyBytes);

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = decryptedKey;
            aesAlg.IV = ivBytes;
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (MemoryStream originalMemory = new MemoryStream())
                    {
                        csDecrypt.CopyTo(originalMemory);
                        return originalMemory.ToArray();
                    }
                }
            }
        }
    }
    public byte[] DecryptKey(byte[] bytes)
    {
        throw new NotImplementedException();
    }
}