using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Room
{
   public int roomId;
   private int coordX,coordY;

   public bool isFogOfWar = true; //Рассеивается когда проходишь на соседней клетке, не обязательно соединённой
   public bool isUnlocked; //Открывается когда прошёл непосредственно по этой клетке (был там)

   public string eventPath;
   public string eventName;
   public bool eventRewardClaim = false;
   public string eventRewardText = "";
   public bool[] doorOpened = { false, false, false, false };
   [NonSerialized]
   private EventData _eventData = null;
   public EventData eventData 
   { 
      get 
      {
         if(_eventData == null)
            _eventData = Resources.Load<EventData>(eventPath + eventName);
         return _eventData;
      }
      set
      {
         _eventData = value;
      }
   }


   public Room(int id, Vector2Int roomCoords)
   {
      roomId = id;
      coordX = roomCoords.x;
      coordY = roomCoords.y;
   }

   public Vector2Int Coords { get => new(coordX,coordY); }
}
