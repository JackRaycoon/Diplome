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
   private Room3D room3d;

   public GameObject eventCanvasGO;
   private Canvas eventCanvas;
   private CanvasGroup eventCanvasCG;

   private void Start()
   {
      room3d = gameObject.GetComponent<Room3D>();
      eventCanvas = eventCanvasGO.GetComponent<Canvas>();
      eventCanvasCG = eventCanvasGO.GetComponent<CanvasGroup>();

      eventCanvasCG.alpha = 1;
      eventCanvas.worldCamera = Camera.main;
   }
   public void UpdateRoom(bool[] status) //true for doors
   {
      for (int i = 0; i < status.Length; i++)
      {
         entrances[i].SetActive(status[i]);
         walls[i].SetActive(!status[i]);
         lampOn[i].SetActive(status[i]);
         lampOff[i].SetActive(!status[i]);
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

         RotateCanvasToCamera();
      }
   }

   private void RotateCanvasToCamera()
   {
      if (Camera.main == null || eventCanvasGO == null)
      {
         Debug.LogWarning("Camera.main или eventCanvasGO == null");
         return;
      }

      Vector3 direction = Camera.main.transform.position - eventCanvasGO.transform.position;
      direction.y = 0;

      if (direction.sqrMagnitude > 0.001f)
      {
         Quaternion targetRot = Quaternion.LookRotation(direction);
         eventCanvasGO.transform.rotation = Quaternion.Slerp(
             eventCanvasGO.transform.rotation,
             targetRot,
             Time.deltaTime * 5f // скорость поворота
         );
         Debug.Log("Поворот");
      }
   }

}
