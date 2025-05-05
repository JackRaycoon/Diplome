using GameDevWare.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameDevWare.Serialization.MsgPack;

public class SaveLoadController
{
   public static RunInfo[] runInfoSlots = { new(1), new(2), new(3) };
   public static bool[] corruptedSlots = { false, false, false };
   public static short slot = 0;

   public static List<Fighter> enemies = null;

   public static RunInfo runInfo {
      get
      {
         if (slot == 0) return null;
         return runInfoSlots[slot - 1];
      }
   }

   private static byte[] aesKey;
   private static byte[] aesIV;
   private static byte[] hmacKey;

   public static void Save()
   {
      (aesKey, aesIV, hmacKey) = KeyManager.GetSavedKeys();

      //Заполняем save data
      runInfo.saveTeam = new();
      foreach (PlayableCharacter chara in runInfo.PlayerTeam)
      {
         if (!chara.isSummon)
            runInfo.saveTeam.Add(new(chara));
      }
      PlayerMovement.SavePosition();

      // Сериализация с MessagePack
      using (MemoryStream memoryStream = new MemoryStream())
      {
         MsgPack.Serialize(runInfo, memoryStream);

         // Шифрование данных
         byte[] encryptedData = EncryptAES(memoryStream, aesKey, aesIV);
         byte[] hmac = ComputeHMAC(encryptedData, hmacKey);

         // Запись в файл
         string path = Application.persistentDataPath + $"/saveRun{slot}.corrupted";
         using (FileStream fs = File.Create(path))
         {
            fs.Write(hmac, 0, hmac.Length); // Сначала записываем HMAC
            fs.Write(encryptedData, 0, encryptedData.Length); // Потом зашифрованные данные
         }
      }
   }

   public static void Load()
   {
      (aesKey, aesIV, hmacKey) = KeyManager.GetSavedKeys();

      corruptedSlots[0] = false;
      corruptedSlots[1] = false;
      corruptedSlots[2] = false;

      for (int i = 1; i <= 3; i++)
      {
         RunInfo data = null;
         string path = Application.persistentDataPath + $"/saveRun{i}.corrupted";

         if (!File.Exists(path))
         {
            runInfoSlots[i - 1] = new((short)i);
            continue;
         }

         byte[] fileBytes = File.ReadAllBytes(path);
         byte[] hmac = fileBytes[..32];  // Первая часть — это HMAC
         byte[] encryptedData = fileBytes[32..];  // Остальная часть — зашифрованные данные

         // Проверка HMAC
         if (!ComputeHMAC(encryptedData, hmacKey).SequenceEqual(hmac))
         {
            corruptedSlots[i - 1] = true;
            continue;
         }

         try
         {
            // Дешифровка данных
            byte[] decryptedData = DecryptAES(encryptedData, aesKey, aesIV);

            // Десериализация объекта runInfo из расшифрованных данных
            using (MemoryStream memoryStream = new MemoryStream(decryptedData))
            {
               data = MsgPack.Deserialize<RunInfo>(memoryStream);
            }

            // Проверка на корректность данных
            if (data.slotID != i)
            {
               corruptedSlots[i - 1] = true;
               continue;
            }

            // Пересоздание команды игроков из saveTeam
            data.PlayerTeam = new();
            foreach (var chara in data.saveTeam)
            {
               PlayableCharacter fighter = new(chara);
               data.PlayerTeam.Add(fighter);
            }

            // Сохранение в слот
            runInfoSlots[i - 1] = data;

            // Обновление глобальных баффов
            GlobalBuffsUpdate(i);
         }
         catch (Exception e)
         {
            Debug.Log(e);
            corruptedSlots[i - 1] = true;
            continue;
         }
      }
   }


   // --- Шифрование и HMAC ---
   private static byte[] EncryptAES(MemoryStream memoryStream, byte[] key, byte[] iv)
   {
      using Aes aes = Aes.Create();
      aes.Key = key;
      aes.IV = iv;

      using MemoryStream ms = new();
      using CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);

      // Пишем все данные из memoryStream в CryptoStream
      memoryStream.Position = 0; // Сбрасываем позицию, чтобы начать с начала
      memoryStream.CopyTo(cs);

      cs.Close(); // Закрытие CryptoStream для завершения шифрования
      return ms.ToArray(); // Возвращаем зашифрованные данные
   }

   private static byte[] DecryptAES(byte[] cipherData, byte[] key, byte[] iv)
   {
      using Aes aes = Aes.Create();
      aes.Key = key;
      aes.IV = iv;

      using MemoryStream ms = new(cipherData);
      using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
      using (BinaryReader reader = new(cs)) // Используем BinaryReader для чтения байтов
      {
         byte[] decryptedData = new byte[cipherData.Length];
         int bytesRead = reader.Read(decryptedData, 0, decryptedData.Length);
         return decryptedData.Take(bytesRead).ToArray(); // Возвращаем расшифрованные данные
      }
   }
   private static byte[] ComputeHMAC(byte[] data, byte[] key)
   {
      using HMACSHA256 hmac = new(key);
      return hmac.ComputeHash(data);
   }
   public static bool ExistSave(short slot)
   {
      return File.Exists(Application.persistentDataPath + $"/saveRun{slot}.corrupted")
         && !corruptedSlots[slot - 1];
   }
   public static void ClearSave(short slot)
   {
      if(File.Exists(Application.persistentDataPath + $"/saveRun{slot}.corrupted"))
      {
         File.Delete(Application.persistentDataPath + $"/saveRun{slot}.corrupted");
      }
   }

   public static void StartFight(List<Fighter> enemiesForFight)
   {
      enemies = new(enemiesForFight);
      Pauser.needOpenFight = true;
      //SceneManager.LoadScene(2);
   }

   public static void EndFight()
   {
      //Проверка скиллов
      foreach(var chara in runInfo.PlayerTeam)
      {
         foreach(var buff in chara.buffs)
         {
            switch (buff)
            {
               case Fighter.Buff.QuietBlessing:
                  chara.hp = chara.max_hp;
                  break;
            }
         }
      }
      enemies = null;
   }
   public static void GlobalBuffsUpdate(int slot = -1)
   {
      var _runInfo = runInfo;
      if (slot != -1)
         _runInfo = runInfoSlots[slot - 1];
      foreach(var fighter in _runInfo.PlayerTeam)
      {
         _runInfo.globalBuffs = new();
         foreach (var skill in fighter.skills)
         {
            var skillData = skill.skillData;
            if (skillData.globalPassiveBuff != RunInfo.GlobalBuff.None)
            {
               _runInfo.globalBuffs.Add(skillData.globalPassiveBuff);
            }
         }
      }
   }
}
