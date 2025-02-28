using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
   private int roomId;
   private Vector2Int coords;

   public Room(int id, Vector2Int roomCoords)
   {
      roomId = id;
      coords = roomCoords;
   }

   public Vector2Int Coords { get => coords; }
}
