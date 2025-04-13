using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadController
{
   public static RunInfo[] runInfoSlots = { new(), new(), new() };
   public static bool[] corruptedSlots = { false, false, false };
   public static short slot = 0;

   public static List<Fighter> enemies;

   public static RunInfo runInfo {
      get
      {
         if (slot == 0) return null;
         return runInfoSlots[slot - 1];
      }
   }

   public static void Save()
   {
      //Заполняем save data
      runInfo.saveTeam = new();
      foreach(PlayableCharacter chara in runInfo.PlayerTeam)
      {
         runInfo.saveTeam.Add(new(chara));
      }
      BinaryFormatter bf = new BinaryFormatter();
      FileStream file = File.Create(Application.persistentDataPath + $"/saveRun{slot}.corrupted");
      bf.Serialize(file, runInfo);
      file.Close();
   }

   public static void Load()
   {
      for (int i = 1; i <= 3; i++)
      {
         if (File.Exists(Application.persistentDataPath + $"/saveRun{i}.corrupted"))
         {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + $"/saveRun{i}.corrupted", FileMode.Open);
            RunInfo data = null;
            try
            {
               data = (RunInfo)bf.Deserialize(file);
            }
            catch
            {
               corruptedSlots[i - 1] = true;
               continue;
            }
            file.Close();
            data.PlayerTeam = new();
            foreach (var chara in data.saveTeam)
            {
               data.PlayerTeam.Add(new(chara));
            }
            runInfoSlots[i - 1] = data;
         }
      }
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
      SceneManager.LoadScene(2);
   }
}
