using KeySystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Room3D : MonoBehaviour
{
   public Room room;

   public TextMeshProUGUI eventTitle;
   public TextMeshProUGUI eventText;
   public List<TextMeshProUGUI> eventBtnsText;
   public List<GameObject> eventBtns;
   public GameObject fightBtn;

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
      if(room != null && !isFilled)
      {
         isFilled = true;
         FillEvent();
      }
   }

   public void FillEvent()
   {

      if (data.eventType == EventData.EventType.EnteranceEvent && 
         SaveLoadController.runInfo.currentRoom != room)
      {
         eventCG.alpha = 0;
         eventCG.interactable = false;
         eventCG.blocksRaycasts = false;
         isFilled = false;
         return;
      }
      eventCG.alpha = 1;
      eventCG.interactable = true;
      eventCG.blocksRaycasts = true;

      eventTitle.text = data.eventName;
      string text = data.eventText;

      //�������
      if(data.choices.Count == 0 && !room.eventRewardClaim)
      {
         //��������� ���������� � �������
         room.eventRewardText = "";
         room.eventRewardText += "\n\n";
         room.eventRewardText += "��������: ";
         bool isNext = false;

         if(data.minGold != 0)
         {
            int rewardCountGold = Random.Range(data.minGold, data.maxGold + 1);
            if(rewardCountGold != 0)
            {
               if (isNext)
               {
                  room.eventRewardText += ", ";
               }
               room.eventRewardText += $"{rewardCountGold} ������";
               isNext = true;

               SaveLoadController.runInfo.goldCount += rewardCountGold;
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
               hero.hp += rewardCountConstitution * 5;
               if (hero.hp > hero.max_hp)
               {
                  hero.hp = hero.max_hp;
               }
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
               room.eventRewardText += $"{rewardCountArmor} �����";
               isNext = true;

               SaveLoadController.runInfo.PlayerTeam[0].defence += rewardCountArmor;
            }
         }
         if (data.healRewardMax != 0)
         {
            int rewardCountHeal = Random.Range(data.healRewardMin, data.healRewardMax + 1);
            if(rewardCountHeal != 0)
            {
               if (isNext)
               {
                  room.eventRewardText += ", ";
               }
               room.eventRewardText += $"{rewardCountHeal} ��������";
               isNext = true;

               var hero = SaveLoadController.runInfo.PlayerTeam[0];
               hero.hp += rewardCountHeal;
               if (hero.hp > hero.max_hp)
               {
                  hero.hp = hero.max_hp;
               }
            }
         }

         bool isPool = false;
         List<SkillSO> rewardSkills = new(data.rewardSkillList);
         if (data.rewardSkillPool != null) 
         {
            rewardSkills = new(data.rewardSkillPool.skillList);
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
            if (!isPool && !DidSkillDrop(rewardChances[j]))
            {
               continue;
            }
            var classes = skillData.availableClasses;
            List<PlayableCharacter> availableCharacters = new();
            foreach (PlayableCharacter character in SaveLoadController.runInfo.PlayerTeam)
            {
               if (skillData.availableClasses.Contains(character.charClass))
               {
                  availableCharacters.Add(character);
               }
            }
            if (availableCharacters.Count == 0) continue;
            PlayableCharacter target = availableCharacters[Random.Range(0, availableCharacters.Count)];

            room.eventRewardText += "\n";
            room.eventRewardText += skillData.skill_type switch
            {
               SkillSO.SkillType.Passive_Battle => "��������� (���)",
               SkillSO.SkillType.Passive_Global => "��������� (����������)",
               _ => "��������"
            };
            room.eventRewardText += $" ����� \"{skillData._name}\" � {skill.Description(target)}.\n�{skillData.quote}�";

            target.AddSkill(skill);
            isNext = true;
            if (!data.allSkillsFromList) break;
         }
         if (!isNext) room.eventRewardText += "������";
         room.eventRewardText += ".";
         room.eventRewardClaim = true;
      }
      text += room.eventRewardText;
      eventText.text = text;

      if (data.isLockableEvent) 
      {
         StartCoroutine(LockRoom());
      }
      else roomB.LockDoors(true);

      //�����, ����� ���������
      fightBtn.SetActive(data.enemies.Count > 0);
      if (data.enemies.Count > 0)
      {
         foreach (var go in eventBtns) go.SetActive(false);
         enemiesForFight = new();
         foreach (CharacterSO enemy in data.enemies)
         {
            enemiesForFight.Add(new Fighter(enemy));
         }
         return;
      }
      int i = 0;
      foreach (var eventData in data.choices)
      {
         eventBtns[i].SetActive(true);
         eventBtnsText[i].text = eventData.textChoice;
         i++;
      }

      for (; i < eventBtns.Count; i++)
      {
         eventBtns[i].SetActive(false);
      }
   }

   bool DidSkillDrop(float chanceToReceiveSkill)
   {
      int scaledChance = Mathf.RoundToInt(chanceToReceiveSkill * 100);
      int roll = UnityEngine.Random.Range(0, 10000);
      return roll < scaledChance;
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
      if(choiceID == 3)
      {
         SaveLoadController.Save();
         Fight.eventRoom = room;
         SaveLoadController.StartFight(enemiesForFight);
      }
      else
      {
         room.eventData = data.choices[choiceID];
         data = room.eventData;
         room.eventName = $"{data.eventID}-{data.eventID_Part}";
         FillEvent();
         SaveLoadController.Save();
      }
   }
}
