using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Choice :MonoBehaviour
{
   public SpriteRenderer pictureLeftLeft, pictureLeft, pictureCenter, pictureRight, pictureRightRight;
   readonly List<PlayableCharacter> allCharacters = new();
   private List<PlayableCharacter> allCharactersTemp;
   private static int playerCount;

   private static int currentPortraitShift = 0;

   private static bool needUpdatePortrait = false;

   private void Start()
   {
      allCharacters.Add(new PlayableCharacter("Playable Warrior"));
      allCharacters.Add(new PlayableCharacter("Playable Archer"));
      allCharacters.Add(new PlayableCharacter("Playable Priest"));

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
