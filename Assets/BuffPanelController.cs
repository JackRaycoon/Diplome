using System.Collections;
using System.Collections.Generic;
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
      Debug.Log("Update");
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
      
      foreach(var buff in character.buffs)
      {
         var skill = character.BuffToSkill(buff);
         if (skill == null) continue;
         GameObject go = Instantiate(handCardPrefab, buffTransform);
         go.GetComponent<CardFiller>().skill = skill;
      }
   }
}
