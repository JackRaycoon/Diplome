using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class CharacterSaveData
{
   [DataMember]
   public string nameClass;
   [DataMember]
   public int hp;
   [DataMember]
   public int defence;
   [DataMember]
   public int strengh, agility, wisdow, constitution;
   [DataMember]
   public byte currentPhase; //1 - человек, 2 - получеловек, 3 - монстр
   [DataMember]
   public bool isDead;
   [DataMember]
   public bool isSpawn;
   [DataMember]
   public Dictionary<Fighter.Effect, int> effectStacks = new();
   [DataMember]
   public List<string> skillNameList = new(); //Или использовать структуру для сохранения, если будут усиления конкретно скиллов

   public CharacterSaveData() { }
   public CharacterSaveData(PlayableCharacter chara)
   {
      nameClass = chara.Data.name;
      hp = chara.hp;
      defence = chara.defence;
      strengh = chara.strengh;
      agility = chara.agility;
      wisdow = chara.wisdow;
      constitution = chara.constitution;
      currentPhase = chara.currentPhase;
      isDead = chara.isDead;
      isSpawn = chara.isSpawn;

      effectStacks = new(chara.effectStacks);
      skillNameList = new();
      foreach(var skill in chara.skills)
      {
         skillNameList.Add(skill.skillData.name);
      }
   }
}
