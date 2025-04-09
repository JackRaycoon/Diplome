using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
   public static bool isEnemyTurn = false;
   private void Start()
   {
      RunInfo.PlayerTeam.Add(new PlayableCharacter("Playable Warrior"));
      RunInfo.PlayerTeam.Add(new PlayableCharacter("Playable Archer"));
      RunInfo.PlayerTeam.Add(new PlayableCharacter("Playable Priest"));


      PlayerTeam = RunInfo.PlayerTeam;
      EnemyTeam = new List<Fighter>
      {
         new Fighter("Wolf"),
         new Fighter("Wolf")
      };
      PlayerUITeam = new List<Fighter>(PlayerTeam);
      EnemyUITeam = new List<Fighter>(EnemyTeam);

      AlreadyTurn = new List<Fighter>();
      AllCharacter = new List<Fighter>();
      foreach (Fighter character in PlayerTeam) AllCharacter.Add(character);
      foreach (Fighter character in EnemyTeam) AllCharacter.Add(character);

      UpdatePortrait();

      FightStart();
   }

   private void Update()
   {
      //������� �� ������ ������ ����
      if (Input.GetKeyDown(KeyCode.Mouse1) && selectedSkill == null)
      {
         SelectedCharacterID = -1;
         _selected = false;
         PlayerUITeam = new List<Fighter>(PlayerTeam);
         UpdatePortrait();
      }

      //����� ���������� ���������
      if (SelectedCharacterID != -1 && !_selected)
      {
         _selected = true;
         PlayerUITeam = new List<Fighter> { PlayerTeam[SelectedCharacterID] };
         UpdatePortrait();
      }

      //���������� �� ����������
      if (needUpdatePortrait)
      {
         needUpdatePortrait = false;
         UpdatePortrait();
      }
      RoundNum.text = "�����: " + round_number;
   }

   private void FightStart()
   {
      foreach (Fighter character in AllCharacter) character.Spawn();
      MakeIntention();
      StartCoroutine(PlayerTurn());
   }

   IEnumerator PlayerTurn()
   {
      if (allPlayerCharactersDoTurn) goto Skip;
      SelectedCharacterID = -1;
      StartTurn:
      selectedSkill = null;
      selectedTarget = null;

      //��� ���� �� ��������� �����
      while (selectedSkill == null) yield return null;

      PlayerUITeam = new List<Fighter>(PlayerTeam);
      UpdatePortrait();
      FightUIController.allInteractable = true;

      //��� ������ ����
      while (selectedTarget == null)
      {
         yield return null;
         if (Input.GetKeyDown(KeyCode.Mouse1))
         {
            FightUIController.allInteractable = false;
            _selected = false;
            goto StartTurn;
         }
      }
      var selectedTargets = new List<Fighter>();
      switch (selectedSkill.skillData.skill_type)
      {
         case SkillSO.SkillType.Solo_Target:
            selectedTargets.Add(selectedTarget);
            break;
         case SkillSO.SkillType.Mass_Target:
            if (PlayerTeam.Contains(selectedTarget as PlayableCharacter))
               selectedTargets = new List<Fighter>(PlayerTeam);
            else
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
         case SkillSO.SkillType.Random_Target:
            selectedTargets = new List<Fighter>() { EnemyTeam[Random.Range(0, EnemyTeam.Count)] };
            if (!selectedSkill.skillData.isCorpseTargetToo)
            {
               do
               {
                  selectedTargets = new List<Fighter>() { EnemyTeam[Random.Range(0, EnemyTeam.Count)] };
               } while (selectedTargets[0].isDead);
            }
            break;
         case SkillSO.SkillType.Allies_Random_Target:
            selectedTargets = new List<Fighter>() { PlayerTeam[Random.Range(0, PlayerTeam.Count)] };
            if (!selectedSkill.skillData.isCorpseTargetToo)
            {
               do
               {
                  selectedTargets = new List<Fighter>() { PlayerTeam[Random.Range(0, PlayerTeam.Count)] };
               } while (selectedTargets[0].isDead);
            }
            break;
         case SkillSO.SkillType.Hard_Random_Target:
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
      SelectedCharacterID = -1;
      _selected = false;

      cast = false;
      UpdatePortrait();

      Skip:
      CheckRoundChange();
      if (isAllDoTurn) StartCoroutine(PlayerTurn());
      else if(!isWin && !isLose) StartCoroutine(EnemyTurn());
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
      //����������� ������ ������ ����
      Fighter target;
      do
      {
         target = PlayerTeam[Random.Range(0, PlayerTeam.Count)];
      } while (target.isDead); //�������� �� ������ ����

      var selectedTargets = new List<Fighter>();
      switch (selectedEnemy.Intension.skillData.skill_type)
      {
         case SkillSO.SkillType.Solo_Target:
            selectedTargets.Add(target);
            break;
         case SkillSO.SkillType.Mass_Target:
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
         case SkillSO.SkillType.Random_Target:
            selectedTargets = new List<Fighter>() { PlayerTeam[Random.Range(0, PlayerTeam.Count)] };
            if (!selectedEnemy.Intension.skillData.isCorpseTargetToo)
            {
               do
               {
                  selectedTargets = new List<Fighter>() { PlayerTeam[Random.Range(0, PlayerTeam.Count)] };
               } while (selectedTargets[0].isDead);
            }
            break;
         case SkillSO.SkillType.Allies_Random_Target:
            selectedTargets = new List<Fighter>() { EnemyTeam[Random.Range(0, EnemyTeam.Count)] };
            if (!selectedEnemy.Intension.skillData.isCorpseTargetToo)
            {
               do
               {
                  selectedTargets = new List<Fighter>() { EnemyTeam[Random.Range(0, EnemyTeam.Count)] };
               } while (selectedTargets[0].isDead);
            }
            break;
         case SkillSO.SkillType.Hard_Random_Target:
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

      Debug.Log("���������� ������: " + selectedEnemy.Intension.skillData._name);
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
         Debug.Log("��������� �����: " + round_number.ToString());
         MakeIntention();
      }
   }

   private void EndFight() 
   {
      StopAllCoroutines();
      WinLosePanel.SetActive(true);
      if (isLose) WinLoseText.text = "� ���������, �� ���������";
      else if (isWin) WinLoseText.text = "���������� � �������";
   }

   public static void SelectCharacter(int id)
   {
      SelectedCharacterID = id;
   }
   public static PlayableCharacter SelectedCharacter()
   {
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

         //���������� ������� ��� �������� (������������ �������)
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
            foreach(Skill skill in PlayerUITeam[0].skills)
            {
               var SkCard = SkillsCards[i];
               SkCard.SetActive(true);
               SkCard.GetComponent<CardFiller>().skill = skill;
               SkCard.GetComponent<Skill_Image>().skill = skill;
               //SkIm.sprite = skill.Icon;
               //SkIm.gameObject.GetComponent<Skill_Image>().skill = skill;
               i++;
            }
      }
      else
      {
         foreach (var Sk_Card in SkillsCards)
         {
            Sk_Card.SetActive(false);
         }
      }
   }
}
