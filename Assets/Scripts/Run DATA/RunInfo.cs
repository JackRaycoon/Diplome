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
            Locations.Dungeon => "Подземелье",
            _ => "",
         };
      }
   }

