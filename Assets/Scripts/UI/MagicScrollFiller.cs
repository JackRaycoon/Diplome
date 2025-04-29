using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class MagicScrollFiller : MonoBehaviour
{
   public PlayableCharacter hero;
   public Image heroPortrate;
   public TextMeshProUGUI characteristicsText;
   public Transform activeHand;
   public GameObject cardPrefab;

   private static readonly List<GameObject> createdCards = new();
   public void Initialize()
   {
      CreatedCardsClear(activeHand);

      hero = SaveLoadController.runInfo.PlayerTeam[0];

      heroPortrate.sprite = hero.Portrait;

      int[] values = { hero.strengh, hero.agility, hero.wisdow, hero.constitution, hero.defence, SaveLoadController.runInfo.souls };
      string text = characteristicsText.text;
      characteristicsText.text = string.Format(text, values.Cast<object>().ToArray());

      //Skills
      ToActiveSkills();
   }

   private void ToActiveSkills()
   {
      if (activeHand.childCount > 0)
      {
         CreatedCardsClear();
      }
      var grid = activeHand.GetComponent<GridLayoutGroup>();
      grid.constraintCount = 2;

      bool isFirstPassive = true;

      List<Skill> skillList = new(hero.skills);
      int count = SaveLoadController.runInfo.PlayerTeam[0].ActiveSkillEmptySlots();

      for(int i = 0; i < count; i++)
      {
         skillList.Add(SkillDB.Instance.GetSkillByName("Empty"));
      }
      foreach (var skill in skillList)
      {
         bool isPassive = skill.skillData.skill_target == SkillSO.SkillTarget.Passive && skill.skillData.name != "Empty";
         if (isPassive)
         {
            if (isFirstPassive && !skill.skillData.isCurse)
            {
               isFirstPassive = false;
            }
            else
               continue;
         }
         GameObject go = Instantiate(cardPrefab, activeHand);
         var filler = go.GetComponent<CardFiller>();
         filler.skill = skill;
         var btn = go.GetComponent<Button>();
         btn.interactable = isPassive;
         if (isPassive)
         {
            go.transform.SetSiblingIndex(0);
            filler.DescriptionPanelOpenCloser();
            filler.DescriptionUpdate("<size=17>Перейти к пассивным навыкам</size>");
            btn.onClick.AddListener(ToPassiveSkills);
         }
         else
            btn.onClick.AddListener(filler.DescriptionPanelOpenCloser);
         createdCards.Add(go);
      }
      if(isFirstPassive)
      {
         var skill = SkillDB.Instance.GetSkillByName("Empty");
         GameObject go = Instantiate(cardPrefab, activeHand);
         var filler = go.GetComponent<CardFiller>();
         filler.skill = skill;
         var btn = go.GetComponent<Button>();
         btn.interactable = true;
         go.transform.SetSiblingIndex(0);
         filler.DescriptionPanelOpenCloser();
         filler.DescriptionUpdate("<size=17>Перейти к пассивным навыкам</size>");
         btn.onClick.AddListener(ToPassiveSkills);
         createdCards.Add(go);
      }
   }
   private void ToPassiveSkills()
   {
      if (activeHand.childCount > 0)
      {
         CreatedCardsClear();
      }
      var grid = activeHand.GetComponent<GridLayoutGroup>();
      grid.constraintCount = 3;

      bool isFirstCurse = true;

      List<Skill> skillList = new(hero.skills);
      int count = SaveLoadController.runInfo.PlayerTeam[0].PassiveSkillEmptySlots();

      for (int i = 0; i < count; i++)
      {
         skillList.Add(SkillDB.Instance.GetSkillByName("Empty"));
      }

      foreach (var skill in skillList)
      {
         bool isCurse = skill.skillData.isCurse && skill.skillData.name != "Empty";
         if (isCurse)
         {
            if (isFirstCurse)
            {
               isFirstCurse = false;
            }
            else
               continue;
         }
         else if (skill.skillData.skill_target != SkillSO.SkillTarget.Passive)
         {
            continue;
         }
         GameObject go = Instantiate(cardPrefab, activeHand);
         var filler = go.GetComponent<CardFiller>();
         filler.skill = skill;
         var btn = go.GetComponent<Button>();
         btn.interactable = isCurse;
         if (isCurse)
         {
            go.transform.SetSiblingIndex(0);
            filler.DescriptionPanelOpenCloser();
            filler.DescriptionUpdate("<size=17>Перейти к проклятьям</size>");
            btn.onClick.AddListener(ToCurseSkills);
         }
         else
            btn.onClick.AddListener(filler.DescriptionPanelOpenCloser);
         createdCards.Add(go);
      }

      if (isFirstCurse)
      {
         var skill = SkillDB.Instance.GetSkillByName("Empty");
         GameObject go = Instantiate(cardPrefab, activeHand);
         var filler = go.GetComponent<CardFiller>();
         filler.skill = skill;
         var btn = go.GetComponent<Button>();
         btn.interactable = true;
         go.transform.SetSiblingIndex(0);
         filler.DescriptionPanelOpenCloser();
         filler.DescriptionUpdate("<size=17>Перейти к проклятьям</size>");
         btn.onClick.AddListener(ToCurseSkills);
         createdCards.Add(go);
      }
   }

   private void ToCurseSkills()
   {
      if (activeHand.childCount > 0)
      {
         CreatedCardsClear();
      }
      var grid = activeHand.GetComponent<GridLayoutGroup>();
      grid.constraintCount = 3;

      bool isFirstActive = true;

      List<Skill> skillList = new(hero.skills);
      int count = SaveLoadController.runInfo.PlayerTeam[0].CurseSkillEmptySlots();

      for (int i = 0; i < count; i++)
      {
         skillList.Add(SkillDB.Instance.GetSkillByName("Empty"));
      }

      foreach (var skill in skillList)
      {
         bool isActive = skill.skillData.skill_target != SkillSO.SkillTarget.Passive && skill.skillData.name != "Empty";
         if (isActive)
         {
            if (isFirstActive)
            {
               isFirstActive = false;
            }
            else
               continue;
         }
         else if (!skill.skillData.isCurse && skill.skillData.name != "Empty")
         {
            continue;
         }
         GameObject go = Instantiate(cardPrefab, activeHand);
         var filler = go.GetComponent<CardFiller>();
         filler.skill = skill;
         var btn = go.GetComponent<Button>();
         btn.interactable = isActive;
         if (isActive)
         {
            go.transform.SetSiblingIndex(0);
            filler.DescriptionPanelOpenCloser();
            filler.DescriptionUpdate("<size=17>Перейти к активным навыкам</size>");
            btn.onClick.AddListener(ToActiveSkills);
         }
         else
            btn.onClick.AddListener(filler.DescriptionPanelOpenCloser);
         createdCards.Add(go);
      }

      if (isFirstActive)
      {
         var skill = SkillDB.Instance.GetSkillByName("Empty");
         GameObject go = Instantiate(cardPrefab, activeHand);
         var filler = go.GetComponent<CardFiller>();
         filler.skill = skill;
         var btn = go.GetComponent<Button>();
         btn.interactable = true;
         go.transform.SetSiblingIndex(0);
         filler.DescriptionPanelOpenCloser();
         filler.DescriptionUpdate("<size=17>Перейти к активным навыкам</size>");
         btn.onClick.AddListener(ToActiveSkills);
         createdCards.Add(go);
      }
   }

   public static void CreatedCardsClear(Transform transform = null)
   {
      if(transform != null)
      {
         foreach (Transform child in transform)
         {
            Destroy(child.gameObject);
         }
         createdCards.Clear();
         return;
      }
      foreach (var go in createdCards)
      {
         Destroy(go);
      }
      createdCards.Clear();
   }
}
