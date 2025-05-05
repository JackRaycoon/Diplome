using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
   private static readonly string aesKeyPlayerPrefKey = "aesKey";
   private static readonly string aesIVPlayerPrefKey = "aesIV";
   private static readonly string hmacKeyPlayerPrefKey = "hmacKey";

   // ������� ��� ��������� ��������� ������
   private static byte[] GenerateRandomKey(int length)
   {
      using (var rng = new RNGCryptoServiceProvider())
      {
         var key = new byte[length];
         rng.GetBytes(key);
         return key;
      }
   }

   // ������� ��� ���������� ������ � PlayerPrefs (� �����������)
   public static void GenerateAndSaveKeys()
   {
      // ��������� ������
      byte[] aesKey = GenerateRandomKey(32); // 256 ��� ��� AES
      byte[] aesIV = GenerateRandomKey(16);  // 128 ��� ��� AES
      byte[] hmacKey = GenerateRandomKey(32); // 256 ��� ��� HMAC

      // �������������� � Base64 ��� �������� � PlayerPrefs
      string aesKeyBase64 = System.Convert.ToBase64String(aesKey);
      string aesIVBase64 = System.Convert.ToBase64String(aesIV);
      string hmacKeyBase64 = System.Convert.ToBase64String(hmacKey);

      // ��������� ����� � PlayerPrefs
      PlayerPrefs.SetString(aesKeyPlayerPrefKey, aesKeyBase64);
      PlayerPrefs.SetString(aesIVPlayerPrefKey, aesIVBase64);
      PlayerPrefs.SetString(hmacKeyPlayerPrefKey, hmacKeyBase64);

      // �� �������� ��������� PlayerPrefs
      PlayerPrefs.Save();
   }

   // ������� ��� ���������� ������ �� PlayerPrefs
   public static (byte[] aesKey, byte[] aesIV, byte[] hmacKey) GetSavedKeys()
   {
      // �������� Base64-������ �� PlayerPrefs
      string aesKeyBase64 = PlayerPrefs.GetString(aesKeyPlayerPrefKey, null);
      string aesIVBase64 = PlayerPrefs.GetString(aesIVPlayerPrefKey, null);
      string hmacKeyBase64 = PlayerPrefs.GetString(hmacKeyPlayerPrefKey, null);

      if (aesKeyBase64 == null || aesIVBase64 == null || hmacKeyBase64 == null ||
         aesKeyBase64 == "" || aesIVBase64 == "" || hmacKeyBase64 == "")
      {
         // ���� ����� �� ������� � PlayerPrefs, ���������� �����
         GenerateAndSaveKeys();
         return GetSavedKeys();
      }

      // ����������� �� Base64 ������� � �����
      byte[] aesKey = System.Convert.FromBase64String(aesKeyBase64);
      byte[] aesIV = System.Convert.FromBase64String(aesIVBase64);
      byte[] hmacKey = System.Convert.FromBase64String(hmacKeyBase64);

      return (aesKey, aesIV, hmacKey);
   }
}
