using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Skill Data", order = 3)]
public class SkillSO : ScriptableObject
{
   public string _name;
   public Sprite icon;
   [TextArea] public string description;
   public SkillType skill_type;
   public bool isCorpseTargetToo = false;

   public enum SkillType
   {
      Solo_Target,
      Mass_Target, //�� ���� �� ��������� ����
      All, //�� ���� ���������
      Random_Target, //�� ���� �����
      Allies_Random_Target,//�� ���� ���������
      Hard_Random_Target, //�� ����
      Passive
   };
}
