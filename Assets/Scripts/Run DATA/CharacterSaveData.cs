using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterSaveData
{
   public string nameClass;
   public int hp;
   public int max_hp;
   public int armor;
   public int strengh, agility, wisdow, constitution;

   public byte currentPhase; //1 - человек, 2 - получеловек, 3 - монстр

   public bool isDead;
   public bool isSpawn;

   public List<string> skillNameList = new(); //Или использовать структуру для сохранения, если будут усиления конкретно скиллов

   public CharacterSaveData(PlayableCharacter chara)
   {
      nameClass = chara.Data.name;
      hp = chara.hp;
      max_hp = chara.max_hp;
      armor = chara.armor;
      strengh = chara.strengh;
      agility = chara.agility;
      wisdow = chara.wisdow;
      constitution = chara.constitution;
      currentPhase = chara.currentPhase;
      isDead = chara.isDead;
      isSpawn = chara.isSpawn;

      skillNameList = new();
      foreach(var skill in chara.skills)
      {
         skillNameList.Add(skill.skillData.name);
      }
   }
}
