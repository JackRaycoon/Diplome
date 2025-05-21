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

      //��������� save data
      runInfo.saveTeam = new();
      foreach (PlayableCharacter chara in runInfo.PlayerTeam)
      {
         if (!chara.isSummon)
            runInfo.saveTeam.Add(new(chara));
      }
      PlayerMovement.SavePosition();

      // ������������ � MessagePack
      using (MemoryStream memoryStream = new MemoryStream())
      {
         MsgPack.Serialize(runInfo, memoryStream);

         // ���������� ������
         byte[] encryptedData = EncryptAES(memoryStream, aesKey, aesIV);
         byte[] hmac = ComputeHMAC(encryptedData, hmacKey);

         // ������ � ����
         string path = Application.persistentDataPath + $"/saveRun{slot}.corrupted";
         using (FileStream fs = File.Create(path))
         {
            fs.Write(hmac, 0, hmac.Length); // ������� ���������� HMAC
            fs.Write(encryptedData, 0, encryptedData.Length); // ����� ������������� ������
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
         byte[] hmac = fileBytes[..32];  // ������ ����� � ��� HMAC
         byte[] encryptedData = fileBytes[32..];  // ��������� ����� � ������������� ������

         // �������� HMAC
         if (!ComputeHMAC(encryptedData, hmacKey).SequenceEqual(hmac))
         {
            corruptedSlots[i - 1] = true;
            continue;
         }

         try
         {
            // ���������� ������
            byte[] decryptedData = DecryptAES(encryptedData, aesKey, aesIV);

            // �������������� ������� runInfo �� �������������� ������
            using (MemoryStream memoryStream = new MemoryStream(decryptedData))
            {
               data = MsgPack.Deserialize<RunInfo>(memoryStream);
            }

            // �������� �� ������������ ������
            if (data.slotID != i)
            {
               corruptedSlots[i - 1] = true;
               continue;
            }

            // ������������ ������� ������� �� saveTeam
            data.PlayerTeam = new();
            foreach (var chara in data.saveTeam)
            {
               PlayableCharacter fighter = new(chara);
               data.PlayerTeam.Add(fighter);
            }

            // ���������� � ����
            runInfoSlots[i - 1] = data;

            // ���������� ���������� ������
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


   // --- ���������� � HMAC ---
   private static byte[] EncryptAES(MemoryStream memoryStream, byte[] key, byte[] iv)
   {
      using Aes aes = Aes.Create();
      aes.Key = key;
      aes.IV = iv;

      using MemoryStream ms = new();
      using CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);

      // ����� ��� ������ �� memoryStream � CryptoStream
      memoryStream.Position = 0; // ���������� �������, ����� ������ � ������
      memoryStream.CopyTo(cs);

      cs.Close(); // �������� CryptoStream ��� ���������� ����������
      return ms.ToArray(); // ���������� ������������� ������
   }

   private static byte[] DecryptAES(byte[] cipherData, byte[] key, byte[] iv)
   {
      using Aes aes = Aes.Create();
      aes.Key = key;
      aes.IV = iv;

      using MemoryStream ms = new(cipherData);
      using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
      using (BinaryReader reader = new(cs)) // ���������� BinaryReader ��� ������ ������
      {
         byte[] decryptedData = new byte[cipherData.Length];
         int bytesRead = reader.Read(decryptedData, 0, decryptedData.Length);
         return decryptedData.Take(bytesRead).ToArray(); // ���������� �������������� ������
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
      //�������� �������
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
