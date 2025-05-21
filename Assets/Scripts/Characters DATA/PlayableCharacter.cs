using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class PlayableCharacter : Fighter
{
   public byte currentPhase = 1; //1 - человек, 2 - получеловек, 3 - монстр
   public bool isDoubleTurn = false;

   public PlayableCharacter(string name) : base("Playable/" + name)
   {
      /*foreach(SkillSO skillSO in Data.skills)
      {
         skills.Add(SkillDB.Instance.GetSkillByName(skillSO.name));
      }*/
   }

   public PlayableCharacter(CharacterSaveData charSD) : base("Playable/" + charSD.nameClass)
   {
      hp = charSD.hp;
      defence = charSD.defence;
      strengh = charSD.strengh;
      agility = charSD.agility;
      wisdow = charSD.wisdow;
      constitution = charSD.constitution;
      currentPhase = charSD.currentPhase;
      isDead = charSD.isDead;
      isSpawn = charSD.isSpawn;
      effectStacks = new(charSD.effectStacks);
      skills = new();
      foreach(var skillName in charSD.skillNameList)
      {
         AddSkill(skillName);
      }
   }

   public new Sprite Portrait
   {
      get
      {
         if (isSummon && Data.portrait_summon != null) return Data.portrait_summon;
         return currentPhase switch
         {
            1 => Data.portrait_human,
            2 => Data.portrait_halfhuman,
            3 => Data.portrait_monster,
            _ => null,
         };
      }
      private set { }
   }

   public List<SkillPool> AvailableSkills
   {
      get
      {
         List<SkillPool> list = new(Data._availableSkills);
         //global buffs
         if (SaveLoadController.runInfo.globalBuffs.Contains(RunInfo.GlobalBuff.AmuletWind)
            && skills.Contains(SkillDB.Instance.GetSkillByName("Amulet of the Wind"))
            )
         {
            list.Add(SkillDB.Instance.GetPoolByName("WindSkills"));
         }
         if (SaveLoadController.runInfo.globalBuffs.Contains(RunInfo.GlobalBuff.HeartDarkness)
            && skills.Contains(SkillDB.Instance.GetSkillByName("Heart of Darkness"))
            )
         {
            list.Add(SkillDB.Instance.GetPoolByName("DarkSkills"));
         }
         return list;
      }
   }
}
