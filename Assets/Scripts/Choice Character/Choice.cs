using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Choice :MonoBehaviour
{
   public SpriteRenderer pictureLeftLeft, pictureLeft, pictureCenter, pictureRight, pictureRightRight;
   readonly List<PlayableCharacter> PlayerTeam = RunInfo.PlayerTeam;
   private static int playerCount;

   private static int currentPortraitShift = 0;

   private static bool needUpdatePortrait = false;

   private void Start()
   {
      PlayerTeam.Add(new PlayableCharacter("Playable Warrior"));
      PlayerTeam.Add(new PlayableCharacter("Playable Archer"));
      PlayerTeam.Add(new PlayableCharacter("Playable Priest"));

      playerCount = PlayerTeam.Count;

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
      pictureCenter.sprite = RunInfo.PlayerTeam[shift].Portrait;

      //Левая
      if (shift - 1 >= 0) TempPic = PlayerTeam[shift - 1].Portrait;
      else TempPic = PlayerTeam[PlayerTeam.Count - 1].Portrait;

      pictureLeft.sprite = TempPic;

      //Правая
      if (shift + 1 < PlayerTeam.Count) TempPic = PlayerTeam[shift + 1].Portrait;
      else TempPic = PlayerTeam[0].Portrait;

      pictureRight.sprite = TempPic;

      // Левая-левая
      if (shift - 2 >= 0)
         TempPic = PlayerTeam[shift - 2].Portrait;
      else if (shift - 2 == -1)
         TempPic = PlayerTeam[PlayerTeam.Count - 1].Portrait;
      else
         TempPic = PlayerTeam[PlayerTeam.Count - 2].Portrait;

      pictureLeftLeft.sprite = TempPic;

      // Правая-правая
      if (shift + 2 < PlayerTeam.Count)
         TempPic = PlayerTeam[shift + 2].Portrait;
      else if (shift + 2 == PlayerTeam.Count)
         TempPic = PlayerTeam[0].Portrait;
      else
         TempPic = PlayerTeam[1].Portrait;

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
