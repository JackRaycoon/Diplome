using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CorridorEnters : MonoBehaviour
{
   [SerializeField] byte numberRoom = 0;
   [SerializeField] Corridor3D corridor3d;
   [SerializeField] Transform partnerTrigger;

   private Vector3 lastPlayerPos;
   private bool isPlayerInTrigger;

   private void OnTriggerEnter(Collider other)
   {
      if (!other.CompareTag("Player")) return;
      isPlayerInTrigger = true;
      lastPlayerPos = other.transform.position;
   }

   private void OnTriggerExit(Collider other)
   {
      if (!other.CompareTag("Player") || !isPlayerInTrigger) return;

      var currentPlayerPos = other.transform.position;
      var moveDirection = (currentPlayerPos - lastPlayerPos).normalized;

      var triggerDirection = (partnerTrigger.position - transform.position).normalized;

      //������ � �������, ��������� � �������
      if (SaveLoadController.runInfo.currentRoom != null)
      {
         if (Vector3.Dot(moveDirection, triggerDirection) < 0) return;
         SaveLoadController.runInfo.currentCorridor = corridor3d.corridor;
         SaveLoadController.runInfo.currentRoom = null;
      }
      //������ � ��������, ������� � �������
      else if(SaveLoadController.runInfo.currentCorridor != null)
      {
         if (Vector3.Dot(moveDirection, triggerDirection) > 0) return;

         if (corridor3d.corridor.room1.Coords.x < corridor3d.corridor.room2.Coords.x ||
            corridor3d.corridor.room1.Coords.y < corridor3d.corridor.room2.Coords.y)
         {
            if (numberRoom == 0)
               SaveLoadController.runInfo.currentRoom = corridor3d.corridor.room1;
            else
               SaveLoadController.runInfo.currentRoom = corridor3d.corridor.room2;
         }
         else
         {
            if (numberRoom == 0)
               SaveLoadController.runInfo.currentRoom = corridor3d.corridor.room2;
            else
               SaveLoadController.runInfo.currentRoom = corridor3d.corridor.room1;
         }
         SaveLoadController.runInfo.currentCorridor = null;
      }
      MiniMapUI.isNeedUpdate = true;
      isPlayerInTrigger = false;
   }
}
