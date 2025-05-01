using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pauser : MonoBehaviour
{
   public GameObject pauseMenuUI;
   private bool isPaused = false;
   public Loading loader;

   public static bool needOpenFight = false;

   private void Start()
   {
      StartCoroutine(StartScene());
   }

   IEnumerator StartScene()
   {
      loader.StartScene();
      while (!loader.isSceneStart) 
         yield return null;
      Resume();
   }
   void Update()
   {
      if (Input.GetKeyDown(KeyCode.Escape))
      {
         if (isPaused)
            Resume();
         else
            Pause();
      }

      if (needOpenFight)
      {
         needOpenFight = false;
         ToFight();
      }
   }

   public void Resume()
   {
      Cursor.lockState = CursorLockMode.Locked;
      pauseMenuUI.SetActive(false);
      Time.timeScale = 1f;
      isPaused = false;
   }

   public void Pause()
   {
      Cursor.lockState = CursorLockMode.None;
      pauseMenuUI.SetActive(true);
      Time.timeScale = 0f;
      isPaused = true;
   }

   public void ToMainMenu()
   {
      Time.timeScale = 1f;
      pauseMenuUI.SetActive(false);
      SaveLoadController.Save();
      loader.LoadScene(0);
   }
   public void ToFight()
   {
      Time.timeScale = 1f;
      pauseMenuUI.SetActive(false);
      SaveLoadController.Save();
      loader.LoadScene(2);
   }
}
