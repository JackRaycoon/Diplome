using KeySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
   public GameObject[] walls; // 0 - North, 1 - South, 2 - East, 3 - West
   public GameObject[] entrances;
   public GameObject[] doors;
   public GameObject[] lampOn;
   public GameObject[] lampOff;
   public GameObject[] particles; 

   private Room3D room3d;

   public GameObject eventCanvasGO;
   private Canvas eventCanvas;
   private CanvasGroup eventCanvasCG;

   public bool[] status;

   public List<KeyItemController.ObjectType> doorsObjectTypes = new();

   private void Start()
   {
      room3d = gameObject.GetComponent<Room3D>();
      eventCanvas = eventCanvasGO.GetComponent<Canvas>();
      eventCanvasCG = eventCanvasGO.GetComponent<CanvasGroup>();

      eventCanvasCG.alpha = 1;
      eventCanvas.worldCamera = Camera.main;

      for(short i = 0; i < doors.Length; i++)
      {
         var key = doors[i].GetComponent<KeyItemController>();
         doorsObjectTypes.Add(key.objectType);
         doors[i].GetComponent<KeyDoorController>().roomSide = i;
      }
   }
   public void UpdateRoom() //true for doors
   {
      for (int i = 0; i < status.Length; i++)
      {
         var key = doors[i].GetComponent<KeyItemController>();
         entrances[i].SetActive(status[i]);
         walls[i].SetActive(!status[i]);
         lampOn[i].SetActive(status[i]);
         lampOff[i].SetActive(!status[i]);
         doors[i].SetActive(key.objectType != KeyItemController.ObjectType.FogDoor);
      }
   }
   bool isFilled = false;
   private void Update()
   {
      if(!isFilled && room3d.room != null)
      {
         isFilled = true;
         foreach(GameObject door in doors)
         {
            door.GetComponent<KeyDoorController>().room = room3d.room;
         }
      }
      RotateCanvasToCamera();
   }

   private void RotateCanvasToCamera()
   {
      if (Camera.main == null || eventCanvasGO == null)
      {
         Debug.LogWarning("Camera.main или eventCanvasGO == null");
         return;
      }

      Vector3 direction = eventCanvasGO.transform.position - Camera.main.transform.position;
      direction.y = 0;

      if (direction.sqrMagnitude > 0.001f)
      {
         Quaternion targetRot = Quaternion.LookRotation(direction);
         eventCanvasGO.transform.rotation = Quaternion.Slerp(
             eventCanvasGO.transform.rotation,
             targetRot,
             Time.deltaTime * 5f // скорость поворота
         );
      }
   }

   public void LockDoors(bool isUnlock)
   {
      if (isUnlock)
      {
         UnlockDoors();
      }
      else
      {
         foreach (GameObject door in doors)
         {
            door.GetComponent<KeyItemController>().objectType = KeyItemController.ObjectType.LockDoor;
            var keyDoor = door.GetComponent<KeyDoorController>();
            if (keyDoor.doorOpen)
            {
               StartCoroutine(keyDoor.OpenDoor(true));
            }
         }
      }
   }
   private void UnlockDoors()
   {
      for(int i = 0; i< doors.Length; i++)
      {
         doors[i].GetComponent<KeyItemController>().objectType = doorsObjectTypes[i];
      }
   }

   public void LoadDoorState(bool[] doorStates)
   {
      for (int i = 0; i < doors.Length; i++)
      {
         var door = doors[i].GetComponent<KeyDoorController>();
         if (doorStates[i])
            door.doorAnim.Play("DoorOpen", 0, 1f);
         else
            door.doorAnim.Play("DoorClose", 0, 1f);
         door.doorAnim.Update(0f);
         door.doorOpen = doorStates[i];
      }
   }

   public void WallUp()
   {
      for(int i = 0; i<doors.Length; i++)
      {
         entrances[i].SetActive(false);
         doors[i].SetActive(false);
         walls[i].SetActive(true);
      }
   }
   public void UnWallUp()
   {
      UpdateRoom();
   }

   public void FogOff()
   {
      foreach (var go in particles)
         go.SetActive(false);
   }
}
