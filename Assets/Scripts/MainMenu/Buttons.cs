using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
   public void NewRun()
   {
      SceneManager.LoadScene("RunScene");
   }
   public void Continue()
   {
      //Application.Quit();
   }
   public void Credits()
   {
      //Application.Quit();
   }
   public void Options()
   {
      //Application.Quit();
   }
   public void Exit()
   {
      Application.Quit();
   }
}
