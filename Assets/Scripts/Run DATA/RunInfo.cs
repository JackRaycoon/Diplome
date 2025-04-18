   using System;
   using System.Collections;
   using System.Collections.Generic;
   using Unity.VisualScripting;
   using UnityEngine;

   [Serializable]
   public class RunInfo
   {
      public List<CharacterSaveData> saveTeam = new();
      public Locations currentLocation = Locations.Dungeon;

      public DungeonStructure dungeonStructure = null;
      public Room currentRoom = null; // ��� ��� ����� �� ������ ������ ���������
      public Corridor currentCorridor = null;

      //���������� ������
      public float positionX = 0, positionY = 0, positionZ = 2;
      public float rotationX = 0, rotationY = 180;

      public short slotID;

      public RunInfo(short slotID)
      {
         this.slotID = slotID;
      }

      public int goldCount = 0;

      [NonSerialized]
      public List<PlayableCharacter> PlayerTeam = new();

      public enum Locations
      {
         Dungeon
      }

      public string RusTranslateLocation()
      {
         return currentLocation switch
         {
            Locations.Dungeon => "����������",
            _ => "",
         };
      }
   }

