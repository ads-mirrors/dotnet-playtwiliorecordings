using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class RecordingsDecryptor
{
    public byte[] DecryptRecording(byte[] encryptedData, byte[] encryptedKey, byte[] iv, byte[] authTag)
    {
        // Decrypt the content encryption key (CEK) using RSA with OAEP padding
        byte[] decryptedKey = DecryptKey(encryptedKey);

        // Decrypt the audio data using AES GCM
        return DecryptAesGcmData(encryptedData, decryptedKey, iv, authTag);
    }

    private byte[] DecryptKey(byte[] encryptedKey)
    {
        // For the purpose of this example, the RSA private key would be stored securely
        // and loaded here to decrypt the key. Implementation will depend on how you manage your keys.
        using (RSA rsa = RSA.Create()) // Ensure you have the private key initialized correctly
        {
            rsa.ImportRSAPrivateKey(Encoding.ASCII.GetBytes("yourPrivateKeyBytes"), out _);
            return rsa.Decrypt(encryptedKey, RSAEncryptionPadding.OaepSHA256);
        }
    }

    private byte[] DecryptAesGcmData(byte[] cipherText, byte[] key, byte[] iv, byte[] authTag)
    {
        using (AesGcm aesGcm = new AesGcm(key))
        {
            byte[] decryptedData = new byte[cipherText.Length];
            aesGcm.Decrypt(iv, cipherText, authTag, decryptedData, null);
            return decryptedData;
        }
    }
}