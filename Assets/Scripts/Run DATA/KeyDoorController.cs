using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeySystem
{
   public class KeyDoorController : MonoBehaviour
   {
      private Animator doorAnim;
      public bool doorOpen = false;

      [Header("Animation Names")]
      [SerializeField] private string openAnimationName = "DoorOpen";
      [SerializeField] private string closeAnimationName = "DoorClose";
      [SerializeField] private string lockedAnimationName = "DoorLocked";

      [SerializeField] private int timeToShowUI = 1;
      [SerializeField] private GameObject showDoorLockedUI = null;

      [SerializeField] private float waitTimer = 1;
      [SerializeField] private bool pauseInteraction = false;

      private BoxCollider bCollider;
      private bool playerInTrigger;

      public Room room;

      private void Awake()
      {
         doorAnim = gameObject.GetComponent<Animator>();
         bCollider = GetComponent<BoxCollider>();
      }

      private void OnTriggerEnter(Collider other)
      {
         if (other.CompareTag("Player")) playerInTrigger = true;
      } 
      private void OnTriggerExit(Collider other)
      {
         if (other.CompareTag("Player")) playerInTrigger = false;
      }

      private IEnumerator PauseDoorInteraction(bool isTrigger)
      {
         if(isTrigger) GetComponent<Collider>().isTrigger = true;
         pauseInteraction = true;
         yield return new WaitForSeconds(waitTimer);
         pauseInteraction = false;
         while (playerInTrigger) yield return null;
         if (isTrigger) GetComponent<Collider>().isTrigger = false;
      }

      public void PlayAnimation(bool hasKey)
      {
         if (hasKey)
         {
            OpenDoor();
         }
         else
         {
            ShowDoorLocked();
         }
      }

      private void OpenDoor()
      {
         if (!doorOpen && !pauseInteraction)
         {
            showDoorLockedUI.SetActive(false);
            MiniMapUI.unlockedRoom = room;
            MiniMapUI.isNeedUpdate = true;
            doorAnim.Play(openAnimationName, 0, 0.0f);
            doorOpen = true;
            StartCoroutine(PauseDoorInteraction(true));
         }

         else if (doorOpen && !pauseInteraction)
         {
            doorAnim.Play(closeAnimationName, 0, 0.0f);
            doorOpen = false;
            StartCoroutine(PauseDoorInteraction(true));
         }
      }

      void ShowDoorLocked()
      {
         if (!pauseInteraction)
         {
            showDoorLockedUI.SetActive(true);
            MiniMapUI.lockedRoom = room;
            MiniMapUI.isNeedUpdate = true;
            doorAnim.Play(lockedAnimationName, 0, 0.0f);
            StartCoroutine(PauseDoorInteraction(false));
         }
      }
   }
}
