using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
   private GameObject currentCard = null;
   public List<CardShower> allCards;

   void Update()
   {
      PointerEventData eventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
      List<RaycastResult> results = new List<RaycastResult>();
      EventSystem.current.RaycastAll(eventData, results);

      GameObject topCard = null;

      foreach (RaycastResult hit in results)
      {
         if (hit.gameObject.CompareTag("Card"))
         {
            // �������� ����� ������ ����� (��� ����� ������� � ��������)
            if (topCard == null || hit.gameObject.transform.position.x > topCard.transform.position.x)
            {
               topCard = hit.gameObject;
            }
         }
      }

      if (topCard != null)
      {
         //��� �� �����
         if (Input.GetKeyDown(KeyCode.Mouse0))
         {
            if (Fight.cast) return;
            var cardSh = topCard.GetComponent<CardShower>();
            /*if (cardSh.isLock)
            {
               var skimage = topCard.GetComponent<Skill_Image>();
               cardSh.isLock = false;
               Skill_Image.isOneLocked = false;
               skimage.Action();
               return;
            }*/
            cardSh.isLock = true; 
            var skimage = topCard.GetComponent<Skill_Image>();
            skimage.Action();
            Skill_Image.isOneLocked = true;
            bool allUnlock = true;
            foreach (var card in allCards)
            {
               if(card != cardSh)
               {
                  if (card.isLock) allUnlock = false;
                  card.isLock = false;
               }
            }
            if(!allUnlock)
            {
               //var skimage = topCard.GetComponent<Skill_Image>();
               skimage.Exit();
               skimage.Enter(false, Fight.SelectedCharacter());
               //StartCoroutine(skimage.Open(false));
            }
         }
         //������ �� �������
         /*if (Input.GetKeyDown(KeyCode.Mouse2))
         {
            var cardSh = topCard.GetComponent<CardShower>();
            cardSh.isLock = false;
            Skill_Image.isOneLocked = false;
            Fight.selectedSkill = null;
         }*/

         if (topCard != currentCard)
         {
            if (currentCard != null)
            {
               var cardSh1 = currentCard.GetComponent<CardShower>();
               cardSh1.isActive = false;
            }

            currentCard = topCard;
            var cardSh2 = currentCard.GetComponent<CardShower>();
            cardSh2.isActive = true;
         }
      }
      else if (currentCard != null)
      {
         var cardSh = currentCard.GetComponent<CardShower>();
         cardSh.isActive = false;
         currentCard = null;
      }
   }

   
}
