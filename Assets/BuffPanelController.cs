using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffPanelController : MonoBehaviour
{
   public GameObject handCardPrefab;
   public Transform buffTransform;
   private CanvasGroup cg;

   public static bool isOpened = false;
   public static Fighter buffFighter = null;

   private void Start()
   {
      cg = GetComponent<CanvasGroup>();
   }

   private void Update()
   {
      if(Input.GetKeyDown(KeyCode.Mouse1) && isOpened)
      {
         Close();
      }

      if(buffFighter != null)
      {
         var character = buffFighter;
         buffFighter = null;
         Open(character);
      }
   }
   public void Open(Fighter character)
   {
      UpdateCards(character);
      cg.alpha = 1f;
      cg.blocksRaycasts = true;
      cg.interactable = true;
      isOpened = true;
   }

   public void Close()
   {
      cg.alpha = 0f;
      cg.blocksRaycasts = false;
      cg.interactable = false;
      isOpened = false;
   }
   public void UpdateCards(Fighter character)
   {
      foreach (Transform child in buffTransform)
      {
         Destroy(child.gameObject);
      }
      List<Skill> skills = new();
      foreach(var buff in character.buffs)
      {
         var skill = character.BuffToSkill(buff);
         if (skill != null)
            skills.Add(skill);
      }

      foreach (var skill in skills.OrderBy(s => s.skillData.name))
      {
         GameObject go = Instantiate(handCardPrefab, buffTransform);
         go.GetComponent<CardFiller>().skill = skill;
      }
   }
}
