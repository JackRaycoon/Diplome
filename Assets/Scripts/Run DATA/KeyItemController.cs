using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeySystem
{
   public class KeyItemController : MonoBehaviour
   {
      public ObjectType objectType;

      public enum ObjectType
      {
         ZeroDoor,
         LockDoor, //Always lock
         FogDoor, //Без двери (эффект тумана)
         RedDoor,
         RedKey
      }
      private KeyDoorController doorObject;

      private void Start()
      {
         if (objectType == ObjectType.RedDoor || objectType == ObjectType.ZeroDoor || objectType == ObjectType.LockDoor)
         {
            doorObject = GetComponent<KeyDoorController>();
         }
      }

      public void ObjectInteraction()
      {
         switch (objectType)
         {
            case ObjectType.ZeroDoor:
               doorObject.PlayAnimation(KeyInventory.hasZeroKey);
               break;
            case ObjectType.RedDoor:
               doorObject.PlayAnimation(KeyInventory.hasRedKey);
               break;
            case ObjectType.RedKey:
               KeyInventory.hasRedKey = true;
               gameObject.SetActive(false);
               break;
            case ObjectType.LockDoor:
               doorObject.PlayAnimation(false);
               break;
         }
      }
   }
}