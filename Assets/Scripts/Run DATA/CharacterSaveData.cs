using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterSaveData
{
   public string nameClass;
   public int hp;
   public int defence;
   public int strengh, agility, wisdow, constitution;

   public byte currentPhase; //1 - �������, 2 - �����������, 3 - ������

   public bool isDead;
   public bool isSpawn;

   public List<string> skillNameList = new(); //��� ������������ ��������� ��� ����������, ���� ����� �������� ��������� �������

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

      skillNameList = new();
      foreach(var skill in chara.skills)
      {
         skillNameList.Add(skill.skillData.name);
      }
   }
}
