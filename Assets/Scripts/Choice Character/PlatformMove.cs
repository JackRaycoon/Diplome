using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMove : MonoBehaviour
{
   public float moveTime = 10f;

   public bool is_Next = true; //ƒвигаем вперЄд или назад?
   public GameObject leftleft, left, center, right, rightright;

   public Transform pos1, pos2, pos3, pos0, pos4;

   public static bool is_Block;

   private void Update()
   {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;

      if (Physics.Raycast(ray, out hit) && hit.transform == transform && !is_Block)
      {
         if (Input.GetKeyDown(KeyCode.Mouse0))
         {
            is_Block = true;
            StartCoroutine(Move());
         }
      }
   }

   public IEnumerator Move()
   {
      Coroutine lastCoroutine;
      if (is_Next)
      {
         StartCoroutine(MoveToPosition(rightright, pos3));
         StartCoroutine(MoveToPosition(right, pos2));
         StartCoroutine(MoveToPosition(center, pos1));
         lastCoroutine = StartCoroutine(MoveToPosition(left, pos0));
      }
      else
      {
         StartCoroutine(MoveToPosition(leftleft, pos1));
         StartCoroutine(MoveToPosition(left, pos2));
         StartCoroutine(MoveToPosition(center, pos3));
         lastCoroutine = StartCoroutine(MoveToPosition(right, pos4));
      }

      yield return lastCoroutine;

      if (is_Next) Choice.ChangeShift(1);
      else Choice.ChangeShift(-1);

      yield return null;

      ToNormalPosition();
      is_Block = false;
   }

   IEnumerator MoveToPosition(GameObject moveObj, Transform targetPosition)
   {
      Vector3 startPosition = moveObj.transform.position;
      Quaternion startRotation = moveObj.transform.rotation;

      float startTime = Time.time;

      while (true)
      {
         float elapsedTime = Time.time - startTime;
         float t = elapsedTime / moveTime;

         moveObj.transform.position = Vector3.Lerp(startPosition, targetPosition.position, t);
         moveObj.transform.rotation = Quaternion.Lerp(startRotation, targetPosition.rotation, t);

         yield return null;

         if (t >= 1.0f)
            break;
      }

      moveObj.transform.position = targetPosition.position;
      moveObj.transform.rotation = targetPosition.rotation;
   }

   public void ToNormalPosition()
   {
      leftleft.transform.position = pos0.position;
      leftleft.transform.rotation = pos0.rotation;

      left.transform.position = pos1.position;
      left.transform.rotation = pos1.rotation;

      center.transform.position = pos2.position;
      center.transform.rotation = pos2.rotation;

      right.transform.position = pos3.position;
      right.transform.rotation = pos3.rotation;

      rightright.transform.position = pos4.position;
      rightright.transform.rotation = pos4.rotation;
   }
}
