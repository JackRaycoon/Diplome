using KeySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
   public GameObject[] walls; // 0 - North, 1 - South, 2 - East, 3 - West
   public GameObject[] entrances;
   public GameObject[] doors;
   private Room3D room3d;

   private void Start()
   {
      room3d = gameObject.GetComponent<Room3D>();
   }
   public void UpdateRoom(bool[] status) //true for doors
   {
      for (int i = 0; i < status.Length; i++)
      {
         entrances[i].SetActive(status[i]);
         walls[i].SetActive(!status[i]);
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
   }
}
