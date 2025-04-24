using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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

      int[] values = { hero.strengh, hero.agility, hero.wisdow, hero.constitution, hero.defence };
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
      foreach (var skill in hero.skills)
      {
         bool isPassive = skill.skillData.skill_target == SkillSO.SkillTarget.Passive;
         if (isPassive)
         {
            if (isFirstPassive)
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
   }
   private void ToPassiveSkills()
   {
      if (activeHand.childCount > 0)
      {
         CreatedCardsClear();
      }
      var grid = activeHand.GetComponent<GridLayoutGroup>();
      grid.constraintCount = 3;

      bool isFirstActive = true;
      foreach (var skill in hero.skills)
      {
         bool isActive = skill.skillData.skill_target != SkillSO.SkillTarget.Passive;
         if (isActive)
         {
            if (isFirstActive)
            {
               isFirstActive = false;
            }
            else
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
