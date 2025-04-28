using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardFiller : MonoBehaviour
{
   public Skill skill = null;
   private Skill _filledSkill = null;
   public Image sprite;
   public TextMeshProUGUI skillName;
   public TextMeshProUGUI description;
   public GameObject descriptionPanel;

   private string descriptionUpdate = "";

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
      if(description != null)
      {
         skillName.text = skill.skillData._name;
         description.text = $"<i>{skill.Description()}</i>";
         if (descriptionUpdate != "")
         {
            skillName.text = "";
            description.text = descriptionUpdate;
         }
      }
   }

   public void DescriptionUpdate(string newDescription)
   {
      if (description != null)
      {
         descriptionUpdate = newDescription;
         description.text = descriptionUpdate;
      }
   }
   public void DescriptionPanelOpenCloser()
   {
      if (descriptionPanel != null)
         descriptionPanel.SetActive(!descriptionPanel.activeInHierarchy);
   }
}
