using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
   public short slotID;
   public TextMeshProUGUI slotName;
   public Image classImage;
   public TextMeshProUGUI className;
   public TextMeshProUGUI locationName; //Может писать + % прохождения?

   public Sprite uncnownCharacter;

    void Start()
    {
      slotName.text = $"Слот сохранения {slotID}";
      if (SaveLoadController.ExistSave(slotID))
      {
         Fill();
      }
      else
      {
         FillNew();
      }
    }

   private void Fill()
   {
      //Пока чисто первого, с кого начинали игру
      //Потом можно будет сделать чтобы показывался сильнейший (хотя бы по сумме хар-к)
      classImage.sprite = SaveLoadController.runInfoSlots[slotID - 1].PlayerTeam[0].Portrait;
      className.text = SaveLoadController.runInfoSlots[slotID - 1].PlayerTeam[0].Data.character_name;
      locationName.text = SaveLoadController.runInfoSlots[slotID - 1].RusTranslateLocation();
   }
   private void FillNew()
   {
      classImage.sprite = uncnownCharacter;
      className.text = "Персонаж не выбран";
      locationName.text = "Новое сохранение";
      if (SaveLoadController.corruptedSlots[slotID - 1])
      {
         locationName.text = "Повреждённое сохранение";
      }
   }
}
