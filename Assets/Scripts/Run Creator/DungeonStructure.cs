using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DungeonStructure
{
   public List<Room> rooms;
   public List<Corridor> corridors;
   [NonSerialized]
   public Dictionary<Room, List<Room>> roomToConnectedRooms;
}
