using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Room3D : MonoBehaviour
{
   public Room room;

   public TextMeshProUGUI eventTitle;
   public TextMeshProUGUI eventText;
   public List<TextMeshProUGUI> eventBtnsText;
   public List<GameObject> eventBtns;
   public GameObject fightBtn;
   public GameObject trapBtn;

   private EventData data;

   public CanvasGroup eventCG;

   private RoomBehaviour roomB;

   public List<GameObject> invisibleWalls;

   private List<Fighter> enemiesForFight = new();

   bool isFilled = false;

   private void Start()
   {
      roomB = GetComponent<RoomBehaviour>();
      data = room.eventData;
   }
   private void Update()
   {
      if(room != null && data != null && !isFilled)
      {
         isFilled = true;
         FillEvent();
      }
      if(data != null 
         && data.eventType == EventData.EventType.BossWin && SaveLoadController.runInfo.currentRoom != room)
      {
         Debug.Log("���� ������ � ������, ����� ������ �� ����� ���������� �������� � ������� ���� �����.");
      }
   }

   public void FillEvent()
   {

      if ((data.eventType == EventData.EventType.EnteranceEvent ||
         data.eventType == EventData.EventType.Trap ||
         data.eventType == EventData.EventType.FightEvent ||
         data.eventType == EventData.EventType.BossEvent) &&
         SaveLoadController.runInfo.currentRoom != room)
      {
         eventCG.alpha = 0;
         eventCG.interactable = false;
         eventCG.blocksRaycasts = false;
         isFilled = false;
         return;
      }

      bool unlock = false;
      //GlobalBuffs
      foreach (var buff in SaveLoadController.runInfo.globalBuffs)
      {
         switch (buff)
         {
            case RunInfo.GlobalBuff.SilentBlood:
               if ((data.eventType == EventData.EventType.FightEvent
                  || data.eventType == EventData.EventType.BossEvent)  &&
                  data.isLockableEvent && 
                  SaveLoadController.runInfo.PlayerTeam.Count == 1)
                  unlock = true;
               break;
         }
      }

      eventCG.alpha = 1;
      eventCG.interactable = true;
      eventCG.blocksRaycasts = true;

      eventTitle.text = data.eventName;

      string text = data.eventText;
      switch (SaveLoadController.runInfo.PlayerTeam[0].Data.name)
      {
         case "Archer":
         case "Priest":
            text = data.eventTextWomanCharacter;
            break;
      }

      //�������
      if(data.choices.Count == 0 && !room.eventRewardClaim)
      {
         //��������� ���������� � �������
         room.eventRewardText = "";
         room.eventRewardText += "\n\n";
         room.eventRewardText += "��������: ";
         bool isNext = false;

         if (data.minSoul != 0)
         {
            int rewardCountSoul = Random.Range(data.minSoul, data.maxSoul + 1);
            if(rewardCountSoul != 0)
            {
               if (isNext)
               {
                  room.eventRewardText += ", ";
               }
               room.eventRewardText += $"{rewardCountSoul} {SoulWord(rewardCountSoul)}";
               isNext = true;

               SaveLoadController.runInfo.souls += rewardCountSoul;
            }
         }
         if (data.strengthRewardMax != 0)
         {
            int rewardCountStrength = Random.Range(data.strengthRewardMin, data.strengthRewardMax + 1);
            if(rewardCountStrength != 0)
            {
               if (isNext)
               {
                  room.eventRewardText += ", ";
               }
               room.eventRewardText += $"{rewardCountStrength} ����";
               isNext = true;

               SaveLoadController.runInfo.PlayerTeam[0].strengh += rewardCountStrength;
            }
         }
         if (data.agilityRewardMax != 0)
         {
            int rewardCountAgility = Random.Range(data.agilityRewardMin, data.agilityRewardMax + 1);
            if(rewardCountAgility != 0)
            {
               if (isNext)
               {
                  room.eventRewardText += ", ";
               }
               room.eventRewardText += $"{rewardCountAgility} ��������";
               isNext = true;

               SaveLoadController.runInfo.PlayerTeam[0].agility += rewardCountAgility;
            }
         }
         if (data.wisdowRewardMax != 0)
         {
            int rewardCountWisdow = Random.Range(data.wisdowRewardMin, data.wisdowRewardMax + 1);
            if(rewardCountWisdow != 0)
            {
               if (isNext)
               {
                  room.eventRewardText += ", ";
               }
               room.eventRewardText += $"{rewardCountWisdow} ��������";
               isNext = true;

               SaveLoadController.runInfo.PlayerTeam[0].wisdow += rewardCountWisdow;
            }
         }
         if (data.constitutionRewardMax != 0)
         {
            int rewardCountConstitution = Random.Range(data.constitutionRewardMin, data.constitutionRewardMax + 1);
            if(rewardCountConstitution != 0)
            {
               if (isNext)
               {
                  room.eventRewardText += ", ";
               }
               room.eventRewardText += $"{rewardCountConstitution} ������������";
               isNext = true;

               var hero = SaveLoadController.runInfo.PlayerTeam[0];
               hero.constitution += rewardCountConstitution;
               hero.TakeHeal(rewardCountConstitution * 5);
            }
         }
         if (data.armorRewardMax != 0)
         {
            int rewardCountArmor = Random.Range(data.armorRewardMin, data.armorRewardMax + 1);
            if(rewardCountArmor != 0)
            {
               if (isNext)
               {
                  room.eventRewardText += ", ";
               }
               room.eventRewardText += $"{rewardCountArmor} ������";
               isNext = true;

               SaveLoadController.runInfo.PlayerTeam[0].defence += rewardCountArmor;
            }
         }
         if (data.healRewardMax != 0)
         {
            var hero = SaveLoadController.runInfo.PlayerTeam[0];
            int randomKoef = Random.Range(data.healRewardMin, data.healRewardMax + 1);
            int rewardCountHeal = (int)(hero.max_hp * (randomKoef / 100.0f));
            
            if(rewardCountHeal != 0)
            {
               if (isNext)
               {
                  room.eventRewardText += ", ";
               }
               room.eventRewardText += $"{rewardCountHeal} ��������";
               isNext = true;

               hero.TakeHeal(rewardCountHeal);
            }
         }
         if (data.karmaMax != 0)
         {
            ushort rewardCountKarma = (ushort)Random.Range(data.karmaMin, data.karmaMax + 1);
            if (rewardCountKarma != 0)
            {
               if (isNext)
               {
                  room.eventRewardText += ", ";
               }
               room.eventRewardText += $"���� ������ ��� �������� ����";
               isNext = true;

               SaveLoadController.runInfo.badKarma += rewardCountKarma;
            }
         }

         bool isPool = false;
         List<SkillSO> rewardSkills = new(data.rewardSkillList);
         if (data.rewardSkillPool != null) 
         {
            rewardSkills = new(data.rewardSkillPool.allSkillList);
            isPool = true;
         }

         List<float> rewardChances = new(data.chanceToReceiveSkill);
         if (!data.allSkillsFromList && rewardSkills.Count != 1) 
         {
            Shuffle(rewardSkills, rewardChances, isPool);
         }

        for(int j = 0; j < rewardSkills.Count; j++)
         {
            var skillData = rewardSkills[j];
            var skill = SkillDB.Instance.GetSkillByName(skillData.name);
            if (!isPool && !DidChanceDrop(rewardChances[j]))
            {
               continue;
            }
            List<PlayableCharacter> availableCharacters = new();
            foreach (PlayableCharacter character in SaveLoadController.runInfo.PlayerTeam)
            {
               bool available = false;
               foreach(var pool in character.AvailableSkills)
               {
                  if (pool.allSkillList.Contains(skillData))
                  {
                     available = true;
                     break;
                  }
               }
               if (
                  available ||
                  skillData.isAllAvailable || data.ignoreSkillPools
                  )
               {
                  availableCharacters.Add(character);
               }
            }
            if (availableCharacters.Count == 0) continue;
            PlayableCharacter target = availableCharacters[Random.Range(0, availableCharacters.Count)];

            room.eventRewardText += "\n";
            room.eventRewardText += skillData.skill_target switch
            {
               SkillSO.SkillTarget.Passive => "���������",
               _ => "��������"
            };
     
            room.eventRewardText += $" ����� \"{skillData._name}\" � {skill.Description()}.\n�{skillData.quote}�";

            if (!target.CheckSkillCount(skill.skillData.skill_target))
               room.eventRewardText += " (����� ���)";

            target.AddSkill(skill);
            if(skillData.globalPassiveBuff != RunInfo.GlobalBuff.None)
            {
               SaveLoadController.GlobalBuffsUpdate();
            }
            isNext = true;
            if (!data.allSkillsFromList) break;
         }
         if (!isNext) room.eventRewardText += "������";
         room.eventRewardText += ".";
         room.eventRewardClaim = true;
      }
      text += room.eventRewardText;

      //�������� ������ ��� ���
      if(data.eventType == EventData.EventType.FightEvent ||
         data.eventType == EventData.EventType.BossEvent)
      {
         text += "\n\n";
         text += "�����: ";
         if (data.randomEnemies || data.isFog)
         {
            text += "������ �� ������� ������.";
         }
         else
         {
            var enemies = data.enemies;
            for (int j = 0; j < enemies.Count; j++)
            {
               text += enemies[j].character_name + $"(x{data.enemiesCount[j]})";
               if (j != enemies.Count - 1) text += ", ";
            }
            text += ".";
         }
      }
      eventText.text = text;

      if (data.isLockableEvent && !unlock) 
      {
         StartCoroutine(LockRoom());
      }
      else roomB.LockDoors(true);

      if (data.eventType == EventData.EventType.BossEvent)
      {
         roomB.WallUp();
      }
      if(data.eventType == EventData.EventType.BossWin)
      {
         roomB.UnWallUp();
         roomB.FogOff();
      }


         //�����
      fightBtn.SetActive(data.eventType == EventData.EventType.FightEvent 
            || data.eventType == EventData.EventType.BossEvent);
      trapBtn.SetActive(data.eventType == EventData.EventType.Trap);

      if (data.eventType == EventData.EventType.FightEvent 
         || data.eventType == EventData.EventType.BossEvent)
      {
         foreach (var go in eventBtns) go.SetActive(false);
         enemiesForFight = new();

         if (data.randomEnemies)
         {
            int count = Random.Range(data.minEnemyCount, data.maxEnemyCount + 1);

            // �������� �� ������������ ������
            if (data.enemies.Count != data.enemiesChances.Count)
            {
               Debug.LogError("���������� ������ � ���������� ������ �� ���������!");
               return;
            }

            for (int j = 0; j < count; j++)
            {
               int roll = Random.Range(0, 10000);
               int cumulative = 0;
               CharacterSO selectedEnemy = null;

               for (int k = 0; k < data.enemies.Count; k++)
               {
                  cumulative += (int)(data.enemiesChances[k] * 100);
                  if (roll < cumulative)
                  {
                     selectedEnemy = data.enemies[k];
                     break;
                  }
               }

               if (selectedEnemy != null)
               {
                  enemiesForFight.Add(new Fighter(selectedEnemy));
               }
               else
               {
                  Debug.LogWarning("�� ������� ������� �����, �������� ����� ������ < 100?");
               }
            }
         }
         else
         {
            for (int j = 0; j < data.enemies.Count; j++)
            {
               for(int k = 0; k < data.enemiesCount[j]; k++)
                  enemiesForFight.Add(new Fighter(data.enemies[j]));
            }
         }
         return;
      }


      //������ ������
      if(data.eventType == EventData.EventType.Trap)
      {
         foreach (var go in eventBtns) go.SetActive(false);

         return;
      }
      int i = 0;
      foreach (var eventData in data.choices)
      {
         bool isVisible = true;
         if (eventData.isHidden)
         {
            //�������� �������� ������ �� ������� �������
            var mainChar = SaveLoadController.runInfo.PlayerTeam[0];
            int chance = (mainChar.wisdow + mainChar.bonus_wisdow) * Fight.procentPerOneCharacteristic;
            if (chance > Fight.limitProcent) chance = Fight.limitProcent;
            int res = Random.Range(0, 100);
            isVisible = res < chance;
         }
         if (i == room.hiddenVariant.Count)
            room.hiddenVariant.Add(isVisible);
         eventBtns[i].SetActive(room.hiddenVariant[i]);
         eventBtnsText[i].text = eventData.textChoice;
         i++;
      }
      for (; i < eventBtns.Count; i++)
      {
         eventBtns[i].SetActive(false);
      }

      MiniMapUI.isNeedUpdate = true;
   }

   bool DidChanceDrop(float chance)
   {
      int scaledChance = Mathf.RoundToInt(chance * 100);
      int roll = Random.Range(0, 10000);
      return roll < scaledChance;
   }

   string SoulWord(int rewardCountSoul)
   {
      string soulWord;
      int lastDigit = rewardCountSoul % 10;
      int lastTwoDigits = rewardCountSoul % 100;

      if (lastTwoDigits >= 11 && lastTwoDigits <= 14)
      {
         soulWord = "���";
      }
      else
      {
         switch (lastDigit)
         {
            case 1:
               soulWord = "����";
               break;
            case 2:
            case 3:
            case 4:
               soulWord = "����";
               break;
            default:
               soulWord = "���";
               break;
         }
      }

      return soulWord;
   }
   private void Shuffle(List<SkillSO> list, List<float> list2, bool ignoreList2)
   {
      if (list.Count != list2.Count && !ignoreList2)
      {
         Debug.LogError("������ ������ �����! Shuffle ����������.");
         return;
      }

      System.Random rng = new System.Random();
      int n = list.Count;
      while (n > 1)
      {
         n--;
         int k = rng.Next(n + 1);

         // ������ �������� � ������ ������
         SkillSO tempSkill = list[k];
         list[k] = list[n];
         list[n] = tempSkill;

         if (ignoreList2) continue;
         // ������ ��������������� �������� �� ������ ������
         float tempValue = list2[k];
         list2[k] = list2[n];
         list2[n] = tempValue;
      }
   }



   IEnumerator LockRoom()
   {
      foreach(var go in invisibleWalls)
      {
         go.SetActive(true);
      }
      roomB.LockDoors(false);

      yield return new WaitForSeconds(1f);

      foreach (var go in invisibleWalls)
      {
         go.SetActive(false);
      }
   }

   public void ChoiceBtnClick(int choiceID)
   {
      switch (choiceID)
      {
         case 3:
            SaveLoadController.Save();
            Fight.eventRoom = room;
            SaveLoadController.StartFight(enemiesForFight);
            break;
         case 4:
            //�������� �� �������� ����� ������ ������� �� �������
            var mainChar = SaveLoadController.runInfo.PlayerTeam[0];
            int chance = 0;
            switch (data.checkTrap)
            {
               case EventData.Characteristic.None:
               case EventData.Characteristic.Agility:
                  chance = (mainChar.agility) * Fight.procentPerOneCharacteristic;
                  break;
               case EventData.Characteristic.Strengh:
                  chance = (mainChar.strengh) * Fight.procentPerOneCharacteristic;
                  break;
               case EventData.Characteristic.Wisdow:
                  chance = (mainChar.wisdow) * Fight.procentPerOneCharacteristic;
                  break;
               case EventData.Characteristic.Consitution:
                  chance = (mainChar.constitution) * Fight.procentPerOneCharacteristic;
                  break;
               case EventData.Characteristic.Armor:
                  chance = (mainChar.armor) * Fight.procentPerOneCharacteristic;
                  break;
            }
            if (chance > Fight.limitProcent) chance = Fight.limitProcent;
            int res = Random.Range(0, 100);
            room.eventData = (res < chance) ? data.choices[1] : data.choices[0];
            data = room.eventData;
            room.eventName = $"{data.eventID}-{data.eventID_Part}";
            FillEvent();
            SaveLoadController.Save();
            break;
         default:
            room.eventData = data.choices[choiceID];
            data = room.eventData;
            room.eventName = $"{data.eventID}-{data.eventID_Part}";
            FillEvent();
            SaveLoadController.Save();
            break;
      }
   }
}
