using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventData", menuName = "Event Data", order = 4)]
public class EventData : ScriptableObject
{
   [Tooltip("Глобальный номер самого ивента, у всех частей он один")]
   public int eventID;

   [Tooltip("Номер части ивента, формат: 1.1 или 120.4, первое число - глобальный номер, второе - номер части")]
   public int eventID_Part;

   [Tooltip("Все части ивента имеют одно название, но могут дополняться, например: Найденное сокровище и Найденное сокровище: провал")]
   [TextArea]
   public string eventName;

   [TextArea(10, 20)]
   public string eventText;

   [Tooltip("StartEvent - Стартовый для персонажа, сюжетный?\n" +
      "EnteranceEvent - Появляется после входа в комнату, старт ивента\n" +
      "EventPart - Часть ивента, по сути после любого действия с ивентом мы переходим к его следующей части")]
   public EventType eventType;

   [Tooltip("Блокирует ли этот ивент выход из комнаты до его окончания")]
   public bool isLockableEvent = false;

   [Tooltip("Текст на кнопку выбора, если это часть ивента")]
   [TextArea]
   public string textChoice;

   [Tooltip("Выборы для ивента, если пустой значит ивент закончился")]
   public List<EventData> choices;

   public enum EventType
   {
      EventPart, //Часть ивента, по сути после любого действия с ивентом мы переходим к его следующей части
      StartEvent, //Стартовый для персонажа, сюжетный?
      EnteranceEvent //Появляется после входа в комнату, старт ивента
   }

   [Header("Награда")]
   public int minGold;
   public int maxGold;

   public List<SkillSO> rewardSkillList = new();
   [Tooltip("Если включено, то в награду будут выданы все скиллы из списка\n" +
      "Иначе будет выдан случайный")]
   public bool allSkillsFromList;
   [Tooltip("Случайный ли (подходящий) герой из списка получит скилл в награду, если выключено то игрок выбирает сам")]
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
