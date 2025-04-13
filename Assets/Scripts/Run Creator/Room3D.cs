using KeySystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Room3D : MonoBehaviour
{
   public Room room;

   public TextMeshProUGUI eventTitle;
   public TextMeshProUGUI eventText;
   public List<TextMeshProUGUI> eventBtnsText;
   public List<GameObject> eventBtns;

   private EventData data;

   public CanvasGroup eventCG;

   private RoomBehaviour roomB;

   public List<GameObject> invisibleWalls;

   bool isFilled = false;

   private void Start()
   {
      roomB = GetComponent<RoomBehaviour>();
      data = room.eventData;
   }
   private void Update()
   {
      if(room != null && !isFilled)
      {
         isFilled = true;
         FillEvent();
      }
   }

   public void FillEvent()
   {

      if (data.eventType == EventData.EventType.EnteranceEvent && MiniMapUI.currentRoom != room)
      {
         eventCG.alpha = 0;
         isFilled = false;
         return;
      }
      eventCG.alpha = 1;

      eventTitle.text = data.eventName;
      eventText.text = data.eventText;

      int i = 0;
      foreach(var eventData in data.choices)
      {
         eventBtns[i].SetActive(true);
         eventBtnsText[i].text = eventData.textChoice;
         i++;
      }

      for(;i< eventBtns.Count; i++)
      {
         eventBtns[i].SetActive(false);
      }

      if (data.isLockableEvent) 
      {
         StartCoroutine(LockRoom());
      }
      else roomB.LockDoors(true);
   }

   IEnumerator LockRoom()
   {
      foreach(var go in invisibleWalls)
      {
         go.SetActive(true);
      }
      roomB.LockDoors(false);

      yield return new WaitForSeconds(1f);

      foreach (var go in invisibleWalls)
      {
         go.SetActive(false);
      }
   }

   public void ChoiceBtnClick(int choiceID)
   {
      room.eventData = data.choices[choiceID];
      data = room.eventData;
      FillEvent();
   }
}
