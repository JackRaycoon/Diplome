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

   bool isFilled = false;

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
      eventTitle.text = room.eventData.eventName;
      eventText.text = room.eventData.eventText;
      int i = 0;
      foreach(var eventData in room.eventData.choices)
      {
         eventBtns[i].SetActive(true);
         eventBtnsText[i].text = eventData.textChoice;
         i++;
      }
      for(;i< eventBtns.Count; i++)
      {
         eventBtns[i].SetActive(false);
      }
   }
}
