using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventData", menuName = "Event Data", order = 4)]
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

   [Tooltip("StartEvent - ��������� ��� ���������, ��������?\n" +
      "EnteranceEvent - ���������� ����� ����� � �������, ����� ������\n" +
      "EventPart - ����� ������, �� ���� ����� ������ �������� � ������� �� ��������� � ��� ��������� �����")]
   public EventType eventType;

   [Tooltip("��������� �� ���� ����� ����� �� ������� �� ��� ���������")]
   public bool isLockableEvent = false;

   [Tooltip("����� �� ������ ������, ���� ��� ����� ������")]
   [TextArea]
   public string textChoice;

   [Tooltip("������ ��� ������, ���� ������ ������ ����� ����������")]
   public List<EventData> choices;

   public enum EventType
   {
      EventPart, //����� ������, �� ���� ����� ������ �������� � ������� �� ��������� � ��� ��������� �����
      StartEvent, //��������� ��� ���������, ��������?
      EnteranceEvent //���������� ����� ����� � �������, ����� ������
   }

   [Header("�������")]
   public int minGold;
   public int maxGold;

   public List<SkillSO> rewardSkillList = new();
   [Tooltip("���� ��������, �� � ������� ����� ������ ��� ������ �� ������\n" +
      "����� ����� ����� ���������")]
   public bool allSkillsFromList;
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

   public int healRewardMin;
   public int healRewardMax;

   public bool isStatsPackReward;
}
