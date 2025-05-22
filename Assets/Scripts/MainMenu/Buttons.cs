using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
   private CircularMenu cm;
   private CanvasGroup cg;

   public CanvasGroup choiseHeroBtn;

   public List<CanvasGroup> panels;
   public CanvasGroup dark;
   //public CanvasGroup loadScreen;
   public Loading load;
   public float duration = 0.2f; // Время анимации

   public List<Button> slotsContinue;

   public List<TextMeshProUGUI> sliderTexts;
   public List<Slider> sliders;

   public AudioMixer mixer;
   public SFXPlayer player;

   private bool isOpened;
   private int openID = -1;
   private Coroutine animate;

   public TextMeshProUGUI continueText;

   private bool[] newGameSlots = {true, true, true};
   private bool isLoading;
   private bool isChangedMenu;

   public static PlayableCharacter selectedCharacter = null;

   private void Awake()
   {
      cm = GetComponent<CircularMenu>();
      cg = GetComponent<CanvasGroup>();

      selectedCharacter = null;

      //Загрузка здесь
      SaveLoadController.Load();
      for (short i = 1; i <= newGameSlots.Length; i++)
      {
         bool exist = SaveLoadController.ExistSave(i);
         newGameSlots[i - 1] = !exist;
         slotsContinue[i - 1].interactable = exist;
         slotsContinue[i - 1].gameObject.GetComponent<CanvasGroup>().alpha = exist ? 1f : 0.5f;
      }

      OptionsManager.Load();
      StartCoroutine(ApplyOptions());

      //отключаем "продолжить"
      continueText.color = (newGameSlots[0] && newGameSlots[1] && newGameSlots[2]) ? 
         new Color(0.4528302f, 0.4528302f, 0.4528302f) : new Color(1f, 1f, 1f);

      Cursor.lockState = CursorLockMode.None;
   }

   private void Start()
   {
      load.StartScene();
   }

   private void Update()
   {
      if (isLoading) return;
      if ((Input.GetKeyUp(KeyCode.Mouse1) || Input.GetKeyUp(KeyCode.Escape)) && animate == null)
      {
         if (isChangedMenu)
         {
            player.PlayClick();
            isChangedMenu = false;
            isSure = false;
            selectedCharacter = null;
            StartCoroutine(ChangeMenuBack());
         }
         else if (isSureMenu)
         {
            player.PlayClick();
            DontSureBtn();
         }
         else
            CloseAllPanels();
      }
      if (Input.GetKeyUp(KeyCode.Return))
      {
         ButtonClick(cm.currentIndex);
      }
   }

   private void OnApplicationQuit()
   {
      if (openID == 3)
         CloseOptionWindow();
   }

   public void ButtonClick(int id)
   {
      if (cm.currentIndex == id)
      {
         player.PlayClick();
         if (cm.isMoving || openID == id) return;

         if (id == 1 && (newGameSlots[0] && newGameSlots[1] && newGameSlots[2])) return;
         if (animate != null) StopCoroutine(animate);
         if (isOpened)
            animate = StartCoroutine(AnimatePanel(panels[openID], false, true, panels[id]));
         else
            animate = StartCoroutine(AnimatePanel(panels[id], true, false));
         openID = id;
      }
      else
      {
         player.PlaySweep();
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
      isOpened = false;

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

      if (open)
      {
         panelGroup.interactable = true;
         panelGroup.blocksRaycasts = true;
         isOpened = true;
      }
      animate = null;
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
      player.PlayClick();
      if (openID == 3) CloseOptionWindow();
      if (animate != null) StopCoroutine(animate);
      animate = StartCoroutine(AnimatePanel(panels[openID], false, false));
      openID = -1;
      SaveLoadController.slot = 0;
      selectedCharacter = null;
   }

   bool isSure = false;
   bool isSureMenu = false;
   public void NewRunBtn(int slot)
   {
      SaveLoadController.slot = (short)slot;
      if (!newGameSlots[slot - 1] && !isSure)
      {
         if (animate != null) StopCoroutine(animate);
         animate = StartCoroutine(AnimatePanel(panels[0], false, true, panels[6]));
         isSureMenu = true;
      }
      else
      {
         StartCoroutine(ChangeMenu());
      }
   }

   public void SureBtn()
   {
      isSure = true;
      NewRunBtn(SaveLoadController.slot);
      if (animate != null) StopCoroutine(animate);
      animate = StartCoroutine(AnimatePanel(panels[6], false, false));
      isSureMenu = false;
   } 
   public void DontSureBtn()
   {
      isSure = false;
      if (animate != null) StopCoroutine(animate);
      animate = StartCoroutine(AnimatePanel(panels[6], false, true, panels[0]));
      isSureMenu = false;
   }

   public void BeginNewRun()
   {
      if (PlatformMove.is_Block) return;
      LoadScene(true);
   }
   public void ContinueBtn(int slot)
   {
      SaveLoadController.slot = (short)slot;
      LoadScene(false);
   }
   
   public void OpenLink(string link)
   {
      Application.OpenURL(link);
   }

   public void UpdateSlider(int id)
   {
      var slider = sliders[id - 1];
      var sliderText = sliderTexts[id - 1];
      float rawValue = slider.value;         // 0–100
      float normalized = rawValue / 100f;             // 0.0–1.0
      float dBVolume = normalized < 0.01f ? -80f : Mathf.Lerp(-20f, 0f, normalized);

      switch (id)
      {
         case 1: // Громкость звуков
            OptionsManager.gameOptions.sfxVolume = (short)rawValue;
            sliderText.text = $"Громкость звука: {Mathf.RoundToInt(rawValue)}%";
            mixer.SetFloat("SFX_Volume", dBVolume);
            break;

         case 2: // Громкость музыки
            OptionsManager.gameOptions.bgmVolume = (short)rawValue;
            sliderText.text = $"Громкость музыки: {Mathf.RoundToInt(rawValue)}%";
            mixer.SetFloat("BGM_Volume", dBVolume);
            break;
      }
   }

   private void CloseOptionWindow()
   {
      OptionsManager.Save();
   }

   IEnumerator ApplyOptions()
   {
      sliders[0].value = OptionsManager.gameOptions.sfxVolume;
      sliders[1].value = OptionsManager.gameOptions.bgmVolume;
      yield return null;
      UpdateSlider(1);
      UpdateSlider(2);
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

   void LoadScene(bool isNewGame)
   {
      //Переход к игре для выбранного слота
      isLoading = true;

      //Здесь устанавливаем параметры
      if (isNewGame)
      {
         //Новая игра
         SaveLoadController.ClearSave(SaveLoadController.slot);
         SaveLoadController.Load();
         SaveLoadController.runInfo.PlayerTeam = new List<PlayableCharacter>() { selectedCharacter };
         SaveLoadController.Save();
      }
      else
      {
         SaveLoadController.Load();
      }

      //Экран загрузки
      load.LoadScene(1);
   }
}
