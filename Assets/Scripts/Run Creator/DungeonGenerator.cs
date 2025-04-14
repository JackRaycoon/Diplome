using KeySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour
{
   [SerializeField] DungeonSettings settings;
   [SerializeField] GameObject roomPrefab;
   [SerializeField] GameObject corridorPrefab;
   [SerializeField] float offsetRoom;
   [SerializeField] float offsetCorridor;
   [SerializeField] MiniMapUI miniMap;

   [SerializeField] GameObject player;

   private List<Room> createdRooms = new();
   private List<GameObject> createdRoomsGO = new();
   private List<Corridor> createdCorridors = new();
   private List<GameObject> createdCorridorsGO = new();
   private Dictionary<Vector2Int, RoomState> deletedRooms = new();
   private Dictionary<Corridor, CorridorState> deletedCorridors = new();

   public static bool isNeedUpdate = false;

   private Dungeon dungeon;

   private void Awake()
   {
      SaveLoadController.Load();
   }
   private void Start()
   {
      isNeedUpdate = false;
      var structure = SaveLoadController.runInfo.dungeonStructure;
      if (SaveLoadController.runInfo.dungeonStructure == null)
      {
         CreateDungeon(settings);
         structure = SaveLoadController.runInfo.dungeonStructure;
      }
      else
      {
         structure.roomToConnectedRooms = ConnectRooms(structure.rooms);
      }
      dungeon = new Dungeon(structure);
      BuildDungeonMap(true);
      miniMap.CreateDungeonMiniMapUI(dungeon);
   }

   private void Update()
   {
      if (isNeedUpdate)
      {
         isNeedUpdate = false;
         BuildDungeonMap(false);
      }
   }

   //Строит подземелье вокруг currentRoom и очищает всё остальное
   private void BuildDungeonMap(bool firstBuild)
   {
      var currentRoom = SaveLoadController.runInfo.currentRoom;
      if(currentRoom == null && firstBuild)
      {
         currentRoom = SaveLoadController.runInfo.currentCorridor.room1;
      }

      if (currentRoom == null) return;
      foreach (var room in dungeon.rooms)
      {
         if (createdRooms.Contains(room)) continue;
         if (Math.Abs(currentRoom.Coords.x - room.Coords.x) > 2
               || Math.Abs(currentRoom.Coords.y - room.Coords.y) > 2) continue;
         //Создаём комнаты, которые в пределах пары комнат от текущей
         RoomBehaviour roomBeh = Instantiate(roomPrefab, new Vector3(offsetRoom * room.Coords.x, -0.01f, offsetRoom * room.Coords.y),
            Quaternion.identity,
            transform).GetComponent<RoomBehaviour>();
         roomBeh.gameObject.GetComponent<Room3D>().room = room;
         createdRooms.Add(room);
         createdRoomsGO.Add(roomBeh.gameObject);
         bool[] status = new bool[4];
         foreach (var connectedRoom in dungeon.roomToConnectedRooms[room])
         {
            if (room.Coords.y - connectedRoom.Coords.y < 0) //North
            {
               status[0] = true;
            }
            if (room.Coords.y - connectedRoom.Coords.y > 0) //South
            {
               status[1] = true;
            }
            if (room.Coords.x - connectedRoom.Coords.x < 0) //East
            {
               status[2] = true;
            }
            if (room.Coords.x - connectedRoom.Coords.x > 0) //West
            {
               status[3] = true;
            }
         }
         roomBeh.UpdateRoom(status);

         //Восстанавливаем состояние
         if (deletedRooms.Keys.Contains(room.Coords))
         {
            Debug.Log("Здесь как-то сделаем восстановление дверей");
         }
      }
      //Удаляем комнаты, которые слишком далеко от игрока
      for(int i = 0; i<createdRoomsGO.Count; i++)
      {
         Room room = createdRoomsGO[i].GetComponent<Room3D>().room;
         if (Math.Abs(currentRoom.Coords.x - room.Coords.x) > 2
               || Math.Abs(currentRoom.Coords.y - room.Coords.y) > 2)
         {
            var roomBeh = createdRoomsGO[i].GetComponent<RoomBehaviour>();
            bool[] bmas = { 
               roomBeh.doors[0].GetComponent<KeyDoorController>().doorOpen, 
               roomBeh.doors[1].GetComponent<KeyDoorController>().doorOpen,
               roomBeh.doors[2].GetComponent<KeyDoorController>().doorOpen,
               roomBeh.doors[3].GetComponent<KeyDoorController>().doorOpen };
            if (deletedRooms.Keys.Contains(room.Coords)) deletedRooms.Remove(room.Coords);
            deletedRooms.Add(room.Coords, new(bmas));
            createdRooms.Remove(createdRoomsGO[i].GetComponent<Room3D>().room);
            Destroy(createdRoomsGO[i]);
            createdRoomsGO.Remove(createdRoomsGO[i]);
            i--;
         }
      }

      //Создаём коридоры
      foreach (var corridor in dungeon.corridors)
      {
         if (createdCorridors.Contains(corridor)) continue;
         if (Math.Abs(currentRoom.Coords.x - corridor.room1.Coords.x) > 2
               || Math.Abs(currentRoom.Coords.y - corridor.room1.Coords.y) > 2
               || Math.Abs(currentRoom.Coords.x - corridor.room2.Coords.x) > 2
               || Math.Abs(currentRoom.Coords.y - corridor.room2.Coords.y) > 2) continue;
         float xMax = Mathf.Max(corridor.room2.Coords.x, corridor.room1.Coords.x);
         float xOffset = Mathf.Abs(corridor.room2.Coords.x - corridor.room1.Coords.x);
         float xFinalOffset = offsetRoom * xMax - offsetCorridor * xOffset;

         float yMax = Mathf.Max(corridor.room2.Coords.y, corridor.room1.Coords.y);
         float yOffset = Mathf.Abs(corridor.room2.Coords.y - corridor.room1.Coords.y);
         float yFinalOffset = offsetRoom * yMax - offsetCorridor * yOffset;

         GameObject go = Instantiate(corridorPrefab, new Vector3(
            xFinalOffset,
            0,
            yFinalOffset),
            Quaternion.Euler(0, corridor.orientation == CorridorOrientation.Horizontal ? 90 : 0, 0),
            transform);
         createdCorridors.Add(corridor);
         createdCorridorsGO.Add(go);
         go.GetComponent<Corridor3D>().corridor = corridor;
         //Восстанавливаем состояние
         if (deletedCorridors.Keys.Contains(corridor))
         {
            Debug.Log("Здесь как-то сделаем восстановление коридоров");
         }
      }

      //Удаляем коридоры, которые слишком далеко от игрока
      for (int i = 0; i < createdCorridorsGO.Count; i++)
      {
         Corridor corridor = createdCorridorsGO[i].GetComponent<Corridor3D>().corridor;
         if (Math.Abs(currentRoom.Coords.x - corridor.room1.Coords.x) > 2
               || Math.Abs(currentRoom.Coords.y - corridor.room1.Coords.y) > 2
               || Math.Abs(currentRoom.Coords.x - corridor.room2.Coords.x) > 2
               || Math.Abs(currentRoom.Coords.y - corridor.room2.Coords.y) > 2)
         {
            if (deletedCorridors.Keys.Contains(corridor)) deletedCorridors.Remove(corridor);
            deletedCorridors.Add(corridor, new());
            createdCorridors.Remove(createdCorridorsGO[i].GetComponent<Corridor3D>().corridor);
            Destroy(createdCorridorsGO[i]);
            createdCorridorsGO.Remove(createdCorridorsGO[i]);
            i--;
         }
      }
   }

   public void CreateDungeon(DungeonSettings settings)
   {
      DungeonStructure structure = new();
      structure.rooms = CreateRooms();
      structure.roomToConnectedRooms = ConnectRooms(structure.rooms);
      structure.corridors = CreateCorridors(structure.roomToConnectedRooms);

      SaveLoadController.runInfo.dungeonStructure = structure;
   }

   private List<Corridor> CreateCorridors(Dictionary<Room, List<Room>> roomToConnectedRooms)
   {
      List<Corridor> corridors = new();
      List<Room> connectedRooms = new();

      foreach(var room in roomToConnectedRooms.Keys)
      {
         foreach(var connectedRoom in roomToConnectedRooms[room])
         {
            if (connectedRooms.Contains(connectedRoom)) continue;
            var newCorridor = new Corridor(room,connectedRoom);
            corridors.Add(newCorridor);
         }
         connectedRooms.Add(room);
      }
      return corridors;
   }

   private Dictionary<Room, List<Room>> ConnectRooms(List<Room> rooms)
   {
      Dictionary<Room, List<Room>> roomToConnectedRooms = new();
      for (int i = 0; i < rooms.Count; i++)
      {
         List<Room> connectedRooms = new();
         for (int j = 0; j < rooms.Count; j++)
         {
            int xDistance = Mathf.Abs(rooms[i].Coords.x - rooms[j].Coords.x);
            int yDistance = Mathf.Abs(rooms[i].Coords.y - rooms[j].Coords.y);
            if(xDistance + yDistance == 1)
            {
               connectedRooms.Add(rooms[j]);
            }
         }
         roomToConnectedRooms[rooms[i]] = connectedRooms;
      }
      return roomToConnectedRooms;
   }

   private List<Room> CreateRooms()
   {
      List<Room> rooms = new();
      Vector2Int nextCoords = Vector2Int.zero;
      int roomsToCreate = Random.Range(settings.minNumberOfRooms, settings.maxNumberOfRooms);
      List<Vector2Int> usedCoords = new();

      while(roomsToCreate > 0)
      {
         if (rooms.Count > 0) nextCoords = GetNextCoords(nextCoords);
         if (usedCoords.Contains(nextCoords)) continue;
         Room room = new(rooms.Count + 1, nextCoords);
         if (room.Coords.x == 0 && room.Coords.y == 0) 
         {
            string path = $"EventData/Start Events/";
            string name = $"{(int)SaveLoadController.runInfo.PlayerTeam[0].charClass - 2}-0";
            room.eventData = Resources.Load<EventData>(path + name);
            room.eventPath = path;
            room.eventName = name;
            SaveLoadController.runInfo.currentRoom = room; 
         }
         else
         {
            //Генератор ивента будет тут
            int eventID = 0;
            string path = $"EventData/";
            string name = $"{eventID}-0";
            room.eventData = Resources.Load<EventData>(path + name);
            room.eventPath = path;
            room.eventName = name;
         }
         rooms.Add(room);
         usedCoords.Add(nextCoords);
         roomsToCreate--;
      }
      return rooms;
   }

   private Vector2Int GetNextCoords(Vector2Int nextCoords)
   {
      float randomNumber = Random.Range(0, 4);
      switch (randomNumber)
      {
         case 0:
            return new Vector2Int(nextCoords.x - 1, nextCoords.y);
         case 1:
            return new Vector2Int(nextCoords.x + 1, nextCoords.y);
         case 2:
            return new Vector2Int(nextCoords.x, nextCoords.y - 1);
         default:
            return new Vector2Int(nextCoords.x, nextCoords.y + 1);
      }
   }
}
