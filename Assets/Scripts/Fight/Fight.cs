using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fight : MonoBehaviour
{
   public GridLayoutGroup heroes, enemies;

   public CanvasGroup playerHand;

   public static List<Fighter> AllCharacter
   {
      get
      {
         var list = new List<Fighter>(EnemyTeam);
         foreach (Fighter hero in PlayerTeam)
            list.Add(hero);
         return list;
      }
   }

   public static List<PlayableCharacter> PlayerTeam;
   public static List<Fighter> PlayerUITeam;

   public static List<Fighter> EnemyTeam;
   public static List<Fighter> EnemyUITeam;

   public static List<Fighter> AlreadyTurn;

   public int round_number = 1;

   public List<SpriteRenderer> HeroesSprites;
   public List<SpriteRenderer> EnemiesSprites;
   public List<SpriteRenderer> AddictSprites;

   public List<GameObject> SkillsCards;

   public GameObject WinLosePanel;
   public TMP_Text WinLoseText;
   public TMP_Text RoundNum;

   private static int SelectedCharacterID = -1;
   private static bool _selected = false;
   public static Skill selectedSkill = null;
   public static Fighter selectedTarget = null;

   public static bool needUpdatePortrait = false;

   public static bool cast = false;

   private bool allEnemiesDoTurn, allPlayerCharactersDoTurn;
   private bool isWin, isLose, isAllDoTurn;
   public static bool endFight = false;
   public static bool isEnemyTurn = false;

   public static Room eventRoom; //Откуда запустилась битва
   private void Start()
   {
      Cursor.lockState = CursorLockMode.None;

      playerHand.alpha = 1f;
      playerHand.interactable = true;
      playerHand.blocksRaycasts = true;

      SelectedCharacterID = -1;
      _selected = false;
      selectedSkill = null;
      selectedTarget = null;
      needUpdatePortrait = false;
      cast = false;
      isEnemyTurn = false;
      endFight = false;

      PlayerTeam = SaveLoadController.runInfo.PlayerTeam;
      EnemyTeam = SaveLoadController.enemies;

      //Summon Buffs
      for (int i = 0; i < PlayerTeam.Count; i++)
      {
         var character = PlayerTeam[i];
         foreach (var skill in character.skills)
         {
            var data = skill.skillData;
            if(data.skill_type == SkillSO.SkillType.Summon &&
               data.skill_target == SkillSO.SkillTarget.Passive)
            {
               switch (data.passiveBuff)
               {
                  case Fighter.Buff.ScreamIntoVoid:
                     //Призываем 3 духов разного типа стартовых персонажей
                     //Сумма их статов = сумме параметров жрицы, распределённых с упором на главные характеристики
                     //Когда сумма параметров Жрицы достигает определённого порога, они получают новые активные навыки из сета персонажа (случайно)
                     int count = 3;
                     int sumParam = character.strengh + character.agility + character.wisdow
                        + character.constitution + character.defence;

                     // Разделяем общее количество параметров между духами
                     int[] statsPerSpirit = DistributeStats(sumParam, count);

                     for (int k = 0; k < count; k++)
                     {
                        var list = new List<CharacterSO>(Resources.LoadAll<CharacterSO>("CharData/Playable/"));
                        list.Remove(character.Data);
                        var randomClass = list[Random.Range(0, list.Count)];
                        var ally = new PlayableCharacter(randomClass.name)
                        {
                           isSummon = true
                        };

                        int spiritStats = statsPerSpirit[k];
                        var stats = new Dictionary<CharacterSO.Stat, int>
                        {
                            { CharacterSO.Stat.Strengh, 0 },
                            { CharacterSO.Stat.Agility, 0 },
                            { CharacterSO.Stat.Wisdow, 0 },
                            { CharacterSO.Stat.Constitution, 1 }, // Минимум 1
                            { CharacterSO.Stat.Defence, 0 }
                        };
                        int remaining = spiritStats - 1; // уже потратили 1 на constitution

                        var priorities = randomClass.priorityStats;
                        int priorityShare = Mathf.RoundToInt(remaining * 0.75f);
                        int nonPriorityShare = remaining - priorityShare;

                        // Распределение приоритетных очков
                        for (int j = 0; j < priorityShare; j++)
                        {
                           CharacterSO.Stat stat = priorities[Random.Range(0, priorities.Count)];
                           stats[stat]++;
                        }

                        // Распределение оставшихся очков
                        var nonPriorities = stats.Keys.Except(priorities).ToList();
                        for (int j = 0; j < nonPriorityShare; j++)
                        {
                           CharacterSO.Stat stat = nonPriorities[Random.Range(0, nonPriorities.Count)];
                           stats[stat]++;
                        }

                        // Присваиваем статы
                        ally.strengh = stats[CharacterSO.Stat.Strengh];
                        ally.agility = stats[CharacterSO.Stat.Agility];
                        ally.wisdow = stats[CharacterSO.Stat.Wisdow];
                        ally.constitution = stats[CharacterSO.Stat.Constitution];
                        ally.defence = stats[CharacterSO.Stat.Defence];

                        ally.FullHeal();

                        while(ally.skills.Count != 1 && sumParam < 25)
                           ally.skills.RemoveAt(1);

                        if (sumParam >= 50)
                        {
                           var skillPools = ally.Data.availableSkills;

                           int skillsToAdd = (sumParam - 50) / 25 + 1; // Например: 50 → 1, 75 → 2, 100 → 3

                           List<SkillSO> availableSkills = new();
                           foreach(SkillPool pool in skillPools)
                           {
                              foreach(SkillSO skillSO in pool.activeSkillList)
                              {
                                 if (!availableSkills.Contains(skillSO))
                                 {
                                    availableSkills.Add(skillSO);
                                 }
                              }
                           }

                           availableSkills.Remove(ally.skills[1].skillData);

                           for (int j = 0; j < skillsToAdd && availableSkills.Count > 0; j++)
                           {
                              var skill1 = availableSkills[Random.Range(0, availableSkills.Count)];
                              ally.AddSkill(skill1);
                              availableSkills.Remove(skill1); // Чтобы не повторялись
                           }
                        }

                        ally.AddSkill("Corpseless");


                        if (PlayerTeam.Count < 6)
                           PlayerTeam.Add(ally);
                     }
                     break;
               }
            }
         }
      }

      PlayerUITeam = new List<Fighter>(PlayerTeam);
      EnemyUITeam = new List<Fighter>(EnemyTeam);

      SelectedCharacterID_Reset();

      AlreadyTurn = new List<Fighter>();
      //AllCharacter = new List<Fighter>();
      //foreach (Fighter character in PlayerTeam) AllCharacter.Add(character);
      //foreach (Fighter character in EnemyTeam) AllCharacter.Add(character);


      //Подготовка персонажей
      foreach (Fighter character in AllCharacter)
      {
         character.armor = character.defence * 2;
         character.bonus_hp = 0;
         character.bonus_strengh = 0;
         character.bonus_wisdow = 0;
         character.bonus_agility = 0;
         character.buffs = new();
      }

      UpdatePortrait();

      FightUIController.hardUpdate = true;

      FightStart();
   }

   private void Update()
   {
      //Возврат по правой кнопки мыши
      if (Input.GetKeyDown(KeyCode.Mouse1) && selectedSkill == null)
      {
         CardShowerReset();
         SelectedCharacterID_Reset();
         _selected = false;
         PlayerUITeam = new List<Fighter>(PlayerTeam);
         UpdatePortrait();
         FightUIController.hardUpdate = true;
      }
      else if (Input.GetKeyDown(KeyCode.Mouse1))
      {
         selectedSkill = null;
         EnemyUITeam = new(EnemyTeam);
         UpdatePortrait();
         FightUIController.hardUpdate = true;
      }

      //Вывод выбранного персонажа
      if (SelectedCharacterID != -1 && !_selected)
      {
         _selected = true;
         PlayerUITeam = new List<Fighter> { PlayerTeam[SelectedCharacterID] };
         UpdatePortrait();
      }

      //Обновление по требованию
      if (needUpdatePortrait)
      {
         needUpdatePortrait = false;
         UpdatePortrait();
      }
      RoundNum.text = "Раунд: " + round_number;
   }

   int[] DistributeStats(int total, int count)
   {
      int[] result = new int[count];
      int remaining = total;

      for (int i = 0; i < count; i++)
      {
         result[i] = total / count;
         remaining -= result[i];
      }

      while (remaining > 0)
      {
         int index = Random.Range(0, count);
         result[index]++;
         remaining--;
      }

      return result;
   }

   public void CardShowerReset()
   {
      foreach (var SkCard in SkillsCards)
      {
         SkCard.GetComponent<CardShower>().isFirstMove = false;
         SkCard.GetComponent<CardShower>().isLock = false;
      }
      Skill_Image.isOneLocked = false;
      Skill_Image.isNeedClose = true;
   }

   private void FightStart()
   {
      foreach (Fighter character in AllCharacter)
      {
         character.Spawn();
      }

      //MakeIntention();

      //StartFight Buffs
      foreach (Fighter character in AllCharacter)
      {
         foreach (var buff in character.buffs)
         {
            switch (buff)
            {
               case Fighter.Buff.QuickRebuff:
                  character.CastSkill(new List<Fighter>() { RandomEnemy(character) }, SkillDB.Instance.GetSkillByName("Basic Attack"));
                  break;
            }
         }
      }

      StartCoroutine(PlayerTurn());
   }

   public void SelectedCharacterID_Reset()
   {
      SelectedCharacterID = -1;
      if (PlayerTeam.Count == 1)
         SelectedCharacterID = 0;
   }

   IEnumerator PlayerTurn()
   {
      if (allPlayerCharactersDoTurn) goto Skip;
      SelectedCharacterID_Reset();
      StartTurn:
      selectedSkill = null;
      selectedTarget = null;
      CardShowerReset();

      //Ждём пока не выберется скилл
      ChangeSkill:
      EnemyUITeam = new List<Fighter>(EnemyTeam);
      UpdatePortrait();
      while (selectedSkill == null) yield return null;

      //Кто отображается в списке персонажей справа
      PlayerUITeam = new List<Fighter>() { SelectedCharacter() };
      switch (selectedSkill.skillData.skill_target)
      {
         case SkillSO.SkillTarget.Solo_Enemy:
         case SkillSO.SkillTarget.Mass_Enemies:
         case SkillSO.SkillTarget.Random_Enemy:
            EnemyUITeam = new List<Fighter>(EnemyTeam);
            break;
         case SkillSO.SkillTarget.All:
         case SkillSO.SkillTarget.Random_Target:
            EnemyUITeam = new List<Fighter>(AllCharacter);
            break;
         case SkillSO.SkillTarget.Solo_Ally:
         case SkillSO.SkillTarget.Mass_Allies:
         case SkillSO.SkillTarget.Random_Ally:
            EnemyUITeam = new List<Fighter>(PlayerTeam);
            break;
         case SkillSO.SkillTarget.Caster:
            EnemyUITeam = new List<Fighter>() { SelectedCharacter() };
            break;
            
      }
      UpdatePortrait();
      FightUIController.allInteractable = true;
      FightUIController.hardUpdate = true;

      var temp_skill = selectedSkill;

      //Ждём выбора цели
      while (selectedTarget == null)
      {
         yield return null;
         if (Input.GetKeyDown(KeyCode.Mouse1))
         {
            FightUIController.allInteractable = false;
            _selected = false;
            goto StartTurn;
         }

         if(temp_skill != selectedSkill)
         {
            FightUIController.allInteractable = false;
            goto ChangeSkill;
         }
      }
      //Выбираем цели для каста
      var selectedTargets = ChooseTarget(selectedSkill, SelectedCharacter(), selectedTarget);
      
      FightUIController.allInteractable = false;
      _selected = false;

      PlayerUITeam = new List<Fighter> { PlayerTeam[SelectedCharacterID] };
      EnemyUITeam = selectedTargets;
      cast = true;
      UpdatePortrait();

      SelectedCharacter().CastSkill(selectedTargets, selectedSkill);

      AlreadyTurn.Add(SelectedCharacter());

      yield return new WaitForSeconds(2f);

      PlayerUITeam = new List<Fighter>(PlayerTeam);
      EnemyUITeam = new List<Fighter>(EnemyTeam);

      selectedSkill = null;
      selectedTarget = null;
      SelectedCharacterID_Reset();
      _selected = false;

      cast = false;
      UpdatePortrait();

      Skip:
      CardShowerReset();
      CheckRoundChange();
      if (!isWin && !isLose) 
      {
         if (isAllDoTurn) StartCoroutine(PlayerTurn());
         else StartCoroutine(EnemyTurn());
      }
   }

   IEnumerator EnemyTurn()
   {
      if (allEnemiesDoTurn) goto Skip;
      var notTurnEnemies = new List<Fighter>();
      foreach (Fighter figh in EnemyTeam)
      {
         if (!AlreadyTurn.Contains(figh) && !figh.isDead) notTurnEnemies.Add(figh);
      }
      if (notTurnEnemies.Count == 0) goto Skip;

      isEnemyTurn = true;
      UpdatePortrait();
      FightUIController.allDisable = true;
      FightUIController.hardUpdate = true;

      yield return new WaitForSeconds(2f);

      var selectedEnemy = notTurnEnemies[Random.Range(0, notTurnEnemies.Count)];

      //EnemyUITeam = new List<Fighter>() { selectedEnemy };
      //UpdatePortrait();
      //Реализовать логику выбора цели
      Fighter target;
      do
      {
         target = PlayerTeam[Random.Range(0, PlayerTeam.Count)];
      } while (target.isDead); //выбираем не мёртвую цель

      var selectedTargets = ChooseTarget(selectedEnemy.Intension, selectedEnemy, target);
      
      Debug.Log("Использует умение: " + selectedEnemy.Intension.skillData._name);
      PlayerUITeam = selectedTargets;
      EnemyUITeam = new List<Fighter> { selectedEnemy };
      cast = true;
      Skill_Image.externalSkill = selectedEnemy.Intension;
      UpdatePortrait();

      yield return new WaitForSeconds(2f);

      selectedEnemy.CastSkill(selectedTargets);
      yield return null;
      cast = false;
      Skill_Image.isDisableExSkill = true;
      yield return null;
      PlayerUITeam = new List<Fighter>(PlayerTeam);
      EnemyUITeam = new List<Fighter>(EnemyTeam);
      AlreadyTurn.Add(selectedEnemy);
      FightUIController.allDisable = false;

      UpdatePortrait();
      isEnemyTurn = false;
      Skip:
      CheckRoundChange();
      if(!isWin && !isLose) StartCoroutine(PlayerTurn());
   }

   private void CheckRoundChange()
   {
      allPlayerCharactersDoTurn = true;
      foreach (Fighter fighter in PlayerTeam)
         if (!AlreadyTurn.Contains(fighter) && !fighter.isDead)
         {
            allPlayerCharactersDoTurn = false;
            break;
         }
      allEnemiesDoTurn = true;
      foreach (Fighter fighter in EnemyTeam)
         if (!AlreadyTurn.Contains(fighter) && !fighter.isDead)
         {
            allEnemiesDoTurn = false;
            break;
         }
      //Check WinLose
      isWin = true;
      foreach (Fighter fighter in EnemyTeam)
         if (!fighter.isDead)
         {
            isWin = false;
            break;
         }
      isLose = true;
      foreach (Fighter fighter in PlayerTeam)
         if (!fighter.isDead)
         {
            isLose = false;
            break;
         }
      if (isLose || isWin)
      {
         EndFight();
         return;
      }
      isAllDoTurn = allEnemiesDoTurn && allPlayerCharactersDoTurn;
      if (isAllDoTurn)
      {
         AlreadyTurn.Clear();
         allEnemiesDoTurn = false;
         allPlayerCharactersDoTurn = false;
         round_number++;
         Debug.Log("Следующий раунд: " + round_number.ToString());

         foreach(var hero in AllCharacter)
         {
            var keys = new List<Skill>(hero.cooldowns.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
               Skill skill = keys[i];
               hero.cooldowns[skill]--;

               if (hero.cooldowns[skill] == 0)
               {
                  hero.cooldowns.Remove(skill);
               }
            }
         }

         foreach(var enemy in EnemyTeam)
            enemy.MakeIntention();
      }
   }

   private void EndFight() 
   {
      StopAllCoroutines();
      endFight = true;
      Skill_Image.isNeedClose = true;
      WinLosePanel.SetActive(true);

      playerHand.alpha = 0f;
      playerHand.interactable = false;
      playerHand.blocksRaycasts = false;

      if (isLose) WinLoseText.text = "К сожалению, вы проиграли";
      else if (isWin) WinLoseText.text = "Поздравляю с победой";
      eventRoom.eventData = eventRoom.eventData.choices[0];
      var data = eventRoom.eventData;
      eventRoom.eventName = $"{data.eventID}-{data.eventID_Part}";
      SaveLoadController.EndFight();
      SaveLoadController.Save();
   }

   public static void SelectCharacter(int id)
   {
      SelectedCharacterID = id;
   }
   public static PlayableCharacter SelectedCharacter()
   {
      if (SelectedCharacterID == -1) return null;
      return PlayerTeam[SelectedCharacterID];
   }
   public static void SelectTarget(int id)
   {
      selectedTarget = id < 6 ? PlayerTeam[id] : EnemyTeam[id - 6];
   }

   public static Fighter RandomEnemy(Fighter enemyForMe)
   {
      List<Fighter> enemies = new(EnemyTeam);
      if (EnemyTeam.Contains(enemyForMe))
      {
         enemies = new(PlayerTeam);
      }
      for (int i = 0; i < enemies.Count; i++)
      {
         if(enemies[i].isDead)
         {
            enemies.RemoveAt(i);
            i--;
         }
      }
      return enemies[Random.Range(0, enemies.Count)];
   }

   public static bool IsEnemy(Fighter me, Fighter him)
   {
      return !(PlayerTeam.Contains(me) && PlayerTeam.Contains(him)
         ||
         EnemyTeam.Contains(me) && EnemyTeam.Contains(him));
   }

   public static List<Fighter> ChooseTarget(Skill selectedSkill, Fighter caster, Fighter selectedTarget)
   {
      List<Fighter> allies = new(PlayerTeam);
      List<Fighter> enemies = new(EnemyTeam);
      if(EnemyTeam.Contains(caster))
      {
         allies = new(EnemyTeam);
         enemies = new(PlayerTeam);
      }
      List<Fighter> selectedTargets = new();
      switch (selectedSkill.skillData.skill_target)
      {
         case SkillSO.SkillTarget.Solo_Enemy:
         case SkillSO.SkillTarget.Solo_Ally:
            selectedTargets.Add(selectedTarget);
            break;
         case SkillSO.SkillTarget.Caster:
            selectedTargets.Add(SelectedCharacter());
            break;
         case SkillSO.SkillTarget.Mass_Enemies:
            selectedTargets = new List<Fighter>(EnemyTeam);
            if (!selectedSkill.skillData.isCorpseTargetToo)
            {
               for (int i = 0; i < selectedTargets.Count; i++)
               {
                  if (selectedTargets[i].isDead)
                  {
                     selectedTargets.Remove(selectedTargets[i]);
                     i--;
                  }
               }
            }
            break;
         case SkillSO.SkillTarget.All:
            selectedTargets = new List<Fighter>(AllCharacter);
            if (!selectedSkill.skillData.isCorpseTargetToo)
            {
               for (int i = 0; i < selectedTargets.Count; i++)
               {
                  if (selectedTargets[i].isDead)
                  {
                     selectedTargets.Remove(selectedTargets[i]);
                     i--;
                  }
               }
            }
            break;
         case SkillSO.SkillTarget.Random_Enemy:
            selectedTargets = new List<Fighter>() { EnemyTeam[Random.Range(0, EnemyTeam.Count)] };
            if (!selectedSkill.skillData.isCorpseTargetToo)
            {
               do
               {
                  selectedTargets = new List<Fighter>() { EnemyTeam[Random.Range(0, EnemyTeam.Count)] };
               } while (selectedTargets[0].isDead);
            }
            break;
         case SkillSO.SkillTarget.Mass_Allies:
            selectedTargets = new List<Fighter>(PlayerTeam);
            if (!selectedSkill.skillData.isCorpseTargetToo)
            {
               for (int i = 0; i < selectedTargets.Count; i++)
               {
                  if (selectedTargets[i].isDead)
                  {
                     selectedTargets.Remove(selectedTargets[i]);
                     i--;
                  }
               }
            }
            break;
         case SkillSO.SkillTarget.Random_Ally:
            selectedTargets = new List<Fighter>() { PlayerTeam[Random.Range(0, PlayerTeam.Count)] };
            if (!selectedSkill.skillData.isCorpseTargetToo)
            {
               do
               {
                  selectedTargets = new List<Fighter>() { PlayerTeam[Random.Range(0, PlayerTeam.Count)] };
               } while (selectedTargets[0].isDead);
            }
            break;
         case SkillSO.SkillTarget.Random_Target:
            selectedTargets = new List<Fighter>() { AllCharacter[Random.Range(0, AllCharacter.Count)] };
            if (!selectedSkill.skillData.isCorpseTargetToo)
            {
               do
               {
                  selectedTargets = new List<Fighter>() { AllCharacter[Random.Range(0, AllCharacter.Count)] };
               } while (selectedTargets[0].isDead);
            }
            break;
      }
      return selectedTargets;
   }

   /*
   private void MakeIntention()
   {
      foreach (Fighter figh in EnemyTeam) 
      {
         figh.prevIntension = figh.Intension;

         //Количество скиллов без пассивок (используемых скиллов)
         int skillCount = 0;
         foreach(Skill skill in figh.skills)
         {
            if (skill.skillData.skill_target != SkillSO.SkillTarget.Passive) skillCount++;
         }
         bool reroll;
         if(skillCount > 0)
            do
            {
               var skills = figh.skills;
               figh.Intension = skills[Random.Range(0, skills.Count)];
               if (figh.prevIntension != null)
                  reroll = figh.Intension.skillData.skill_target == SkillSO.SkillTarget.Passive ||
                           (skillCount > 1 && figh.Intension == figh.prevIntension);
               else reroll = figh.Intension.skillData.skill_target == SkillSO.SkillTarget.Passive;
            } while (reroll);
      }
   }*/

   private void UpdatePortrait()
   {
      FightUIController.CountHeroes = PlayerUITeam.Count;
      FightUIController.CountEnemies = EnemyUITeam.Count;

      for (int i = 0; i < PlayerUITeam.Count; i++)
      {
         if(i > 5)
            AddictSprites[i - 6].sprite = PlayerUITeam[i].Portrait;
         else
            HeroesSprites[i].sprite = PlayerUITeam[i].Portrait;
      }
      for (int i = 0; i < EnemyUITeam.Count; i++)
      {
         if (i > 5)
            AddictSprites[i - 6].sprite = EnemyUITeam[i].Portrait;
         else
            EnemiesSprites[i].sprite = EnemyUITeam[i].Portrait;
      }
      if(PlayerUITeam.Count == 1)
      {
         int i = 0;
         if(!isEnemyTurn)
         {
            foreach (Skill skill in PlayerUITeam[0].skills)
            {
               if (skill.skillData.skill_target == SkillSO.SkillTarget.Passive) continue;
               var SkCard = SkillsCards[i];
               SkCard.GetComponent<Graphic>().enabled = true;
               Show(SkCard);
               SkCard.GetComponent<CardFiller>().skill = skill;
               SkCard.GetComponent<Skill_Image>().skill = skill;

               var contain = PlayerUITeam[0].cooldowns.Keys.Contains(skill);
               SkCard.GetComponent<Image>().raycastTarget = !contain;
               SkCard.transform.GetChild(2).gameObject.SetActive(contain);
               if (contain)
               {
                  SkCard.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = PlayerUITeam[0].cooldowns[skill].ToString();
               }
               //SkIm.sprite = skill.Icon;
               //SkIm.gameObject.GetComponent<Skill_Image>().skill = skill;
               i++;
            }
            for(;i< SkillsCards.Count; i++)
            {
               Hide(SkillsCards[i]);
            }
         }
         else
         {
            AllHide();
         }
      }
      else
      {
         AllHide();
      }
   }

   void AllHide()
   {
      foreach (var SkCard in SkillsCards)
      {
         Hide(SkCard);
      }
   }

   void Show(GameObject go)
   {
      go.GetComponent<LayoutElement>().ignoreLayout = false;
      go.GetComponent<Graphic>().enabled = true;
      ShowChildren(go);
   }
   void Hide(GameObject go)
   {
      go.GetComponent<LayoutElement>().ignoreLayout = true;
      HideChildren(go);
      go.GetComponent<Graphic>().enabled = false;
   }
   void HideChildren(GameObject go)
   {
      Graphic[] lChildRenderers = go.GetComponentsInChildren<Graphic>();
      foreach (Graphic lRenderer in lChildRenderers)
      {
         lRenderer.enabled = false;
      }
   }
   void ShowChildren(GameObject go)
   {
      Graphic[] lChildRenderers = go.GetComponentsInChildren<Graphic>();
      foreach (Graphic lRenderer in lChildRenderers)
      {
         lRenderer.enabled = true;
      }
   }

   public void ContinueBtn()
   {
      if (isLose)
      {
         SaveLoadController.ClearSave(SaveLoadController.slot);
         SceneManager.LoadScene(0);
      }
      else
      {
         SceneManager.LoadScene(1);
      }
   }
}
