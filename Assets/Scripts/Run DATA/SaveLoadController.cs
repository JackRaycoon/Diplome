using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadController
{
   public static RunInfo[] runInfoSlots = { new(1), new(2), new(3) };
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
      //Debug.Log($"Before Save: HP: {runInfo.PlayerTeam[0].hp}, A: {runInfo.PlayerTeam[0].defence}");
      runInfo.saveTeam = new();
      foreach(PlayableCharacter chara in runInfo.PlayerTeam)
      {
         runInfo.saveTeam.Add(new(chara));
      }
      PlayerMovement.SavePosition();
      //Debug.Log($"In Save: HP: {runInfo.saveTeam[0].hp}, A: {runInfo.saveTeam[0].defence}");
      BinaryFormatter bf = new BinaryFormatter();
      FileStream file = File.Create(Application.persistentDataPath + $"/saveRun{slot}.corrupted");
      bf.Serialize(file, runInfo);
      file.Close();
      //Debug.Log($"After Save: HP: {runInfo.PlayerTeam[0].hp}, A: {runInfo.PlayerTeam[0].defence}");

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
               file.Close();
               continue;
            }
            file.Close();
            if (data.slotID != i)
            {
               corruptedSlots[i - 1] = true;
               continue;
            }

            data.PlayerTeam = new();
            foreach (var chara in data.saveTeam)
            {
               PlayableCharacter fighter = new(chara);
               data.PlayerTeam.Add(fighter);
            }
            runInfoSlots[i - 1] = data;

            var runInfo = data;
            GlobalBuffsUpdate(i);
         }
         else
         {
            runInfoSlots[i - 1] = new((short)i);
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
            if (skillData.skill_type == SkillSO.SkillType.Global &&
               skillData.skill_target == SkillSO.SkillTarget.Passive)
            {
               _runInfo.globalBuffs.Add(skillData.globalPassiveBuff);
            }
         }
      }
   }
}
