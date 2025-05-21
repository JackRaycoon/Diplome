   using System;
   using System.Collections;
   using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

[DataContract]
public class RunInfo
   {
   [DataMember]
   public List<CharacterSaveData> saveTeam = new();
   [DataMember]
   public ushort badKarma = 0; //На 5 и 10 происходит смена стадии

   [IgnoreDataMember]
   public List<GlobalBuff> globalBuffs = new();
   [DataMember]
   public Locations currentLocation = Locations.Dungeon;
   [DataMember]
   public DungeonStructure dungeonStructure = null;
   [DataMember]
   public Room currentRoom = null; // Где наш герой на данный момент находится
   [DataMember]
   public Corridor currentCorridor = null;

   //Координаты игрока
   [DataMember]
   public float positionX = 0, positionY = 0, positionZ = 2;
   [DataMember]
   public float rotationX = 0, rotationY = 180;
   [DataMember]
   public bool activePotion, passivePotion;
   [DataMember]
   public short slotID;

   public RunInfo() { }
   public RunInfo(short slotID)
   {
      this.slotID = slotID;
   }
   [DataMember]
   public int souls = 0;

   [IgnoreDataMember]
   public List<PlayableCharacter> PlayerTeam = new();

   public enum Locations
   {
      Dungeon
   }
   public enum GlobalBuff
   {
      None,
      SilentBlood,
      AmuletWind,
      TraceAncientRoute,
      TouchingMystery,
      HeartDarkness,
      CursedHand,
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

