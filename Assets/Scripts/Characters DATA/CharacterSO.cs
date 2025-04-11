using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Character Data", order = 1)]
public class CharacterSO : ScriptableObject
{
   public string character_name;
   [TextArea] 
   public string character_description;
   public Sprite portrait_human;
   public Sprite portrait_halfhuman;
   public Sprite portrait_monster;
   public Sprite[] other_portraits;

   public List<SkillSO> skills;

   //Стартовые характеристики
   public int strengh, agility, wisdow, constitution, armor;
   public bool isEnemy;
}
