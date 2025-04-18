using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventData", menuName = "Event Data", order = 5)]
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
      EnteranceEvent, //Появляется после входа в комнату, старт ивента
      BossEvent, //Босс локации
      BossWin //Ивент победы над боссом, будет появляться переход на следующую локацию
   }

   [Header("Бой")]
   [Tooltip("Если никого нет, то ивент обычный, но если есть хоть 1, то все выборы заменяются на кнопку Начать Бой" +
      "\nНе может быть финальным! Требует ровно 1 ивент для перехода дальше (после победы в бою)")]
   public List<CharacterSO> enemies;
   [Tooltip("Количество врагов конкретного типа. В сумме не больше 6." +
   "\nНе нужно для randomEnemies.")]
   public List<short> enemiesCount;
   [Tooltip("При включении игрок не будет видеть врагов до боя.")]
   public bool isFog;
   [Tooltip("При включении список выше станет пулом из которого могут сгенерироваться враги.")]
   public bool randomEnemies;
   [Tooltip("Нужно только для randomEnemies.")]
   public int minEnemyCount;
   [Tooltip("Нужно только для randomEnemies.")]
   public int maxEnemyCount;
   [Tooltip("Шанс на спавн конкретного врага из списка, от 0 до 100. Сумма должна быть 100." +
      "\nНужно только для randomEnemies.")]
   public List<float> enemiesChances;


   [Header("Награда")]
   public int minGold;
   public int maxGold;

   public List<SkillSO> rewardSkillList = new();
   [Tooltip("Пулл скиллов которые могут быть выданы, обычно выдаётся только 1 отсюда, список выше игнорируется")]
   public SkillPool rewardSkillPool = null;
   [Tooltip("Если включено, то в награду будут выданы все скиллы из списка\n" +
      "Иначе будет выдан случайный")]
   public bool allSkillsFromList;
   [Tooltip("Шанс на получение скилла из списка, от 0 до 100.")]
   public List<float> chanceToReceiveSkill = new();
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
