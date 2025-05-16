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

   public GameObject skipBtn;

   public GameObject WinLosePanel;
   public TMP_Text WinLoseText;
   public TMP_Text RoundNum;

   private static int SelectedCharacterID = -1;
   private static bool _selected = false;
   public static Skill selectedSkill = null;
   public static Fighter selectedTarget = null;

   public static bool needUpdatePortrait = false;

   public static bool cast = false;
   public static bool seeIntension = false;

   private bool allEnemiesDoTurn, allPlayerCharactersDoTurn;
   public static bool isWin, isLose, isAllDoTurn;
   public static bool endFight = false;
   public static bool isEnemyTurn = false;
   public static bool skipTurn = false;

   public static List<CastSkillStructure> additionalCastSkills = new();

   public static Room eventRoom; //Откуда запустилась битва

   public static readonly int procentPerOneCharacteristic = 5; // Для проверок влияния хар-к
   public static readonly int limitProcent = 80;
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
      skipTurn = false;
      additionalCastSkills = new();

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
                           var skillPools = ally.AvailableSkills;

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
                  case Fighter.Buff.WanderingMusician:
                     Fighter summon = new("Summons/Accordion Accompanist");
                     summon.FullHeal();
                     if (PlayerTeam.Count < 6)
                        PlayerTeam.Add(summon as PlayableCharacter);
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

      FightUIController.hardUpdate = true;

      FightStart();
   }

   private void Update()
   {
      //Возврат по правой кнопки мыши
      if (Input.GetKeyDown(KeyCode.Mouse1) && selectedSkill == null && !isEnemyTurn
         && !BuffPanelController.isOpened && PlayerTeam.Count != 1)
      {
         CardShowerReset();
         SelectedCharacterID_Reset();
         _selected = false;
         PlayerUITeam = new List<Fighter>(PlayerTeam);
         UpdatePortrait();
         FightUIController.hardUpdate = true;
         skipBtn.SetActive(false);
      }
      else if (Input.GetKeyDown(KeyCode.Mouse1) && !isEnemyTurn && !BuffPanelController.isOpened && selectedSkill != null)
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
         if(PlayerTeam.Count != 1)
            skipBtn.SetActive(true);
      }

      //Обновление по требованию
      if (needUpdatePortrait)
      {
         needUpdatePortrait = false;
         UpdatePortrait();
      }
      RoundNum.text = "Раунд: " + round_number + (seeIntension ? " (намерения видны)" : "");
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
         character.isSpawn = false;
         character.Spawn();
      }

      UpdatePortrait();
      //MakeIntention();

      //StartFight Buffs
      foreach (Fighter character in AllCharacter)
      {
         foreach (var buff in character.buffs)
         {
            switch (buff)
            {
               case Fighter.Buff.QuickRebuff:
                  character.CastSkill(new List<Fighter>() { RandomEnemy(character) }, SkillDB.Instance.GetSkillByName("Basic Attack"), false);
                  break;
               case Fighter.Buff.EchoHope:
                  int heal = (int)(character.max_hp * 0.15f);
                  if (heal == 0) heal = 1;
                  character.TakeHeal(character, heal);
                  break;
               case Fighter.Buff.NoAttack:
                  var skill = SkillDB.Instance.GetSkillByName("Basic Attack");
                  if (character.skillsBattle.Contains(skill))
                  {
                     character.skillsBattle.Remove(skill);
                  }
                  break;
            }
         }
      }

      StartRound();
   }

   public void StartRound()
   {
      //Проверка на просмотр намерений:
      var mainChar = PlayerTeam[0];
      int chance = (mainChar.wisdow + mainChar.bonus_wisdow) * procentPerOneCharacteristic;
      if (chance > limitProcent) chance = limitProcent;
      int res = Random.Range(0, 100);
      seeIntension = res < chance;

      foreach (var enemy in EnemyTeam)
      {
         //Проверка на страх:
         chance = ( (mainChar.strengh + mainChar.bonus_strengh) - (enemy.strengh + enemy.bonus_strengh) ) * procentPerOneCharacteristic;
         if (chance > limitProcent) chance = limitProcent;
         res = Random.Range(0, 100);
         enemy.isFear = res < chance;
         if (res < chance)
         {
            AlreadyTurn.Add(enemy);
         }
      }

      //Всё что нужно делать в начале раунда из баффов
      foreach (var chara in AllCharacter)
      {
         foreach(var buff in chara.buffs)
         {
            switch (buff)
            {
               case Fighter.Buff.Accompaniment:
                  List<Fighter> list = new(EnemyTeam);
                  if (!EnemyTeam.Contains(chara))
                  {
                     list = new(PlayerTeam);
                  }
                  bool contains = false;
                  foreach(var figh in list)
                  {
                     if(figh.Data.name == "Ancient Evil")   
                     {
                        contains = true;
                        break;
                     }
                  }
                  if (!contains)
                     chara.buffs.Remove(Fighter.Buff.Accompaniment);
                  break;
               case Fighter.Buff.WeightMemories:
                  chance = 30;
                  res = Random.Range(0, 100);
                  chara.isFear = res < chance;
                  if (res < chance)
                  {
                     AlreadyTurn.Add(chara);
                  }
                  break;
            }
         }
      }
      CheckConditionWinLose();

      PlayerUITeam = new(PlayerTeam);
      EnemyUITeam = new(EnemyTeam);
      UpdatePortrait();
      FightUIController.hardUpdate = true;

      // Кто стартует раунд?
      bool isPlayerFirst = true;
      int playerAgility = PlayerTeam[0].agility + PlayerTeam[0].bonus_agility;
      int maxEnemyAgility = int.MinValue;

      foreach (var chara in EnemyTeam)
      {
         int enemyAgility = chara.agility + chara.bonus_agility;
         if (enemyAgility > maxEnemyAgility)
            maxEnemyAgility = enemyAgility;
      }

      if (maxEnemyAgility > playerAgility)
      {
         isPlayerFirst = false;
      }
      else if (maxEnemyAgility == playerAgility)
      {
         isPlayerFirst = Random.value < 0.5f;
      }

      if (isPlayerFirst)
         StartCoroutine(PlayerTurn());
      else
         StartCoroutine(EnemyTurn());

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
      skipBtn.SetActive(false);
      selectedSkill = null;
      selectedTarget = null;
      CardShowerReset();

      if (skipTurn)
         goto SkipTurn;

      //Ждём пока не выберется скилл
      ChangeSkill:
      EnemyUITeam = new List<Fighter>(EnemyTeam);
      UpdatePortrait();
      if(PlayerTeam.Count == 1)
         skipBtn.SetActive(true);

      while (selectedSkill == null)
      {
         yield return null;
         if (skipTurn)
         {
            goto StartTurn;
         }
      }
      
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
            var list = new List<Fighter>(AllCharacter);
            list.Remove(SelectedCharacter());
            EnemyUITeam = list;
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

         if (skipTurn)
         {
            FightUIController.allInteractable = false;
            _selected = false;
            goto StartTurn;
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
      FightUIController.hardUpdate = true;

      SelectedCharacter().CastSkill(selectedTargets, selectedSkill);

      SkipTurn:

      AlreadyTurn.Add(SelectedCharacter());

      skipBtn.SetActive(false);

      if (!skipTurn)
         yield return new WaitForSeconds(2f);
      //Если применение скилла повлекло за собой применение других скиллов
      while (additionalCastSkills.Count > 0)
      {
         //Устанавливаем весь дизайн для каста и кастуем здесь
         selectedTargets = additionalCastSkills[0].targets;
         selectedSkill = additionalCastSkills[0].skill;

         Skill_Image.externalSkill = selectedSkill;
         Skill_Image.externalCaster = SelectedCharacter();

         PlayerUITeam = new List<Fighter> { PlayerTeam[SelectedCharacterID] };
         EnemyUITeam = selectedTargets;
         cast = true;
         UpdatePortrait();
         FightUIController.hardUpdate = true;

         SelectedCharacter().CastSkill(selectedTargets, selectedSkill, additionalCastSkills[0].needCooldown);
         additionalCastSkills.RemoveAt(0);
         yield return new WaitForSeconds(2f);
      }

      PlayerUITeam = new List<Fighter>(PlayerTeam);
      EnemyUITeam = new List<Fighter>(EnemyTeam);

      selectedSkill = null;
      selectedTarget = null;
      SelectedCharacterID_Reset();
      _selected = false;
      skipTurn = false;

      cast = false;
      UpdatePortrait();

      Skip:
      CardShowerReset();
      CheckRoundChange(SelectedCharacter());
      if (!isWin && !isLose) 
      {
         if (isAllDoTurn) StartRound();
         else StartCoroutine(EnemyTurn());
      }
   }

   IEnumerator EnemyTurn()
   {
      Fighter caster = null;
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

      caster = notTurnEnemies[Random.Range(0, notTurnEnemies.Count)];

      if (caster.Intension == null)
         goto SkipTurn;

      //EnemyUITeam = new List<Fighter>() { selectedEnemy };
      //UpdatePortrait();
      //Реализовать логику выбора цели
      Fighter target;
      //int i = 0;
      /*do
      {
         target = PlayerTeam[Random.Range(0, PlayerTeam.Count)];
         i++;
      } while (target.isDead && i < 1000 && target.buffs.Contains(Fighter.Buff.Provocation)); //выбираем не мёртвую цель без провокации
      */
      var possibleTargets = PlayerTeam
    .Where(f => !f.isDead && !f.buffs.Contains(Fighter.Buff.Provocation))
    .ToList();
      if (possibleTargets.Count > 0)
         target = possibleTargets[Random.Range(0, possibleTargets.Count)];
      else
         target = PlayerTeam.First(f => !f.isDead); // fallback

      var selectedTargets = ChooseTarget(caster.Intension, caster, target);
      
      Debug.Log("Использует умение: " + caster.Intension.skillData._name);
      PlayerUITeam = selectedTargets;
      EnemyUITeam = new List<Fighter> { caster };
      cast = true;
      Skill_Image.externalSkill = caster.Intension;
      Skill_Image.externalCaster = caster;
      UpdatePortrait();

      yield return new WaitForSeconds(2f);

      caster.CastSkill(selectedTargets, caster.Intension);

      SkipTurn:
      AlreadyTurn.Add(caster);

      //Если применение скилла повлекло за собой применение других скиллов
      while (additionalCastSkills.Count > 0)
      {
         //Устанавливаем весь дизайн для каста и кастуем здесь
         selectedTargets = additionalCastSkills[0].targets;
         var selectedSkill = additionalCastSkills[0].skill;

         Skill_Image.externalSkill = selectedSkill;
         Skill_Image.externalCaster = caster;

         PlayerUITeam = selectedTargets;
         EnemyUITeam = new List<Fighter> { caster };
         cast = true;
         UpdatePortrait();
         FightUIController.hardUpdate = true;

         yield return new WaitForSeconds(2f);
         caster.CastSkill(selectedTargets, selectedSkill, additionalCastSkills[0].needCooldown);
         additionalCastSkills.RemoveAt(0);
      }
      yield return null;
      cast = false;
      Skill_Image.isDisableExSkill = true;
      yield return null;
      PlayerUITeam = new List<Fighter>(PlayerTeam);
      EnemyUITeam = new List<Fighter>(EnemyTeam);
      FightUIController.allDisable = false;

      UpdatePortrait();
      isEnemyTurn = false;
      Skip:
      CheckRoundChange(caster);
      if (!isWin && !isLose)
      {
         if (isAllDoTurn) StartRound();
         else StartCoroutine(PlayerTurn());
      }
   }

   bool isDoubleTurn = false;

   private void CheckRoundChange(Fighter whoMakeTurn)
   {
      for (int i = 0; i < EnemyTeam.Count; i++)
      {
         Fighter chara = EnemyTeam[i];
         if (chara.isDead && chara.buffs.Contains(Fighter.Buff.Corpseless))
            EnemyTeam.Remove(chara);
      }

      if (isDoubleTurn && whoMakeTurn == PlayerTeam[0])
      {
         PlayerTeam[0].isDoubleTurn = false;
      }

      if (!isDoubleTurn && whoMakeTurn == PlayerTeam[0])
      {
         //Проверка на двойной ход:
         var mainChar = PlayerTeam[0];
         int chance = (mainChar.agility + mainChar.bonus_agility) * procentPerOneCharacteristic;
         if (chance > limitProcent) chance = limitProcent;
         int res = Random.Range(0, 100);
         PlayerTeam[0].isDoubleTurn = res < chance;
         if (res < chance)
         {
            AlreadyTurn.Remove(mainChar);
            //Кулдауны
            var keys = new List<Skill>(mainChar.cooldowns.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
               Skill skill = keys[i];
               mainChar.cooldowns[skill]--;

               if (mainChar.cooldowns[skill] == 0)
               {
                  mainChar.cooldowns.Remove(skill);
               }
            }
         }
         isDoubleTurn = true;
      }


      PlayerUITeam = new(PlayerTeam);
      EnemyUITeam = new(EnemyTeam);
      UpdatePortrait();
      FightUIController.hardUpdate = true;

      CheckConditionWinLose();

      isAllDoTurn = allEnemiesDoTurn && allPlayerCharactersDoTurn;


      if (isAllDoTurn)
      {
         //Всё что нужно делать в конце раунда из баффов, начало раунда (если будет нужно) в методе StartRound();
         foreach (var chara in AllCharacter)
         {
            foreach(var buff in chara.buffs)
            {
               switch (buff)
               {
                  case Fighter.Buff.CurseVictim:
                     chara.SacrificeHP(1);
                     break;
                  case Fighter.Buff.CurseDestruction:
                     chara.TakeDmg(null, 1, SkillSO.SkillElement.Dark);
                     break;
               }
            }

            if (chara.buffs.Contains(Fighter.Buff.Poison))
            {
               chara.TakeDmg(null, chara.poisonStacks, SkillSO.SkillElement.Poison);
               chara.poisonStacks /= 2;
               if(chara.poisonStacks <= 0)
               {
                  chara.poisonStacks = 0;
                  chara.buffs.Remove(Fighter.Buff.Poison);
               }
            }
         }

         
         ////Проверка на просмотр намерений:
         //var mainChar = PlayerTeam[0];
         //int chance = (mainChar.wisdow + mainChar.bonus_wisdow) * procentPerOneCharacteristic;
         //if (chance > limitProcent) chance = limitProcent;
         //int res = Random.Range(0, 100);
         //seeIntension = res < chance;

         AlreadyTurn.Clear();
         allEnemiesDoTurn = false;
         allPlayerCharactersDoTurn = false;
         isDoubleTurn = false;
         PlayerTeam[0].isDoubleTurn = false;
         round_number++;
         Debug.Log("Следующий раунд: " + round_number.ToString());

         //Кулдауны
         foreach (var hero in AllCharacter)
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

         //Намерения
         foreach (var enemy in EnemyTeam)
            enemy.MakeIntention();

         CheckConditionWinLose();
      }
   }

   private void CheckConditionWinLose()
   {
      //Проверка надо ли удалять трупы
      for (int i = 0; i < PlayerTeam.Count; i++)
      {
         PlayableCharacter chara = PlayerTeam[i];
         if (chara.isDead && chara.buffs.Contains(Fighter.Buff.Corpseless))
            PlayerTeam.Remove(chara);
      }

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

      if (isLose)
      {
         WinLoseText.text = "К сожалению, вы проиграли";
         SaveLoadController.EndFight();
         var data = eventRoom.eventData;
         if (data.isNotOver)
         {
            WinLoseText.text += "\nНо это ещё не конец.";
            eventRoom.eventData = eventRoom.eventData.choices[1];
            var data2 = eventRoom.eventData;
            eventRoom.eventName = $"{data2.eventID}-{data2.eventID_Part}";

            foreach(var chara in PlayerTeam)
            {
               chara.hp = (int)(chara.max_hp * (data.overHpProcent / 100.0f));
            }
            SaveLoadController.Save();
         }
         else
            SaveLoadController.ClearSave(SaveLoadController.slot);
      }
      else if (isWin)
      {
         WinLoseText.text = "Поздравляю с победой";

         eventRoom.eventData = eventRoom.eventData.choices[0];
         var data = eventRoom.eventData;
         eventRoom.eventName = $"{data.eventID}-{data.eventID_Part}";
         SaveLoadController.Save();
      }
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
      if (enemies.Count == 0) return EnemyTeam[0];
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
            selectedTargets.Add(caster);
            break;
         case SkillSO.SkillTarget.Mass_Enemies:
            selectedTargets = new List<Fighter>(enemies);
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
            selectedTargets.Remove(caster);
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
            selectedTargets = new List<Fighter>() { enemies[Random.Range(0, enemies.Count)] };
            if (!selectedSkill.skillData.isCorpseTargetToo)
            {
               do
               {
                  selectedTargets = new List<Fighter>() { enemies[Random.Range(0, enemies.Count)] };
               } while (selectedTargets[0].isDead);
            }
            break;
         case SkillSO.SkillTarget.Mass_Allies:
            selectedTargets = new List<Fighter>(allies);
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
            selectedTargets = new List<Fighter>() { allies[Random.Range(0, allies.Count)] };
            if (!selectedSkill.skillData.isCorpseTargetToo)
            {
               do
               {
                  selectedTargets = new List<Fighter>() { allies[Random.Range(0, allies.Count)] };
               } while (selectedTargets[0].isDead);
            }
            break;
         case SkillSO.SkillTarget.Random_Target:
            List<Fighter> list = new(AllCharacter);
            list.Remove(caster);
            selectedTargets = new List<Fighter>() { list[Random.Range(0, list.Count)] };
            if (!selectedSkill.skillData.isCorpseTargetToo)
            {
               do
               {
                  selectedTargets = new List<Fighter>() { list[Random.Range(0, list.Count)] };
               } while (selectedTargets[0].isDead);
            }
            break;
      }
      return selectedTargets;
   }

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
            foreach (Skill skill in PlayerUITeam[0].skillsBattle)
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

   public void SkipBtn()
   {
      skipTurn = true;
   }
}
