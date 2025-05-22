using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CircularMenu : MonoBehaviour
{
   public List<RectTransform> buttons;
   public List<TextMeshProUGUI> textButtons;
   public int currentIndex = 0;
   public float radius = 100f;
   public float angleRange = 120f; // �� -60� �� +60�
   public float yStretch = 1.5f; // ��������, 1.5 ��� 150% ������
   public float lerpSpeed = 10f;

   public SFXPlayer player;

   public bool isMoving;

   private Coroutine[] move = new Coroutine[5];
   float[] alphaLevels = { 1f, 0.3f, 0.1f, 0.02f, 0f };
   float[] sizeLevels = { 170f, 120f, 100f, 70f, 70f };

   void Start()
   {
      UpdateButtonPositions(true);

      for (int i = 0; i < buttons.Count; i++)
      {
         SetButtonOpacity(buttons[i], alphaLevels[i]);
         textButtons[i].fontSize = (i == 0) ? 55 : 45;
      }
   }

   void Update()
   {
      if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxis("Mouse ScrollWheel") > 0f) && currentIndex > 0)
      {
         currentIndex--;
         player.PlaySweep();
         UpdateButtonPositions();
      }
      else if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxis("Mouse ScrollWheel") < 0f) && currentIndex < 4)
      {
         currentIndex++;
         player.PlaySweep();
         UpdateButtonPositions();
      }
   }



   public void UpdateButtonPositions(bool instant = false)
   {
      int count = buttons.Count;
      //float startAngle = -angleRange / 2f;
      float angleStep = angleRange / (count - 1);

      // ����������� ������� ������ ���� �� angle = 0, � ��������� � ���������� ������
      // �� ���� �� ������ ��� �������, � ��������� ������� ������������ ����
      float startAngle = -angleStep * currentIndex;

      for (int i = 0; i < count; i++)
      {
         float angle = startAngle + i * angleStep;

         float y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius * yStretch;
         float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius - radius;

         Vector2 targetPos = new Vector2(x, -y);

         // ��������� ������������ � ����������� �� ����������
         float distance = Mathf.Abs(i - currentIndex);
         float alpha = distance < alphaLevels.Length ? alphaLevels[(int)distance] : 0f;
         
         buttons[i].sizeDelta = new Vector2(buttons[i].sizeDelta.x, sizeLevels[(int)distance]); 

         if (instant)
            buttons[i].anchoredPosition = targetPos;
         else
         {
            if (move[i] != null) StopCoroutine(move[i]);
            move[i] = StartCoroutine(AnimateMove(buttons[i], targetPos, i, alpha, textButtons[i], (distance == 0) ? 55 : 45));
         }
      }
   }

   // ����� ��� ��������� ������������ ������
   void SetButtonOpacity(RectTransform button, float alpha)
   {
      CanvasGroup canvasGroup = button.GetComponent<CanvasGroup>();
      if (canvasGroup == null)
      {
         canvasGroup = button.gameObject.AddComponent<CanvasGroup>();
      }
      canvasGroup.alpha = alpha;
      canvasGroup.interactable = alpha != 0;
      canvasGroup.blocksRaycasts = alpha != 0;
   }

   IEnumerator AnimateMove(RectTransform rect, Vector2 target, int id, float targetAlpha, TextMeshProUGUI text, float targetFontSize)
   {
      isMoving = true;

      float initialAlpha = rect.GetComponent<CanvasGroup>().alpha;
      float initialFontSize = text.fontSize;
      float elapsedTime = 0f;

      Vector2 initialPosition = rect.anchoredPosition;

      while (Vector2.Distance(rect.anchoredPosition, target) > 0.5f)
      {
         float t = elapsedTime * lerpSpeed;

         // ������� �������
         rect.anchoredPosition = Vector2.Lerp(initialPosition, target, t);

         // ������� ������������
         float alpha = Mathf.Lerp(initialAlpha, targetAlpha, t);
         SetButtonOpacity(rect, alpha);

         // ������� ������ ������
         text.fontSize = Mathf.Lerp(initialFontSize, targetFontSize, t);

         elapsedTime += Time.deltaTime;
         yield return null;
      }

      // ��������� ��������
      rect.anchoredPosition = target;
      SetButtonOpacity(rect, targetAlpha);
      text.fontSize = targetFontSize;

      isMoving = false;
      move[id] = null;
   }


}
