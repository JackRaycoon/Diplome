using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class DungeonStructure
{
   [DataMember]
   public List<Room> rooms;
   [DataMember]
   public List<Corridor> corridors;
   [IgnoreDataMember]
   public Dictionary<Room, List<Room>> roomToConnectedRooms;
}
