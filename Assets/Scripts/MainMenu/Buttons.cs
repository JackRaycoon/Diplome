using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
   private CircularMenu cm;
   private CanvasGroup cg;

   public CanvasGroup choiseHeroBtn;

   public List<CanvasGroup> panels;
   public CanvasGroup dark;
   public CanvasGroup loadScreen;
   public float duration = 0.2f; // Время анимации

   private bool isOpened;
   private int openID = -1;
   private Coroutine animate;
   private short choisenSlot = 0;

   public TextMeshProUGUI continueText;

   private bool newGameSlot1 = true, newGameSlot2 = true, newGameSlot3 = true;
   private bool isLoading;
   private bool isChangedMenu;

   private void Start()
   {
      cm = GetComponent<CircularMenu>();
      cg = GetComponent<CanvasGroup>();

      //Загрузка здесь

      //отключаем "продолжить"
      continueText.color = (newGameSlot1 && newGameSlot2 && newGameSlot3) ? 
         new Color(0.4528302f, 0.4528302f, 0.4528302f) : new Color(1f, 1f, 1f);
   }

   private void Update()
   {
      if (isLoading) return;
      if (Input.GetKeyUp(KeyCode.Mouse1) || Input.GetKeyUp(KeyCode.Escape))
      {
         if (isChangedMenu)
         {
            isChangedMenu = false;
            StartCoroutine(ChangeMenuBack());
         }
         else
            CloseAllPanels();
      }
      if (Input.GetKeyUp(KeyCode.Return))
      {
         ButtonClick(cm.currentIndex);
      }
   }
   public void ButtonClick(int id)
   {
      if (cm.isMoving || openID == id) return;
      if (cm.currentIndex == id)
      {
         if (id == 1 && (newGameSlot1 && newGameSlot2 && newGameSlot3)) return;
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
   public void CloseAllPanels()
   {
      if (openID == -1) return;
      if (animate != null) StopCoroutine(animate);
      animate = StartCoroutine(AnimatePanel(panels[openID], false, false));
      openID = -1;
      choisenSlot = 0;
   }

   public void NewRunBtn(int slot)
   {
      choisenSlot = (short)slot;
      StartCoroutine(ChangeMenu());
   }
   public void ContinueBtn(int slot)
   {
      //Просто загружаем сцену с нужными данными, не забыть проверку на то что слот не новый
      choisenSlot = (short)slot;
      StartCoroutine(LoadScene());
   }

   IEnumerator ChangeMenu()
   {
      if (animate != null) StopCoroutine(animate);
      animate = StartCoroutine(AnimatePanel(panels[0], false, true, panels[5]));
      openID = 5;

      float startAlpha = 1f, endAlpha = 0f, elapsed = 0f;
      float startAlpha2 = 0f, endAlpha2 = 1f; ;
      cg.alpha = startAlpha;
      cg.interactable = false;
      cg.blocksRaycasts = false;

      choiseHeroBtn.alpha = startAlpha2;
      choiseHeroBtn.interactable = false;
      choiseHeroBtn.blocksRaycasts = false;

      while (elapsed < duration)
      {
         float t = elapsed / duration;
         float alpha = Mathf.Lerp(startAlpha, endAlpha, t);

         cg.alpha = alpha;

         elapsed += Time.deltaTime;
         yield return null;
      }
      // Установка финальных значений
      cg.alpha = endAlpha;

      while (elapsed < duration)
      {
         float t = elapsed / duration;
         float alpha2 = Mathf.Lerp(startAlpha2, endAlpha2, t);

         choiseHeroBtn.alpha = alpha2;

         elapsed += Time.deltaTime;
         yield return null;
      }

      // Установка финальных значений
      choiseHeroBtn.alpha = endAlpha2;
      choiseHeroBtn.interactable = true;
      choiseHeroBtn.blocksRaycasts = true;
      isChangedMenu = true;
   }
   IEnumerator ChangeMenuBack()
   {
      if (animate != null) StopCoroutine(animate);
      animate = StartCoroutine(AnimatePanel(panels[5], false, true, panels[0]));
      openID = 0;

      float startAlpha2 = 1f, endAlpha2 = 0f, elapsed = 0f;
      float startAlpha = 0f, endAlpha = 1f; ;
      cg.alpha = startAlpha;
      cg.interactable = false;
      cg.blocksRaycasts = false;

      choiseHeroBtn.alpha = startAlpha2;
      choiseHeroBtn.interactable = false;
      choiseHeroBtn.blocksRaycasts = false;


      while (elapsed < duration)
      {
         float t = elapsed / duration;
         float alpha2 = Mathf.Lerp(startAlpha2, endAlpha2, t);

         choiseHeroBtn.alpha = alpha2;

         elapsed += Time.deltaTime;
         yield return null;
      }

      // Установка финальных значений
      choiseHeroBtn.alpha = endAlpha2;

      while (elapsed < duration)
      {
         float t = elapsed / duration;
         float alpha = Mathf.Lerp(startAlpha, endAlpha, t);

         cg.alpha = alpha;

         elapsed += Time.deltaTime;
         yield return null;
      }
      // Установка финальных значений
      cg.alpha = endAlpha;
      cg.interactable = true;
      cg.blocksRaycasts = true;
      isChangedMenu = false;
   }

   IEnumerator LoadScene()
   {
      //Переход к сцене для выбранного слота
      //choisenSlot
      isLoading = true;
      float startAlpha = 0f, endAlpha = 1f, elapsed = 0f;
      loadScreen.alpha = startAlpha;
      loadScreen.interactable = true;
      loadScreen.blocksRaycasts = true;

      while (elapsed < duration)
      {
         float t = elapsed / duration;
         float alpha = Mathf.Lerp(startAlpha, endAlpha, t);

         loadScreen.alpha = alpha;

         elapsed += Time.deltaTime;
         yield return null;
      }

      // Установка финальных значений
      loadScreen.alpha = endAlpha;
      SceneManager.LoadScene(1);
   }
}
