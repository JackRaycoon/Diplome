﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Corridor
{
   public Room room1;
   public Room room2;
   public CorridorOrientation orientation;

   public bool isFogOfWar = true; //Рассеивается когда проходишь на соседней клетке, не обязательно соединённой

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
