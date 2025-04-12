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

   [TextArea]
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
}
