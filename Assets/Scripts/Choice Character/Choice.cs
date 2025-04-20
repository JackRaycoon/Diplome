using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Choice :MonoBehaviour
{
   public SpriteRenderer pictureLeftLeft, pictureLeft, pictureCenter, pictureRight, pictureRightRight;
   private List<PlayableCharacter> allCharacters = new();
   private List<PlayableCharacter> allCharactersTemp;
   private static int playerCount;

   private static int currentPortraitShift = 0;

   private static bool needUpdatePortrait = false;

   public TextMeshProUGUI characterName, characterDescription;

   private void Start()
   {
      var list = new List<CharacterSO>(Resources.LoadAll<CharacterSO>("CharData/Playable/"));
      foreach(var data in list)
      {
         allCharacters.Add(new PlayableCharacter(data.name));
      }
      allCharacters = allCharacters.OrderBy(c => c.charClass).ToList();

      allCharactersTemp = new(allCharacters);

      playerCount = allCharacters.Count;

      needUpdatePortrait = true;
   }

   private void Update()
   {
      if (needUpdatePortrait)
      {
         needUpdatePortrait = false;
         SpriteFill(currentPortraitShift);
      }
   }

   public void SpriteFill(int shift)
   {
      Sprite TempPic;

      //Центр
      pictureCenter.sprite = allCharacters[shift].Portrait;

      Buttons.selectedCharacter = allCharacters[shift];
      characterName.text = allCharacters[shift].Data.character_name;
      characterDescription.text = allCharacters[shift].Data.character_description;

      //Левая
      if (shift - 1 >= 0) TempPic = allCharactersTemp[shift - 1].Portrait;
      else TempPic = allCharactersTemp[allCharactersTemp.Count - 1].Portrait;

      pictureLeft.sprite = TempPic;

      //Правая
      if (shift + 1 < allCharactersTemp.Count) TempPic = allCharactersTemp[shift + 1].Portrait;
      else TempPic = allCharactersTemp[0].Portrait;

      pictureRight.sprite = TempPic;

      // Левая-левая
      if (shift - 2 >= 0)
         TempPic = allCharactersTemp[shift - 2].Portrait;
      else if (shift - 2 == -1)
         TempPic = allCharactersTemp[allCharactersTemp.Count - 1].Portrait;
      else
         TempPic = allCharactersTemp[allCharactersTemp.Count - 2].Portrait;

      pictureLeftLeft.sprite = TempPic;

      // Правая-правая
      if (shift + 2 < allCharactersTemp.Count)
         TempPic = allCharactersTemp[shift + 2].Portrait;
      else if (shift + 2 == allCharactersTemp.Count)
         TempPic = allCharactersTemp[0].Portrait;
      else
         TempPic = allCharactersTemp[1].Portrait;

      pictureRightRight.sprite = TempPic;
   }

   public static void ChangeShift(int change)
   {
      currentPortraitShift += change;

      if (currentPortraitShift < 0)
      {
         currentPortraitShift = (currentPortraitShift % playerCount + playerCount) % playerCount;
      }
      else if (currentPortraitShift >= playerCount)
      {
         currentPortraitShift = currentPortraitShift % playerCount;
      }

      needUpdatePortrait = true;
   }
}
