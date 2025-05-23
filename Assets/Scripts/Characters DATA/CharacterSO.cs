using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Character Data", order = 1)]
public class CharacterSO : ScriptableObject
{
   public string character_name;
   public Class charClass;
   [TextArea] 
   public string character_description;
   public Sprite portrait_human;
   public Sprite portrait_halfhuman;
   public Sprite portrait_monster;
   public Sprite portrait_summon;
   public Sprite[] other_portraits;

   public List<SkillSO> skills;

   public List<SkillPool> _availableSkills;

   //Стартовые характеристики
   public int strengh, agility, wisdow, constitution, defence;

   public List<Stat> priorityStats;

   public enum Stat
   {
      Strengh,
      Agility,
      Wisdow,
      Constitution,
      Defence
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
