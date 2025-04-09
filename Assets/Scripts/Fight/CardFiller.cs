using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardFiller : MonoBehaviour
{
   public Skill skill;
   public Text _name;
   public Text hp;
   public Text atk;
   public Text description;
   public Image image;
   public Transform abilities;
   public GameObject AtkAbility;
   public GameObject prefab_ability;
   //public GameObject CharacterCardback;
   //public GameObject SpellCardback;
   //public bool isCardback;
   //public Card card = null;

   void Start()
    {
      //if(card == null) card = new Card();
      //Fill();
    }

   public void Fill()
   {
      /*if (card != null)
      {
         switch (card.CARD_TYPE)
         {
            case Card.TypeCARD.Character:
            case Card.TypeCARD.Legendary:
               CharacterCardback.SetActive(isCardback);
               _name.text = card.NAME;
               image.sprite = card.SPRITE;
               AtkAbility.SetActive(true);
               hp.gameObject.SetActive(true);
               hp.text = new string('♥', card.HP);
               atk.text = card.ATK.ToString();
               for (int i = 0; i < card.ABILITIES.Count; i++)
               {
                  if (abilities.childCount == i + 1)
                  {
                     GameObject go = Instantiate(prefab_ability, abilities);
                     go.GetComponent<Image>().sprite = Resources.Load<Sprite>(card.ABILITIES[i].keyWord.ToString());
                  }
               }
               break;
            case Card.TypeCARD.Spell:
            case Card.TypeCARD.Quest:
            case Card.TypeCARD.Event:
               SpellCardback.SetActive(isCardback);
               AtkAbility.SetActive(false);
               hp.gameObject.SetActive(false);
               _name.text = card.NAME;
               image.sprite = card.SPRITE;
               break;
            case Card.TypeCARD.Structure:
               SpellCardback.SetActive(isCardback);
               AtkAbility.SetActive(false);
               hp.gameObject.SetActive(true);
               _name.text = card.NAME;
               image.sprite = card.SPRITE;
               hp.text = new string('♥', card.HP);
               foreach (Ability ab in card.ABILITIES)
               {
                  GameObject go = Instantiate(prefab_ability, abilities);
                  go.GetComponent<Image>().sprite = Resources.Load<Sprite>(ab.keyWord.ToString());
               }
               break;
         }
      }
    }*/
   }
}
