using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
   private CircularMenu cm;
   private void Start()
   {
      cm = GetComponent<CircularMenu>();
   }
   public void NewRun()
   {
      if (cm.isMoving) return;

      if(cm.currentIndex == 0)
         SceneManager.LoadScene("RunScene");
      else
      {
         cm.currentIndex = 0;
         cm.UpdateButtonPositions(false);
      }
   }
   public void Continue()
   {
      if (cm.isMoving) return;

      if (cm.currentIndex == 1)
      {
         Debug.Log("Continue");
      }
      else
      {
         cm.currentIndex = 1;
         cm.UpdateButtonPositions(false);
      }
   }
   public void Credits()
   {
      if (cm.isMoving) return;

      if (cm.currentIndex == 2)
      {
         Debug.Log("Credits");
      }
      else
      {
         cm.currentIndex = 2;
         cm.UpdateButtonPositions(false);
      }
   }
   public void Options()
   {
      if (cm.isMoving) return;

      if (cm.currentIndex == 3)
      {
         Debug.Log("Options");
      }
      else
      {
         cm.currentIndex = 3;
         cm.UpdateButtonPositions(false);
      }
   }
   public void Exit()
   {
      if (cm.isMoving) return;

      if (cm.currentIndex == 4)
      {
         Application.Quit();
         Debug.Log("Exit");
      }
      else
      {
         cm.currentIndex = 4;
         cm.UpdateButtonPositions(false);
      }
   }
}
