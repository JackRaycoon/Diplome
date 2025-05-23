using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventData", menuName = "Event Data", order = 5)]
public class EventData : ScriptableObject
{
   [Tooltip("���������� ����� ������ ������, � ���� ������ �� ����")]
   public int eventID;

   [Tooltip("����� ����� ������, ������: 1.1 ��� 120.4, ������ ����� - ���������� �����, ������ - ����� �����")]
   public int eventID_Part;

   [Tooltip("��� ����� ������ ����� ���� ��������, �� ����� �����������, ��������: ��������� ��������� � ��������� ���������: ������")]
   [TextArea]
   public string eventName;

   [TextArea(10, 20)]
   public string eventText;
   [TextArea(10, 20)]
   [Tooltip("��� ���� ����� �������� ��� ���� � ������")]
   public string eventTextWomanCharacter;

   [Tooltip("StartEvent - ��������� ��� ���������, ��������?\n" +
      "EnteranceEvent - ���������� ����� ����� � �������, ����� ������\n" +
      "Trap - ������� 2 ������ - ������ ��� ��������� � �������, ������ ��� ��������� �������\n" +
      "EventPart - ����� ������, �� ���� ����� ������ �������� � ������� �� ��������� � ��� ��������� �����")]
   public EventType eventType;

   [Tooltip("��������� �� ���� ����� ����� �� ������� �� ��� ���������")]
   public bool isLockableEvent = false;

   [Tooltip("����� �� ������ ������, ���� ��� ����� ������")]
   [TextArea]
   public string textChoice;
   [Tooltip("������� �� ��� �����?")]
   public bool isHidden;

   [Tooltip("������ ��� ������, ���� ������ ������ ����� ����������")]
   public List<EventData> choices;

   public enum EventType
   {
      EventPart, //����� ������, �� ���� ����� ������ �������� � ������� �� ��������� � ��� ��������� �����
      StartEvent, //��������� ��� ���������, ��������?
      EnteranceEvent, //���������� ����� ����� � �������, ����� ������
      FightEvent, //������ �����
      BossEvent, //���� �������
      BossWin, //����� ������ ��� ������, ����� ���������� ������� �� ��������� �������
      Trap //������� �� ���, ��������� 2 ������ ��� ��������� � ������� � � ���������
   }

   [Tooltip("���� �������, �� �� ����� ���-�� ������� � ���������? ���� None, �� �� ��������")]
   public Characteristic checkTrap;

   public enum Characteristic
   {
      None,
      Strengh,
      Agility,
      Wisdow,
      Consitution,
      Armor
   }


   [Header("���")]
   [Tooltip("���� ������ ���, �� ����� �������, �� ���� ���� ���� 1, �� ��� ������ ���������� �� ������ ������ ���" +
      "\n�� ����� ���� ���������! ������� ����� 1 ����� ��� �������� ������ (����� ������ � ���)")]
   public List<CharacterSO> enemies;
   [Tooltip("���������� ������ ����������� ����. � ����� �� ������ 6." +
   "\n�� ����� ��� randomEnemies.")]
   public List<short> enemiesCount;
   [Tooltip("��� ��������� ����� �� ����� ������ ������ �� ���.")]
   public bool isFog;
   [Tooltip("���� �������� � ���� � ���, ������ ��������� ������������ 2-� ����� �� ������ �������.")]
   public bool isNotOver;
   [Tooltip("������� �� (� %) ������ ���� � ��������� ����� ��������� � ���, ���� ������� isNotOver")]
   public int overHpProcent;
   [Tooltip("��� ��������� ������ ���� ������ ����� �� �������� ����� ��������������� �����.")]
   public bool randomEnemies;
   [Tooltip("����� ������ ��� randomEnemies.")]
   public int minEnemyCount;
   [Tooltip("����� ������ ��� randomEnemies.")]
   public int maxEnemyCount;
   [Tooltip("���� �� ����� ����������� ����� �� ������, �� 0 �� 100. ����� ������ ���� 100." +
      "\n����� ������ ��� randomEnemies.")]
   public List<float> enemiesChances;


   [Header("�������")]

   [Tooltip("������ �������� ������ ��������")]
   public bool isLost;

   public int minSoul;
   public int maxSoul;

   public List<SkillSO> rewardSkillList = new();
   [Tooltip("���� ������� ������� ����� ���� ������, ������ ������� ������ 1 ������, ������ ���� ������������")]
   public SkillPool rewardSkillPool = null;
   [Tooltip("���� ��������, �� � ������� ����� ������ ��� ������ �� ������\n" +
      "����� ����� ����� ���������")]
   public bool allSkillsFromList;
   [Tooltip("�� ��������� �������������� ������")]
   public bool ignoreSkillPools;
   [Tooltip("���� �� ��������� ������ �� ������, �� 0 �� 100.")]
   public List<float> chanceToReceiveSkill = new();
   [Tooltip("��������� �� (����������) ����� �� ������ ������� ����� � �������, ���� ��������� �� ����� �������� ���")]
   public bool randomRewardTarget;

   public int strengthRewardMin;
   public int strengthRewardMax;

   public int agilityRewardMin;
   public int agilityRewardMax;

   public int wisdowRewardMin;
   public int wisdowRewardMax;

   public int constitutionRewardMin;
   public int constitutionRewardMax;

   public int armorRewardMin;
   public int armorRewardMax;

   [Tooltip("� ���������")]
   public int healRewardMin;
   public int healRewardMax;

   public int karmaMin;
   public int karmaMax;

   //public bool isStatsPackReward;
}
