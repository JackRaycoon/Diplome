using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
   public CanvasGroup loadScreen;
   public float duration = 0.2f;
   public bool isSceneStart = false;

   public void StartScene()
   {
      StartCoroutine(StartCoroutine());
   }
   public void LoadScene(int sceneID)
   {
      StartCoroutine(LoadCoroutine(sceneID));
   }

   private IEnumerator LoadCoroutine(int sceneID)
   {
      loadScreen.interactable = true;
      loadScreen.blocksRaycasts = true;

      float startAlpha = 0f, endAlpha = 1f, elapsed = 0f;
      loadScreen.alpha = startAlpha;

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
      yield return null;
      SceneManager.LoadScene(sceneID);
   }
   private IEnumerator StartCoroutine()
   {
      float startAlpha = 1f, endAlpha = 0f, elapsed = 0f;
      loadScreen.alpha = startAlpha;

      yield return new WaitForSeconds(0.5f);

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
      yield return null;
      loadScreen.interactable = false;
      loadScreen.blocksRaycasts = false;
      isSceneStart = true;
   }
}
