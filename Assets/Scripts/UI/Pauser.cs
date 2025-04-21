using UnityEngine;
using UnityEngine.SceneManagement;

public class Pauser : MonoBehaviour
{
   public GameObject pauseMenuUI;
   private bool isPaused = false;

   void Update()
   {
      if (Input.GetKeyDown(KeyCode.Escape))
      {
         if (isPaused)
            Resume();
         else
            Pause();
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
      SceneManager.LoadScene(0);
   }
}
