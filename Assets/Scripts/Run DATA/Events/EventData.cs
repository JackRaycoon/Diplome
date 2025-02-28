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
   public bool isLockableEvent = false; //Блокирует ли этот ивент выход из комнаты до его окончания
   public EventType eventType;
}
