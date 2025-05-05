using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class Room
{
   [DataMember]
   public int roomId;
   [DataMember]
   public int coordX,coordY;
   [DataMember]
   public bool isFogOfWar = true; //Рассеивается когда проходишь на соседней клетке, не обязательно соединённой
   [DataMember]
   public bool isUnlocked; //Открывается когда прошёл непосредственно по этой клетке (был там)
   [DataMember]
   public RoomType roomType = RoomType.Common;
   [DataMember]
   public string eventPath;
   [DataMember]
   public string eventName;
   [DataMember]
   public bool eventRewardClaim = false;
   [DataMember]
   public string eventRewardText = "";
   [DataMember]
   public List<bool> hiddenVariant = new();
   [DataMember]
   public bool[] doorOpened = { false, false, false, false };
   [IgnoreDataMember]
   private EventData _eventData = null;
   [IgnoreDataMember]
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

   public Room() { }
   public Room(int id, Vector2Int roomCoords)
   {
      roomId = id;
      coordX = roomCoords.x;
      coordY = roomCoords.y;
   }

   [IgnoreDataMember]
   public Vector2Int Coords { get => new(coordX,coordY); }

   public enum RoomType
   {
      Common,
      Shop,
      Boss
   }
}
