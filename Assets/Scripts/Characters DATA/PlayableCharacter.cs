using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class PlayableCharacter : Fighter
{
   public byte currentPhase = 1; //1 - �������, 2 - �����������, 3 - ������
   public Class charClass;

   public PlayableCharacter(string name) : base(name)
   {
      //��������, ����� ��� �� ��������� �� ������� ���� �����, � ��� ������ 1-� ����� � + ����� �������� ���������
      foreach(SkillSO skillSO in Data.skills)
      {
         skills.Add(SkillDB.Instance.GetSkillByName(skillSO.name));
         if (skills.Count == 5) break;
      }
      charClass = StringToClass(name);
      //
   }

   public PlayableCharacter(CharacterSaveData charSD) : base(charSD.nameClass)
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
         "Playable Warrior" => Class.Warrior,
         "Playable Archer" => Class.Archer,
         "Playable Priest" => Class.Priest,
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
