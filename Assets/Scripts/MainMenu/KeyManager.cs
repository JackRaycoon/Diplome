using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
   private static readonly string aesKeyPlayerPrefKey = HashKeyName("cfg_1A9z" + GetDeviceSuffix());
   private static readonly string aesIVPlayerPrefKey = HashKeyName("cfg_XxB7" + GetDeviceSuffix());
   private static readonly string hmacKeyPlayerPrefKey = HashKeyName("cfg_Hk92" + GetDeviceSuffix());

   private static string GetDeviceSuffix()
   {
      return SystemInfo.deviceUniqueIdentifier.Substring(0, 4); // Для "уникальности"
   }

   private static byte[] GenerateRandomKey(int length)
   {
      using (var rng = new RNGCryptoServiceProvider())
      {
         var key = new byte[length];
         rng.GetBytes(key);
         return key;
      }
   }

   private static byte[] GetDeviceKey()
   {
      string deviceId = SystemInfo.deviceUniqueIdentifier;
      using (var sha256 = SHA256.Create())
      {
         return sha256.ComputeHash(Encoding.UTF8.GetBytes(deviceId));
      }
   }

   private static string HashKeyName(string name)
   {
      using (var sha256 = SHA256.Create())
      {
         byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(name + SystemInfo.deviceUniqueIdentifier));
         return System.Convert.ToBase64String(hash).Substring(0, 16); // Можно урезать
      }
   }

   private static string EncryptKey(byte[] key)
   {
      byte[] deviceKey = GetDeviceKey();
      using (var aes = Aes.Create())
      {
         aes.Key = deviceKey;
         aes.GenerateIV();
         aes.Mode = CipherMode.CBC;
         aes.Padding = PaddingMode.PKCS7;

         using (var encryptor = aes.CreateEncryptor())
         {
            byte[] encrypted = encryptor.TransformFinalBlock(key, 0, key.Length);
            // Сохраняем вместе IV и зашифрованные данные
            byte[] combined = new byte[aes.IV.Length + encrypted.Length];
            aes.IV.CopyTo(combined, 0);
            encrypted.CopyTo(combined, aes.IV.Length);
            return System.Convert.ToBase64String(combined);
         }
      }
   }

   private static byte[] DecryptKey(string encryptedKey)
   {
      byte[] combined = System.Convert.FromBase64String(encryptedKey);
      byte[] iv = new byte[16];
      byte[] encrypted = new byte[combined.Length - 16];
      System.Array.Copy(combined, 0, iv, 0, 16);
      System.Array.Copy(combined, 16, encrypted, 0, encrypted.Length);

      byte[] deviceKey = GetDeviceKey();

      using (var aes = Aes.Create())
      {
         aes.Key = deviceKey;
         aes.IV = iv;
         aes.Mode = CipherMode.CBC;
         aes.Padding = PaddingMode.PKCS7;

         using (var decryptor = aes.CreateDecryptor())
         {
            return decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
         }
      }
   }

   public static void GenerateAndSaveKeys()
   {
      byte[] aesKey = GenerateRandomKey(32);
      byte[] aesIV = GenerateRandomKey(16);
      byte[] hmacKey = GenerateRandomKey(32);

      PlayerPrefs.SetString(aesKeyPlayerPrefKey, EncryptKey(aesKey));
      PlayerPrefs.SetString(aesIVPlayerPrefKey, EncryptKey(aesIV));
      PlayerPrefs.SetString(hmacKeyPlayerPrefKey, EncryptKey(hmacKey));
      PlayerPrefs.Save();
   }

   public static (byte[] aesKey, byte[] aesIV, byte[] hmacKey) GetSavedKeys()
   {
      string aesKeyEncrypted = PlayerPrefs.GetString(aesKeyPlayerPrefKey, null);
      string aesIVEncrypted = PlayerPrefs.GetString(aesIVPlayerPrefKey, null);
      string hmacKeyEncrypted = PlayerPrefs.GetString(hmacKeyPlayerPrefKey, null);

      if (string.IsNullOrEmpty(aesKeyEncrypted) || string.IsNullOrEmpty(aesIVEncrypted) || string.IsNullOrEmpty(hmacKeyEncrypted))
      {
         GenerateAndSaveKeys();
         return GetSavedKeys();
      }

      byte[] aesKey = DecryptKey(aesKeyEncrypted);
      byte[] aesIV = DecryptKey(aesIVEncrypted);
      byte[] hmacKey = DecryptKey(hmacKeyEncrypted);

      return (aesKey, aesIV, hmacKey);
   }
}
