using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardFiller : MonoBehaviour
{
   public Skill skill = null;
   private Skill _filledSkill = null;
   public Image sprite;

   void Update()
    {
      if(skill != _filledSkill)
      {
         _filledSkill = skill;
         Fill();
      }
    }

   public void Fill()
   {
      sprite.sprite = skill.skillData.icon;
   }
}
