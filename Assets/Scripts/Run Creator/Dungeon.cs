using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon
{
   private DungeonStructure structure;

   public Dungeon(DungeonStructure structure)
   {
      this.structure = structure;
   }

   public List<Room> rooms { get => structure.rooms; }
   public List<Corridor> corridors { get => structure.corridors; }
   public Dictionary<Room, List<Room>> roomToConnectedRooms { get => structure.roomToConnectedRooms; }
}
