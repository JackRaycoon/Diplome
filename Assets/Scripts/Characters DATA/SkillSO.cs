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
   public SkillTarget skill_target;
   public SkillType skill_type;
   public SkillElement skill_elem;
   public short cooldown = 1;
   //[Tooltip("Каким классам доступен скилл, враги по дефолту входят в доступные")]
   //public List<PlayableCharacter.Class> availableClasses;
   public bool isAllAvailable;
   public bool isCorpseTargetToo = false;
   [Tooltip("Является ли эта пассивка проклятием?")]
   public bool isCurse;

   [Tooltip("Для пассивных скиллов, какой бафф они выдают в начале игры.\n" +
      "Чисто для того, чтобы программно было проще сделать.")]
   public Fighter.Buff passiveBuff;
   public RunInfo.GlobalBuff globalPassiveBuff;

   public enum SkillTarget
   {
      Solo_Enemy, //По одному врагу
      Mass_Enemies, //По всем врагам
      Solo_Ally, //По одному союзнику
      Mass_Allies, //По всем союзникам
      All, //по всем абсолютно
      Random_Enemy, //Случайный враг
      Random_Ally,//Случайный союзник
      Random_Target, //Случайный персонаж
      Caster, //На себя 
      Passive
   };
   public enum SkillType
   {
      Attack,
      Defence,
      Buff,
      Heal,
      Summon,
      Access,
      Special,
      Map,
      Debuff
   }
   public enum SkillElement
   {
      Physical, //физический урон
      Fire,
      Wind,
      Water,
      Earth,
      Light,
      Dark
   }
}
