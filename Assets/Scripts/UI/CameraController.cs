using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform CameraPos1, CameraPos2;
    public float moveTime = 10f;

   void Start()
    {
        
    }

   public void CameraMove(int indexPos)
   {
      switch (indexPos)
      {
         case 1:
            StartCoroutine(MoveToPosition(gameObject, CameraPos1));
            break;
         case 2:
            StartCoroutine(MoveToPosition(gameObject, CameraPos2));
            break;
      }
   }

   IEnumerator MoveToPosition(GameObject moveObj, Transform targetPosition)
   {
      Vector3 startPosition = moveObj.transform.position;
      Quaternion startRotation = moveObj.transform.rotation;

      float startTime = Time.time;

      while (true)
      {
         // Прошедшее время с начала движения
         float elapsedTime = Time.time - startTime;
         // Нормализованное время (0.0 до 1.0) для интерполяции
         float t = elapsedTime / moveTime;

         // Интерполируем позицию и вращение
         moveObj.transform.position = Vector3.Lerp(startPosition, targetPosition.position, t);
         moveObj.transform.rotation = Quaternion.Lerp(startRotation, targetPosition.rotation, t);

         // Ждем следующий кадр
         yield return null;

         // Завершаем корутину, если объект достиг цели
         if (t >= 1.0f)
            break;
      }

      // Убедитесь, что объект точно достиг конечной позиции и ориентации
      moveObj.transform.position = targetPosition.position;
      moveObj.transform.rotation = targetPosition.rotation;
   }
}
