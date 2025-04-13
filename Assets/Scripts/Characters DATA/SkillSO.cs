using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Skill Data", order = 3)]
public class SkillSO : ScriptableObject
{
   public string _name;
   public Sprite icon;
   [TextArea] public string description;
   [TextArea] public string quote;
   public SkillType skill_type;
   [Tooltip("Каким классам доступен скилл, враги по дефолту входят в доступные")]
   public List<PlayableCharacter.Class> availableClasses;
   public bool isCorpseTargetToo = false;

   public enum SkillType
   {
      Solo_Enemy, //По одному врагу
      Mass_Enemies, //По всем врагам
      Solo_Ally, //По одному союзнику
      Mass_Allies, //По всем союзникам
      All, //по всем абсолютно
      Random_Enemy, //Случайный враг
      Random_Ally,//Случайный союзник
      Random_Target, //Случайный персонаж
      Passive
   };
}
