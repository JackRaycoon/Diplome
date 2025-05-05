using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class Corridor
{
   [DataMember]
   public Room room1;
   [DataMember]
   public Room room2;
   [DataMember]
   public CorridorOrientation orientation;
   [DataMember]
   public bool isFogOfWar = true; //Рассеивается когда проходишь на соседней клетке, не обязательно соединённой

   public Corridor() { }
   public Corridor(Room room1, Room room2)
   {
      this.room1 = room1;
      this.room2 = room2;

      if (room1.Coords.x != room2.Coords.x)
         orientation = CorridorOrientation.Horizontal;
      else
         orientation = CorridorOrientation.Vertical;
   }
}
public enum CorridorOrientation
{
   Vertical,
   Horizontal
}
