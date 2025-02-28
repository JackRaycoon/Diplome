using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomState
{
   //public Vector2Int position;
   //public Enemy enemy; //Кто враг в комнате? Если врага нет, то null
   public bool[] doorOpened = {false, false, false, false}; //Открыта ли конкретная дверь
   public short eventCount = 0; //Количество сработавших в комнате ивентов, на 1 комнату не более 2-х ивентов
   //public List<ItemData> itemsInRoom;

   public RoomState(bool[] doorOpenedMas)
   {
      doorOpened = doorOpenedMas;
   }
}