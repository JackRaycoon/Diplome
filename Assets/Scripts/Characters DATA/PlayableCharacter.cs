using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class PlayableCharacter : Fighter
{
   public byte currentPhase = 1; //1 - �������, 2 - �����������, 3 - ������
   public Class charClass;

   public PlayableCharacter(string name) : base("Playable/" + name)
   {
      /*foreach(SkillSO skillSO in Data.skills)
      {
         skills.Add(SkillDB.Instance.GetSkillByName(skillSO.name));
      }*/
      charClass = StringToClass(name);
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
      skills = new();
      foreach(var skillName in charSD.skillNameList)
      {
         AddSkill(skillName);
      }
      charClass = StringToClass(charSD.nameClass);
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

   private Class StringToClass(string name)
   {
      return name switch
      {
         "Warrior" => Class.Warrior,
         "Archer" => Class.Archer,
         "Priest" => Class.Priest,
         _ => Class.Enemy,
      };
   }

   public enum Class
   {
      Enemy,
      All,
      Warrior,
      Archer,
      Priest
   }
}
