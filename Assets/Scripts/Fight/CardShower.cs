using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardShower : MonoBehaviour
{
   public Coroutine move;
   public Vector3 startPos;
   public Vector3 targetPosition;
   public bool isLock;
   public bool isActive;
   private bool isActualActive;
   public bool isFirstMove; //Сработало ли хоть раз (для определения переменных)



   public float sec = 0.1f;
   private static GameObject objectUp = null;

   //public GameObject prefab_card;
   private CardFiller filler;

   private Vector3 offset;
   bool isMove;

   private Coroutine show;

   private void Awake()
   {
      filler = GetComponent<CardFiller>();
   }

   // Поднимаем карту
   public void RaiseCard()
   {
      if (Fight.endFight && (isActive || isActualActive)) 
      {
         if(!isFirstMove)
            LowerCard();
         isActive = false;
         isActualActive = false;
         return;
      }
      if (!isFirstMove) startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
       targetPosition =  startPos + new Vector3(0f, 4f, 0f);
       move = StartCoroutine(MoveCard( startPos,  targetPosition, 0.1f));
       //isActualActive = true;
       isFirstMove = true;
   }

   // Опускаем карту
   public void LowerCard()
   {
      if (move != null) StopCoroutine(move);
       move = StartCoroutine(MoveCard( targetPosition,  startPos, 0.1f));
      //isActualActive = false;
   }

   IEnumerator MoveCard(Vector3 startPosition, Vector3 target, float duration)
   {
      float time = 0f;

      while (time < duration)
      {
         transform.position = Vector3.Lerp(startPosition, target, time / duration);
         time += Time.deltaTime;
         yield return null;
      }

      transform.position = target;
      move = null;
   }

   public void Exit()
   {
      if (move != null) StopCoroutine(move);
      if (show != null) StopCoroutine(show);
      move = StartCoroutine(MoveCard(targetPosition, startPos, sec));
      objectUp = null;
   }

   void Update()
   {
      if (isMove)
      {
         Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
         transform.position = new Vector3(mousePos.x + offset.x, mousePos.y + offset.y, transform.position.z);
      }
      if(move == null && (isActive != isActualActive || isLock))
      {
         if (isLock && !isActualActive)
         {
            RaiseCard();
            isActualActive = true;
            return;
         }
         else if (isLock) return;

         if (isActive) RaiseCard();
         else LowerCard();

         isActualActive = isActive;
      }
   }
}
