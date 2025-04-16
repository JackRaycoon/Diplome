using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FightUIController : MonoBehaviour
{
   public GridLayoutGroup heroes, enemies; 

   public static int CountHeroes, CountEnemies;

   private int _countHeroes, _countEnemies;

   public GameObject portraitPrefab;
   public List<RenderTexture> heroesRenderTextures;
   public List<RenderTexture> enemiesRenderTextures;
   public List<RenderTexture> addictsRenderTextures;

   public static bool allInteractable, allDisable;
   private bool _allInteractable, _allDisable;

   //public static bool hardUpdate = false;

   //public static int oneID_heroes = -1;
   //private int _oneID_heroes = -1;
   //public static Fighter one_enemies = null;

   void Start()
    {
      UpdateCount(1);
      UpdateCount(2);
   }


   private void UpdateCount(int commandNum)
   {
      if (commandNum == 1) { _countHeroes = CountHeroes; 
         //_oneID_heroes = oneID_heroes; 
      }
      if (commandNum == 2) _countEnemies = CountEnemies;
      if (commandNum == 0)
      {
         _allInteractable = allInteractable;
         _allDisable = allDisable;
      }

      if(commandNum == 0)
      {
         foreach (Transform go in heroes.transform)
         {
            var fp = go.GetComponent<FightPortrait>();
            if(fp.id < 6)
               go.GetComponent<FightPortrait>().isInteractable = !Fight.AlreadyTurn.Contains(Fight.PlayerTeam[fp.id]);
            if(allDisable) go.GetComponent<FightPortrait>().isInteractable = false;
            if (allInteractable) go.GetComponent<FightPortrait>().isInteractable = true;
            if (_countHeroes == 1 && Fight.selectedSkill != null) fp.isInteractable = false;
         }

         foreach (Transform go in enemies.transform)
         {
            go.GetComponent<FightPortrait>().isInteractable = allInteractable;
         }
      }

      //HEROES
      if (commandNum == 1)
      {
         //if (_oneID_heroes != -1 && _countHeroes != 1) 
         //{ _oneID_heroes = -1; oneID_heroes = -1; }
         foreach (Transform child in heroes.transform)
         {
            Destroy(child.gameObject);
         }
         for (int i = 0; i < _countHeroes; i++)
         {
            var go = Instantiate(portraitPrefab, heroes.transform);
            var fp = go.GetComponent<FightPortrait>();
            List<RenderTexture> renderTextures = new(heroesRenderTextures);
            foreach (var rt in addictsRenderTextures) renderTextures.Add(rt);
            go.transform.GetChild(0).GetComponent<RawImage>().texture = renderTextures[i];
            //fp.id = (_oneID_heroes != -1) ? _oneID_heroes : i;
            //_oneID_heroes = -1;
            //oneID_heroes = -1;
            if (Fight.EnemyTeam.Contains(Fight.PlayerUITeam[i]))
               fp.id = Fight.EnemyTeam.IndexOf(Fight.PlayerUITeam[i]) + 6;
            else
            {
               fp.id = Fight.PlayerTeam.IndexOf(Fight.PlayerUITeam[i] as PlayableCharacter);
            }

            if (fp.id < 6)
               fp.isInteractable = !Fight.AlreadyTurn.Contains(Fight.PlayerTeam[fp.id]);
            if (allDisable) go.GetComponent<FightPortrait>().isInteractable = false;
            if (_countHeroes == 1 && Fight.selectedSkill != null) fp.isInteractable = false;
         }
         UpdateSize(_countHeroes, heroes);
      }

      //ENEMIES
      if (commandNum == 2)
      {
         foreach (Transform child in enemies.transform)
         {
            Destroy(child.gameObject);
         }
         for (int i = 0; i < _countEnemies; i++)
         {
            var go = Instantiate(portraitPrefab, enemies.transform);
            var fp = go.GetComponent<FightPortrait>();
            List<RenderTexture> renderTextures = new(enemiesRenderTextures);
            foreach (var rt in addictsRenderTextures) renderTextures.Add(rt);
            go.transform.GetChild(0).GetComponent<RawImage>().texture = renderTextures[i];

            //fp.id = i + 6;
            fp.isInteractable = false;

            if (Fight.EnemyTeam.Contains(Fight.EnemyUITeam[i]))
               fp.id = Fight.EnemyTeam.IndexOf(Fight.EnemyUITeam[i]) + 6;
            else
               fp.id = Fight.PlayerTeam.IndexOf(Fight.EnemyUITeam[i] as PlayableCharacter);
         }
         UpdateSize(_countEnemies, enemies);
      }
   }

   public void UpdateSize(int count, GridLayoutGroup grid)
   {
      switch (count)
      {
         case 1:
            CharacterUpdate(grid, 400, 480, 80, 80, 68, 50, 70, 1, 10, 80);
            break;
         case 2:
            CharacterUpdate(grid, 300, 360, 60, 70, 51, 40, 50, 1, 5, 70);
            break;
         case 3:
         case 4:
            CharacterUpdate(grid, 250, 300, 50, 65, 34, 30, 40, 2, 0, 60);
            break;
         case 5:
         case 6:
            CharacterUpdate(grid, 200, 240, 40, 60, 23, 15, 30, 2, 0, 50);
            break;
         default: //more then 6
            CharacterUpdate(grid, 150, 180, 30, 55, 23, 15, 30, 3, 0, 45);
            break;
      }
}

   private void CharacterUpdate(GridLayoutGroup grid, float sizeX, float sizeY, float bottomImage,
      float heightSlider, float sliderPosY, float sliderRightMove, float sliderLeftMove, 
      int ColumnCount, float iconPosX, float iconSize)
   {
      grid.cellSize = new Vector2(sizeX, sizeY);
      grid.constraintCount = ColumnCount;

      foreach (Transform child in grid.transform)
      {
         var SpriteRtransform = child.GetChild(0).GetComponent<RectTransform>();
         Vector2 offsetMin = SpriteRtransform.offsetMin;
         offsetMin.y = bottomImage;
         SpriteRtransform.offsetMin = offsetMin;

         var healthPan = child.GetChild(1).GetComponent<RectTransform>();
         Vector3 currentPosition = healthPan.anchoredPosition;
         currentPosition.y = sliderPosY;
         healthPan.anchoredPosition = currentPosition;
         healthPan.offsetMax = new Vector2(-sliderRightMove, healthPan.offsetMax.y);
         healthPan.offsetMin = new Vector2(sliderLeftMove, healthPan.offsetMin.y);
         Vector2 sizeDelta = healthPan.sizeDelta;
         sizeDelta.y = heightSlider;
         healthPan.sizeDelta = sizeDelta;

         var healthIconRTransform = child.GetChild(1).GetChild(1).GetComponent<RectTransform>();
         sizeDelta = healthIconRTransform.sizeDelta;
         sizeDelta.x = iconSize;
         sizeDelta.y = iconSize;
         healthIconRTransform.sizeDelta = sizeDelta;
         Vector3 currentPositionImage = healthIconRTransform.anchoredPosition;
         currentPositionImage.x = iconPosX;
         healthIconRTransform.anchoredPosition = currentPositionImage;
      }
   }
   public static bool hardUpdate;
   void Update()
    {
      if (_countHeroes != CountHeroes)
      {
         UpdateCount(1);
      }
     // if (oneID_heroes != _oneID_heroes)
     //    UpdateCount(1);
      if (_countEnemies != CountEnemies)
         UpdateCount(2);
      if(_allDisable != allDisable || _allInteractable != allInteractable)
         UpdateCount(0);

      if (hardUpdate) //Ћомают и позвол€ют ходить в ход врага -> ќшибка
      {
         hardUpdate = false;
         UpdateCount(1);
         UpdateCount(2);
         UpdateCount(0);
      }
   }
}
