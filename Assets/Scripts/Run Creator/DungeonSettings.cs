using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonSettings", menuName = "Dungeon Settings")]
public class DungeonSettings : ScriptableObject
{
   public int minNumberOfRooms, maxNumberOfRooms;
}
