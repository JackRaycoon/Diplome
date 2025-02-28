using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventData
{
    public enum EventType
   {
      StartEvent,
      EnteranceEvent
   }
   public bool isLockableEvent = false; //��������� �� ���� ����� ����� �� ������� �� ��� ���������
   public EventType eventType;
}
