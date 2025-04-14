using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class FightPortrait : MonoBehaviour
{
   public int id;
   public bool isInteractable;

   private Slider hp_slider;
   private TMP_Text hp_text;
   private Image hearth_icon, slider_filler;

   public GameObject Dark, Skeleton;

   private void Start()
   {
      hp_slider = transform.GetChild(1).GetChild(0).GetComponent<Slider>();
      hp_text = transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<TMP_Text>();
      hearth_icon = transform.GetChild(1).GetChild(1).GetComponent<Image>();
      slider_filler = transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>();
   }
   public void ChooseCharacterRun()
   {
      if (isInteractable)
      {
         if (FightUIController.allInteractable) Fight.SelectTarget(id);
         else Fight.SelectCharacter(id);
      }
   }

   private void Update()
   {
      //Для обновления хп
      //Добавить изменение цвета в зависимости от % хп
      Fighter character = null;
      if (id < 6)
      {
         character = Fight.PlayerTeam[id];
         if (character.isDead) isInteractable = false;

         if (Dark.activeInHierarchy == isInteractable)
            Dark.SetActive(!isInteractable);
      }
      else
      {
         character = Fight.EnemyTeam[id - 6];

         bool darkCondition = Fight.AlreadyTurn.Contains(character);
         if (character.isDead)
         {
            isInteractable = false;
            darkCondition = true;
         }
         if (Dark.activeInHierarchy != darkCondition)
            Dark.SetActive(darkCondition);
      }

      if (Fight.cast) Dark.SetActive(false);

      hp_slider.maxValue = character.max_hp + character.bonus_hp;
      hp_slider.value = character.hp;

      float ratio = hp_slider.value / hp_slider.maxValue;
      //armor 557B7E(4F686A) green 40CF30(1B6714) yellow CBCD35(AEB00B) red E02B29(9A171D)
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
      if (character.armor_current > 0)
      {
         hearth_icon.color = new Color32(85, 123, 126, 255);
         slider_filler.color = new Color32(78, 104, 106, 255);
         //hp_slider.value = character.armor;
      }
      if (character.hp <= 0)
      {
         hearth_icon.color = new Color32(46, 46, 46, 255);
         Skeleton.SetActive(true);
      }

      hp_text.text = character.hp.ToString() +
         "/" + (character.max_hp + character.bonus_hp).ToString() +
         ((character.armor_current > 0) ? ("\n" + character.armor_current + " A") : "");
   }
}
