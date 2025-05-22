using System;
using System.IO;
using UnityEngine;
using GameDevWare.Serialization;

public static class OptionsManager
{
   public static GameOptions gameOptions = new();

   public static void Save()
   {
      // ������������ � MessagePack
      using (MemoryStream memoryStream = new MemoryStream())
      {
         MsgPack.Serialize(gameOptions, memoryStream);

         // ������ � ����
         string path = Application.persistentDataPath + "/options";
         using (FileStream fs = File.Create(path))
         {
            fs.Write(memoryStream.ToArray(), 0, (int)memoryStream.Length);
         }
      }
   }

   public static void Load()
   {
      string path = Application.persistentDataPath + "/options";

      if (!File.Exists(path))
      {
         gameOptions = new GameOptions();
         return;
      }

      try
      {
         byte[] fileBytes = File.ReadAllBytes(path);
         using (MemoryStream memoryStream = new MemoryStream(fileBytes))
         {
            gameOptions = MsgPack.Deserialize<GameOptions>(memoryStream);
         }
      }
      catch (Exception e)
      {
         Debug.Log(e);
         gameOptions = new GameOptions();
      }
   }
}
