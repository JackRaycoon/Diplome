using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeySystem
{
   public class KeyItemController : MonoBehaviour
   {
      [SerializeField] private ObjectType objectType;

      public enum ObjectType
      {
         ZeroDoor,
         RedDoor,
         RedKey,
         GoodBtn,
         BadBtn,
         NeutralBtn1,
         NeutralBtn2
      }
      private KeyDoorController doorObject;

      private void Start()
      {
         if (objectType == ObjectType.RedDoor || objectType == ObjectType.ZeroDoor)
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
            case ObjectType.GoodBtn:
               break;
            case ObjectType.BadBtn:
               break;
            case ObjectType.NeutralBtn1:
               break;
            case ObjectType.NeutralBtn2:
               break;
         }
      }
   }
}