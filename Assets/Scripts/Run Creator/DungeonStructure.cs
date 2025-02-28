using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonStructure
{
   public List<Room> rooms;
   public List<Corridor> corridors;
   public Dictionary<Room, List<Room>> roomToConnectedRooms;
}
