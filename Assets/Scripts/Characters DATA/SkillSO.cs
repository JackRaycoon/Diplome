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
   //[Tooltip("����� ������� �������� �����, ����� �� ������� ������ � ���������")]
   //public List<PlayableCharacter.Class> availableClasses;
   public bool isAllAvailable;
   public bool isCorpseTargetToo = false;
   [Tooltip("�������� �� ��� �������� ����������?")]
   public bool isCurse;

   [Tooltip("��� ��������� �������, ����� ���� ��� ������ � ������ ����.\n" +
      "����� ��� ����, ����� ���������� ���� ����� �������.")]
   public Fighter.Buff passiveBuff;
   public RunInfo.GlobalBuff globalPassiveBuff;

   public enum SkillTarget
   {
      Solo_Enemy, //�� ������ �����
      Mass_Enemies, //�� ���� ������
      Solo_Ally, //�� ������ ��������
      Mass_Allies, //�� ���� ���������
      All, //�� ���� ���������
      Random_Enemy, //��������� ����
      Random_Ally,//��������� �������
      Random_Target, //��������� ��������
      Caster, //�� ���� 
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
      Physical, //���������� ����
      Fire,
      Wind,
      Water,
      Earth,
      Light,
      Dark
   }
}
