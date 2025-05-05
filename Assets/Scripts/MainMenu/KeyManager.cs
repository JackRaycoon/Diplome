using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
   private static readonly string aesKeyPlayerPrefKey = "aesKey";
   private static readonly string aesIVPlayerPrefKey = "aesIV";
   private static readonly string hmacKeyPlayerPrefKey = "hmacKey";

   // Функция для генерации случайных ключей
   private static byte[] GenerateRandomKey(int length)
   {
      using (var rng = new RNGCryptoServiceProvider())
      {
         var key = new byte[length];
         rng.GetBytes(key);
         return key;
      }
   }

   // Функция для сохранения ключей в PlayerPrefs (с шифрованием)
   public static void GenerateAndSaveKeys()
   {
      // Генерация ключей
      byte[] aesKey = GenerateRandomKey(32); // 256 бит для AES
      byte[] aesIV = GenerateRandomKey(16);  // 128 бит для AES
      byte[] hmacKey = GenerateRandomKey(32); // 256 бит для HMAC

      // Преобразование в Base64 для хранения в PlayerPrefs
      string aesKeyBase64 = System.Convert.ToBase64String(aesKey);
      string aesIVBase64 = System.Convert.ToBase64String(aesIV);
      string hmacKeyBase64 = System.Convert.ToBase64String(hmacKey);

      // Сохраняем ключи в PlayerPrefs
      PlayerPrefs.SetString(aesKeyPlayerPrefKey, aesKeyBase64);
      PlayerPrefs.SetString(aesIVPlayerPrefKey, aesIVBase64);
      PlayerPrefs.SetString(hmacKeyPlayerPrefKey, hmacKeyBase64);

      // Не забываем сохранить PlayerPrefs
      PlayerPrefs.Save();
   }

   // Функция для извлечения ключей из PlayerPrefs
   public static (byte[] aesKey, byte[] aesIV, byte[] hmacKey) GetSavedKeys()
   {
      // Получаем Base64-строки из PlayerPrefs
      string aesKeyBase64 = PlayerPrefs.GetString(aesKeyPlayerPrefKey, null);
      string aesIVBase64 = PlayerPrefs.GetString(aesIVPlayerPrefKey, null);
      string hmacKeyBase64 = PlayerPrefs.GetString(hmacKeyPlayerPrefKey, null);

      if (aesKeyBase64 == null || aesIVBase64 == null || hmacKeyBase64 == null ||
         aesKeyBase64 == "" || aesIVBase64 == "" || hmacKeyBase64 == "")
      {
         // Если ключи не найдены в PlayerPrefs, генерируем новые
         GenerateAndSaveKeys();
         return GetSavedKeys();
      }

      // Преобразуем из Base64 обратно в байты
      byte[] aesKey = System.Convert.FromBase64String(aesKeyBase64);
      byte[] aesIV = System.Convert.FromBase64String(aesIVBase64);
      byte[] hmacKey = System.Convert.FromBase64String(hmacKeyBase64);

      return (aesKey, aesIV, hmacKey);
   }
}
