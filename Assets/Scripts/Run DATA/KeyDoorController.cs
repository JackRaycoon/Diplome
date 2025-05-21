using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeySystem
{
   public class KeyDoorController : MonoBehaviour
   {
      public Animator doorAnim;
      public bool doorOpen = false;

      [Header("Animation Names")]
      [SerializeField] private string openAnimationName = "DoorOpen";
      [SerializeField] private string closeAnimationName = "DoorClose";
      [SerializeField] private string lockedAnimationName = "DoorLocked";

      [SerializeField] private int timeToShowUI = 1;
      [SerializeField] private List<MeshRenderer> showDoorLockedUI = null;

      [SerializeField] private float waitTimer = 1;
      [SerializeField] private bool pauseInteraction = false;

      public List<Material> lockMaterials = new();

      private BoxCollider bCollider;
      private bool playerInTrigger;

      public short roomSide = -1;

      public Room room;

      private KeyItemController itemController;

      private void Awake()
      {
         doorAnim = gameObject.GetComponent<Animator>();
         bCollider = GetComponent<BoxCollider>();
         itemController = GetComponent<KeyItemController>();
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
            StartCoroutine(OpenDoor(false));
         }
         else
         {
            ShowDoorLocked();
         }
      }

      public IEnumerator OpenDoor(bool needPause)
      {
         while (pauseInteraction && needPause)
         {
            yield return null;
         }
         if (!doorOpen && !pauseInteraction)
         {
            foreach(var go in showDoorLockedUI)
               go.gameObject.SetActive(false);
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

         //Сохраняем новое состояние
         room.doorOpened[roomSide] = doorOpen;
      }

      void ShowDoorLocked()
      {
         if (!pauseInteraction)
         {
            foreach (var go in showDoorLockedUI)
            {
               switch (itemController.objectType)
               {
                  case KeyItemController.ObjectType.RedDoor:
                     go.material = lockMaterials[1];
                     MiniMapUI.lockedRoom = room;
                     MiniMapUI.isNeedUpdate = true;
                     break;
                  case KeyItemController.ObjectType.LockDoor:
                     go.material = lockMaterials[0];
                     break;
               }
               go.gameObject.SetActive(true);
            }
            AnimatorStateInfo stateInfo = doorAnim.GetCurrentAnimatorStateInfo(0);
            bool isPlaying = !doorAnim.IsInTransition(0) && stateInfo.normalizedTime < 1f;
            if (isPlaying) return;
            doorAnim.Play(lockedAnimationName, 0, 0.0f);
            StartCoroutine(PauseDoorInteraction(false));
         }
      }
   }
}
