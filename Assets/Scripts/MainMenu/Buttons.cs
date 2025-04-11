using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
   private CircularMenu cm;
   public List<CanvasGroup> panels;
   public CanvasGroup dark;
   public float duration = 0.2f; // Время анимации

   private bool isOpened;
   private int openID = -1;
   private Coroutine animate;
   private void Start()
   {
      cm = GetComponent<CircularMenu>();
   }

   private void Update()
   {
      if (Input.GetKeyUp(KeyCode.Mouse1))
      {
         if (animate != null) StopCoroutine(animate);
         animate = StartCoroutine(AnimatePanel(panels[openID], false, false));
         openID = -1;
      }
   }
   public void ButtonClick(int id)
   {
      if (cm.isMoving || openID == id) return;
      if (cm.currentIndex == id)
      {
         if (animate != null) StopCoroutine(animate);
         if (isOpened)
            animate = StartCoroutine(AnimatePanel(panels[openID], false, true, panels[id]));
         else
            animate = StartCoroutine(AnimatePanel(panels[id], true, false));
         openID = id;
      }
      else
      {
         cm.currentIndex = id;
         cm.UpdateButtonPositions(false);
      }
   }

   private IEnumerator AnimatePanel(CanvasGroup panelGroup, bool open, bool needOpen, CanvasGroup panelOpen = null)
   {
      float startAlpha = open ? 0f : 1f;
      float endAlpha = open ? 1f : 0f;

      float t1 = Mathf.InverseLerp(startAlpha, endAlpha, panelGroup.alpha);
      float elapsed = t1 * duration;

      panelGroup.interactable = false;
      panelGroup.blocksRaycasts = false;
      dark.interactable = false;
      dark.blocksRaycasts = false;

      while (elapsed < duration)
      {
         float t = elapsed / duration;
         float alpha = Mathf.Lerp(startAlpha, endAlpha, t);

         panelGroup.alpha = alpha;
         if(!needOpen && dark.alpha != endAlpha)
            dark.alpha = alpha;

         elapsed += Time.deltaTime;
         yield return null;
      }

      // Установка финальных значений
      panelGroup.alpha = endAlpha;
      if (!needOpen)
         dark.alpha = endAlpha;
      isOpened = false;

      if (open)
      {
         panelGroup.interactable = true;
         panelGroup.blocksRaycasts = true;
         isOpened = true;
      }
      if (needOpen)
      {
         animate = StartCoroutine(AnimatePanel(panelOpen, true, false));
      }
   }


   public void ExitYesBtn()
   {
      Application.Quit();
   }
}
