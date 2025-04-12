using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventData", menuName = "Event Data", order = 4)]
public class EventData : ScriptableObject
{
   [Tooltip("√лобальный номер самого ивента, у всех частей он один")]
   public int eventID;

   [Tooltip("Ќомер части ивента, формат: 1.1 или 120.4, первое число - глобальный номер, второе - номер части")]
   public int eventID_Part;

   [Tooltip("¬се части ивента имеют одно название, но могут дополн€тьс€, например: Ќайденное сокровище и Ќайденное сокровище: провал")]
   [TextArea]
   public string eventName;

   [TextArea]
   public string eventText;

   [Tooltip("StartEvent - —тартовый дл€ персонажа, сюжетный?\n" +
      "EnteranceEvent - ѕо€вл€етс€ после входа в комнату, старт ивента\n" +
      "EventPart - „асть ивента, по сути после любого действи€ с ивентом мы переходим к его следующей части")]
   public EventType eventType;

   [Tooltip("Ѕлокирует ли этот ивент выход из комнаты до его окончани€")]
   public bool isLockableEvent = false;

   [Tooltip("“екст на кнопку выбора, если это часть ивента")]
   [TextArea]
   public string textChoice;

   [Tooltip("¬ыборы дл€ ивента, если пустой значит ивент закончилс€")]
   public List<EventData> choices;

   public enum EventType
   {
      EventPart, //„асть ивента, по сути после любого действи€ с ивентом мы переходим к его следующей части
      StartEvent, //—тартовый дл€ персонажа, сюжетный?
      EnteranceEvent //ѕо€вл€етс€ после входа в комнату, старт ивента
   }
}
