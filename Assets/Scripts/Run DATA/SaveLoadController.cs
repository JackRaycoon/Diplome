using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveLoadController
{
   public static RunInfo[] runInfoSlots = { new(1), new(2), new(3) };
   public static short slot = 0;

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
      FileStream file = File.Create(Application.persistentDataPath + $"/saveRun{runInfo.slot}.corrupted");
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
            RunInfo data = (RunInfo)bf.Deserialize(file);
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
      return File.Exists(Application.persistentDataPath + $"/saveRun{slot}.corrupted");
   }
}
