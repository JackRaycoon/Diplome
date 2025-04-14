using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MiniMapUI : MonoBehaviour
{
   [SerializeField] GameObject mapContent, fullMapContent;
   [SerializeField] GameObject mapPanel, fullMapPanel;
   [SerializeField] GameObject roomMiniMapPrefab;
   [SerializeField] GameObject playerMiniMapPrefab;
   [SerializeField] GameObject horizontalCorridorMiniMapPrefab, verticalCorridorMiniMapPrefab;
   [SerializeField] private float distanceBetweenRooms;

   public static bool isNeedUpdate = false;
   private Vector3 shift = new(0,0,0);
   private GameObject ZeroRoom;
   //private int stateCorridor = 0; //На какой клетке коридора находимся, от 0 до 4, 0 - не в коридоре.

   public static Room lockedRoom;
   public static Room unlockedRoom;

   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.Tab))
      {
         //Открытие фулл мапы
         mapPanel.SetActive(false);
         fullMapPanel.SetActive(true);
      }
      if (Input.GetKeyUp(KeyCode.Tab))
      {
         //Закрытие фулл мапы
         mapPanel.SetActive(true);
         fullMapPanel.SetActive(false);
      }

      if (isNeedUpdate)
      {
         isNeedUpdate = false;
         ShiftMap();
         DungeonGenerator.isNeedUpdate = true;
         if(lockedRoom != null) LockRoom();
         if(unlockedRoom != null) UnlockRoom();
      }
   }

   private void LockRoom()
   {
      foreach (Transform child in mapContent.transform)
      {
         var fog = child.GetComponent<FogOfWarUI>();
         if (fog.room == lockedRoom)
            child.GetChild(1).gameObject.SetActive(true);
      }
      foreach (Transform child in fullMapContent.transform)
      {
         var fog = child.GetComponent<FogOfWarUI>();
         if (fog.room == lockedRoom)
            child.GetChild(1).gameObject.SetActive(true);
      }
      lockedRoom = null;
   }
   private void UnlockRoom()
   {
      foreach (Transform child in mapContent.transform)
      {
         var fog = child.GetComponent<FogOfWarUI>();
         if (fog.room == unlockedRoom)
            child.GetChild(1).gameObject.SetActive(false);
      }
      foreach (Transform child in fullMapContent.transform)
      {
         var fog = child.GetComponent<FogOfWarUI>();
         if (fog.room == unlockedRoom)
            child.GetChild(1).gameObject.SetActive(false);
      }
      unlockedRoom = null;
   }

   private void ShiftMap()
   {
      var currentRoom = SaveLoadController.runInfo.currentRoom;
      var currentCorridor = SaveLoadController.runInfo.currentCorridor;
      if (SaveLoadController.runInfo.currentRoom != null)
      {
         shift = new(currentRoom.Coords.x * distanceBetweenRooms, currentRoom.Coords.y * distanceBetweenRooms);
      }
      else
      {
         if (currentCorridor.orientation == CorridorOrientation.Horizontal)
         {
            float xPos = (currentCorridor.room1.Coords.x * distanceBetweenRooms + currentCorridor.room2.Coords.x * distanceBetweenRooms) / 2;
            shift = new(xPos, currentCorridor.room1.Coords.y * distanceBetweenRooms);
         }
         else
         {
            float yPos = (currentCorridor.room1.Coords.y * distanceBetweenRooms + currentCorridor.room2.Coords.y * distanceBetweenRooms) / 2;
            shift = new(currentCorridor.room1.Coords.x * distanceBetweenRooms, yPos);
         }
      }
      shift = -shift;
      //shift - то на каких координатах должна находится нулевая комната, а нам нужно то, насколько надо подвинуть текущие координаты
      shift += -ZeroRoom.transform.localPosition;
      foreach (Transform child in mapContent.transform) child.localPosition += shift;
      foreach (Transform child in fullMapContent.transform) child.localPosition += shift;

      FogOfWar();
   }

   private void FogOfWar()
   {
      var currentRoom = SaveLoadController.runInfo.currentRoom;
      //Открываем новые клетки
      if (currentRoom != null)
      {
         List<Room> allRoomList_WithoutFogOfWar = new();
         //Открываем комнаты
         foreach (Transform child in mapContent.transform)
         {
            var fog = child.GetComponent<FogOfWarUI>();
            if (fog.room == null) continue;

            if (Math.Abs(currentRoom.Coords.x - fog.room.Coords.x) < 2
               && Math.Abs(currentRoom.Coords.y - fog.room.Coords.y) < 2)
               fog.isFogOfWar = false;
            if (currentRoom.Coords.x == fog.room.Coords.x
               && currentRoom.Coords.y == fog.room.Coords.y)
               fog.isUnlocked = true;
            if(!fog.isFogOfWar) 
               allRoomList_WithoutFogOfWar.Add(fog.room);
         }
         //Открываем коридоры
         foreach (Transform child in mapContent.transform)
         {
            var fog = child.GetComponent<FogOfWarUI>();
            if (fog.corridor == null) continue;
               
            if(allRoomList_WithoutFogOfWar.Contains(fog.corridor.room1) 
                  && allRoomList_WithoutFogOfWar.Contains(fog.corridor.room2))
               {
                  fog.isFogOfWar = false;
               }
         }

         //Если туман войны есть, то скрываем клетку
         foreach (Transform child in mapContent.transform)
         {
            var fog = child.GetComponent<FogOfWarUI>();
            child.gameObject.SetActive(!fog.isFogOfWar);
            fog.fullMapAnalogue.SetActive(!fog.isFogOfWar);
         }

         //Открываем картинку анлока
         foreach (Transform child in mapContent.transform)
         {
            var fog = child.GetComponent<FogOfWarUI>();
            if (fog.room == null) continue;
            child.GetChild(0).gameObject.SetActive(fog.isUnlocked);
            fog.fullMapAnalogue.transform.GetChild(0).gameObject.SetActive(fog.isUnlocked);
         }
      }
   }

   public void CreateDungeonMiniMapUI(Dungeon dungeon)
   {
      ClearMiniMap();
      foreach (var room in dungeon.rooms)
      {
         //if (room.Coords.x == 0 && room.Coords.y == 0) currentRoom = room;
         CreateFogOfWarUI(room);
      }
      foreach (var corridor in dungeon.corridors)
      {
         CreateCorridorUI(corridor);
      }

      ShiftMap();
   }

   private void CreateCorridorUI(Corridor corridor)
   {
      GameObject goInstance, goInstanceFullMap;
      Vector2 centralPos;

      if(corridor.orientation == CorridorOrientation.Horizontal)
      {
         float xPos = (corridor.room1.Coords.x * distanceBetweenRooms + corridor.room2.Coords.x * distanceBetweenRooms) / 2;
         centralPos = new(xPos, corridor.room1.Coords.y * distanceBetweenRooms);

         goInstance = Instantiate(horizontalCorridorMiniMapPrefab, mapContent.transform);
         goInstanceFullMap = Instantiate(horizontalCorridorMiniMapPrefab, fullMapContent.transform);
      }
      else
      {
         float yPos = (corridor.room1.Coords.y * distanceBetweenRooms + corridor.room2.Coords.y * distanceBetweenRooms) / 2;
         centralPos = new(corridor.room1.Coords.x * distanceBetweenRooms, yPos);

         goInstance = Instantiate(verticalCorridorMiniMapPrefab, mapContent.transform);
         goInstanceFullMap = Instantiate(verticalCorridorMiniMapPrefab, fullMapContent.transform);
      }

      goInstance.transform.localPosition = centralPos;
      goInstance.GetComponent<FogOfWarUI>().corridor = corridor;
      goInstanceFullMap.transform.localPosition = centralPos;
      goInstance.GetComponent<FogOfWarUI>().fullMapAnalogue = goInstanceFullMap;
   }

   private void CreateFogOfWarUI(Room room)
   {
      GameObject go = Instantiate(roomMiniMapPrefab, mapContent.transform);
      GameObject goFullMap = Instantiate(roomMiniMapPrefab, fullMapContent.transform);
      var position = new Vector3(room.Coords.x * distanceBetweenRooms, room.Coords.y * distanceBetweenRooms, 0);
      go.transform.localPosition = position;
      goFullMap.transform.localPosition = position;

      if (room.Coords.x == 0 && room.Coords.y == 0) ZeroRoom = go;
      go.GetComponent<FogOfWarUI>().room = room;
      go.GetComponent<FogOfWarUI>().fullMapAnalogue = goFullMap;
   }

   private void ClearMiniMap()
   {
      foreach (Transform child in mapContent.transform) Destroy(child.gameObject);
      foreach (Transform child in fullMapContent.transform) Destroy(child.gameObject);
   }
}
