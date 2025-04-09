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
            // Выбираем самую правую карту (или самую верхнюю в иерархии)
            if (topCard == null || hit.gameObject.transform.position.x > topCard.transform.position.x)
            {
               topCard = hit.gameObject;
            }
         }
      }

      if (topCard != null)
      {
         if (Input.GetKeyDown(KeyCode.Mouse0))
         {
            var cardSh = topCard.GetComponent<CardShower>();
            cardSh.isLock = !cardSh.isLock;
            foreach(var card in allCards)
            {
               if(card != cardSh)
               {
                  card.isLock = false;
               }
            }
         }
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
