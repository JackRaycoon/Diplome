using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fight : MonoBehaviour
{
   public GridLayoutGroup heroes, enemies;

   public static List<Fighter> AllCharacter;

   public static List<PlayableCharacter> PlayerTeam;
   public static List<Fighter> PlayerUITeam;

   public static List<Fighter> EnemyTeam;
   public static List<Fighter> EnemyUITeam;

   public static List<Fighter> AlreadyTurn;

   public int round_number = 1;

   public List<SpriteRenderer> HeroesSprites;
   public List<SpriteRenderer> EnemiesSprites;

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
   private void Start()
   {
      Cursor.lockState = CursorLockMode.None;

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
      PlayerUITeam = new List<Fighter>(PlayerTeam);
      EnemyUITeam = new List<Fighter>(EnemyTeam);

      SelectedCharacterID_Reset();

      AlreadyTurn = new List<Fighter>();
      AllCharacter = new List<Fighter>();
      foreach (Fighter character in PlayerTeam) AllCharacter.Add(character);
      foreach (Fighter character in EnemyTeam) AllCharacter.Add(character);

      UpdatePortrait();

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
      }
      else if (Input.GetKeyDown(KeyCode.Mouse1))
      {
         selectedSkill = null;
         Debug.Log("NUll");
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
      foreach (Fighter character in AllCharacter) character.Spawn();
      MakeIntention();
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

      //Сделать чтобы менялось в зависимости от целей каста, менять EnemyUITeam
      PlayerUITeam = new List<Fighter>() { SelectedCharacter() };
      switch (selectedSkill.skillData.skill_type)
      {
         case SkillSO.SkillType.Solo_Enemy:
         case SkillSO.SkillType.Mass_Enemies:
         case SkillSO.SkillType.Random_Enemy:
            EnemyUITeam = new List<Fighter>(EnemyTeam);
            break;
         case SkillSO.SkillType.All:
         case SkillSO.SkillType.Random_Target:
            EnemyUITeam = new List<Fighter>(AllCharacter);
            break;
         case SkillSO.SkillType.Solo_Ally:
         case SkillSO.SkillType.Mass_Allies:
         case SkillSO.SkillType.Random_Ally:
            EnemyUITeam = new List<Fighter>(PlayerTeam);
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
      var selectedTargets = new List<Fighter>();
      switch (selectedSkill.skillData.skill_type)
      {
         case SkillSO.SkillType.Solo_Enemy:
         case SkillSO.SkillType.Solo_Ally:
            selectedTargets.Add(selectedTarget);
            break;
         case SkillSO.SkillType.Mass_Enemies:
            selectedTargets = new List<Fighter>(EnemyTeam);
            if (!selectedSkill.skillData.isCorpseTargetToo)
            {
               for(int i = 0; i<selectedTargets.Count; i++)
               {
                  if (selectedTargets[i].isDead)
                  {
                     selectedTargets.Remove(selectedTargets[i]);
                     i--;
                  }
               }
            }
            break;
         case SkillSO.SkillType.All:
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
         case SkillSO.SkillType.Random_Enemy:
            selectedTargets = new List<Fighter>() { EnemyTeam[Random.Range(0, EnemyTeam.Count)] };
            if (!selectedSkill.skillData.isCorpseTargetToo)
            {
               do
               {
                  selectedTargets = new List<Fighter>() { EnemyTeam[Random.Range(0, EnemyTeam.Count)] };
               } while (selectedTargets[0].isDead);
            }
            break;
         case SkillSO.SkillType.Mass_Allies:
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
         case SkillSO.SkillType.Random_Ally:
            selectedTargets = new List<Fighter>() { PlayerTeam[Random.Range(0, PlayerTeam.Count)] };
            if (!selectedSkill.skillData.isCorpseTargetToo)
            {
               do
               {
                  selectedTargets = new List<Fighter>() { PlayerTeam[Random.Range(0, PlayerTeam.Count)] };
               } while (selectedTargets[0].isDead);
            }
            break;
         case SkillSO.SkillType.Random_Target:
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

      FightUIController.allInteractable = false;
      _selected = false;

      PlayerUITeam = new List<Fighter> { PlayerTeam[SelectedCharacterID] };
      EnemyUITeam = selectedTargets;
      cast = true;
      UpdatePortrait();

      selectedSkill.Cast(PlayerTeam[SelectedCharacterID], selectedTargets);

      AlreadyTurn.Add(PlayerTeam[SelectedCharacterID]);

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

      yield return new WaitForSeconds(2f);

      var selectedEnemy = notTurnEnemies[Random.Range(0, notTurnEnemies.Count)];
      //Реализовать логику выбора цели
      Fighter target;
      do
      {
         target = PlayerTeam[Random.Range(0, PlayerTeam.Count)];
      } while (target.isDead); //выбираем не мёртвую цель

      var selectedTargets = new List<Fighter>();
      switch (selectedEnemy.Intension.skillData.skill_type)
      {
         case SkillSO.SkillType.Solo_Enemy:
         case SkillSO.SkillType.Solo_Ally:
            selectedTargets.Add(target);
            break;
         case SkillSO.SkillType.Mass_Enemies:
            selectedTargets = new List<Fighter>(PlayerTeam);
            if (!selectedEnemy.Intension.skillData.isCorpseTargetToo)
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
         case SkillSO.SkillType.Mass_Allies:
            selectedTargets = new List<Fighter>(EnemyTeam);
            if (!selectedEnemy.Intension.skillData.isCorpseTargetToo)
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
         case SkillSO.SkillType.All:
            selectedTargets = new List<Fighter>(AllCharacter);
            if (!selectedEnemy.Intension.skillData.isCorpseTargetToo)
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
         case SkillSO.SkillType.Random_Enemy:
            selectedTargets = new List<Fighter>() { PlayerTeam[Random.Range(0, PlayerTeam.Count)] };
            if (!selectedEnemy.Intension.skillData.isCorpseTargetToo)
            {
               do
               {
                  selectedTargets = new List<Fighter>() { PlayerTeam[Random.Range(0, PlayerTeam.Count)] };
               } while (selectedTargets[0].isDead);
            }
            break;
         case SkillSO.SkillType.Random_Ally:
            selectedTargets = new List<Fighter>() { EnemyTeam[Random.Range(0, EnemyTeam.Count)] };
            if (!selectedEnemy.Intension.skillData.isCorpseTargetToo)
            {
               do
               {
                  selectedTargets = new List<Fighter>() { EnemyTeam[Random.Range(0, EnemyTeam.Count)] };
               } while (selectedTargets[0].isDead);
            }
            break;
         case SkillSO.SkillType.Random_Target:
            selectedTargets = new List<Fighter>() { AllCharacter[Random.Range(0, AllCharacter.Count)] };
            if (!selectedEnemy.Intension.skillData.isCorpseTargetToo)
            {
               do
               {
                  selectedTargets = new List<Fighter>() { AllCharacter[Random.Range(0, AllCharacter.Count)] };
               } while (selectedTargets[0].isDead);
            }
            break;
      }

      Debug.Log("Использует умение: " + selectedEnemy.Intension.skillData._name);
      PlayerUITeam = selectedTargets;
      EnemyUITeam = new List<Fighter> { selectedEnemy };
      cast = true;
      Skill_Image.externalSkill = selectedEnemy.Intension;
      UpdatePortrait();

      yield return new WaitForSeconds(2f);

      selectedEnemy.Intension.Cast(selectedEnemy, selectedTargets);
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
         MakeIntention();
      }
   }

   private void EndFight() 
   {
      StopAllCoroutines();
      endFight = true;
      WinLosePanel.SetActive(true);
      if (isLose) WinLoseText.text = "К сожалению, вы проиграли";
      else if (isWin) WinLoseText.text = "Поздравляю с победой";
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

   private void MakeIntention()
   {
      foreach (Fighter figh in EnemyTeam) 
      {
         figh.prevIntension = figh.Intension;

         //Количество скиллов без пассивок (используемых скиллов)
         int skillCount = 0;
         foreach(Skill skill in figh.skills)
         {
            if (skill.skillData.skill_type != SkillSO.SkillType.Passive) skillCount++;
         }
         bool reroll;
         do
         {
            var skills = figh.skills;
            figh.Intension = skills[Random.Range(0, skills.Count)];
            if(figh.prevIntension != null)
               reroll = figh.Intension.skillData.skill_type == SkillSO.SkillType.Passive ||
                        (skillCount > 1 && figh.Intension == figh.prevIntension);
            else reroll = figh.Intension.skillData.skill_type == SkillSO.SkillType.Passive;
         } while (reroll);
      }
   }

   private void UpdatePortrait()
   {
      FightUIController.CountHeroes = PlayerUITeam.Count;
      FightUIController.CountEnemies = EnemyUITeam.Count;

      for (int i = 0; i < PlayerUITeam.Count; i++)
      {
         HeroesSprites[i].sprite = PlayerUITeam[i].Portrait;
      }
      for (int i = 0; i < EnemyUITeam.Count; i++)
      {
         EnemiesSprites[i].sprite = EnemyUITeam[i].Portrait;
      }
      if(PlayerUITeam.Count == 1)
      {
         int i = 0;
         if(!isEnemyTurn)
         {
            foreach (Skill skill in PlayerUITeam[0].skills)
            {
               if (skill.skillData.skill_type == SkillSO.SkillType.Passive) continue;
               var SkCard = SkillsCards[i];
               SkCard.GetComponent<Graphic>().enabled = true;
               Show(SkCard);
               SkCard.GetComponent<CardFiller>().skill = skill;
               SkCard.GetComponent<Skill_Image>().skill = skill;
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
      if (isWin)
      {
         SceneManager.LoadScene(1);
      }
      else
      {
         SaveLoadController.ClearSave(SaveLoadController.slot);
         SceneManager.LoadScene(0);
      }
   }
}
