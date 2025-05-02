using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static FightPortrait;

public class MagicScrollFiller : MonoBehaviour
{
   public PlayableCharacter hero;
   public Image heroPortrate;
   public Slider sliderHP;
   public Image hearth_icon, slider_filler;
   public TextMeshProUGUI hp_text, characteristicsText;
   public Transform activeHand;
   public GameObject cardPrefab;

   private static readonly List<GameObject> createdCards = new();
   public void Initialize()
   {
      CreatedCardsClear(activeHand);

      hero = SaveLoadController.runInfo.PlayerTeam[0];

      heroPortrate.sprite = hero.Portrait;

      //Хп
      sliderHP.maxValue = hero.max_hp;
      sliderHP.value = hero.hp;

      float ratio = sliderHP.value / sliderHP.maxValue;
      //defence 557B7E(4F686A) green 40CF30(1B6714) yellow CBCD35(AEB00B) red E02B29(9A171D)
      if (ratio > 0.6)
      {
         hearth_icon.color = new Color32(64, 207, 48, 255);
         slider_filler.color = new Color32(27, 103, 20, 255);
      }
      else if (ratio > 0.3)
      {
         hearth_icon.color = new Color32(202, 204, 53, 255);
         slider_filler.color = new Color32(173, 176, 11, 255);
      }
      else
      {
         hearth_icon.color = new Color32(224, 43, 41, 255);
         slider_filler.color = new Color32(154, 22, 29, 255);
      }
      hp_text.text = hero.hp.ToString() +
      "/" + (hero.max_hp + hero.bonus_hp).ToString();

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
      grid.constraintCount = 3;

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
