using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Skill_Image : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
   public static Skill externalSkill = null;
   public static bool isDisableExSkill;

   public Skill skill;

   public GameObject part1, part2, Veil;
   public Transform pos_part1, pos_part2, pos_Veil;
   private Vector3 startPos_part1, startPos_part2, startPosVeil;

   public Image CenterSkill;
   public TMP_Text Description;

   public static Coroutine move1, move2, move3, wait;

   public static bool isEnabled = true;

   public bool isClickable = true;

   public static bool isOneLocked = false;

   public static bool isNeedClose = false;

   private void Start()
   {
      startPos_part1 = part1.transform.position;
      startPos_part2 = part2.transform.position;
      startPosVeil = Veil.transform.position;
   }

   private void Update()
   {
      if (externalSkill != null)
      { 
         Enter(true); 
         externalSkill = null; 
      }

      if (isDisableExSkill)
      {
         isDisableExSkill = false;
         Exit();
      }

      if (isNeedClose)
      {
         isNeedClose = false;
         Exit();
      }
   }

   public void OnPointerEnter(PointerEventData eventData)
   {
      if (!isEnabled || isOneLocked) { return; }
      Enter(false);
   }

   public void Action()
   {
      if (!isClickable) { return; }

      Fight.selectedSkill = skill;
      isEnabled = false;

      Enter(false);
      if (wait != null) 
         StopCoroutine(wait);
      wait = StartCoroutine(Wait());
      //if (move3 != null) StopCoroutine(move3);
      //move3 = StartCoroutine(MoveToPosition(Veil, pos_Veil.position, 200f));
      //Debug.Log("Veil move to zero");
   }
   /*public void OnPointerClick(PointerEventData eventData)
   {
      
   }*/

   public void OnPointerExit(PointerEventData eventData)
   {
      if (!isEnabled || isOneLocked) { return; }
      Exit();
   }

   public void Enter(bool external)
   {
      StartCoroutine(Open(external));
   }
   public void Exit()
   {
      //CenterSkill.sprite = null;

      if (move1 != null) { StopCoroutine(move1); StopCoroutine(move2); }
      move1 = StartCoroutine(MoveToPosition(part1, startPos_part1, 200f));
      move2 = StartCoroutine(MoveToPosition(part2, startPos_part2, 200f));
   }

   public IEnumerator Open(bool external)
   {
      if (move1 != null) { yield return move1; yield return move2; }
      if (external)
      {
         CenterSkill.sprite = externalSkill.Icon;
         Description.text = externalSkill.Name + "\n";
         Description.text += externalSkill.Description();
      }
      else
      {
         CenterSkill.sprite = skill.Icon;
         Description.text = skill.Name + "\n";
         Description.text += skill.Description();
      }
      move1 = StartCoroutine(MoveToPosition(part1, pos_part1.position, 100f));
      move2 = StartCoroutine(MoveToPosition(part2, pos_part2.position, 100f));
   }
   public IEnumerator Wait()
   {
      while (Fight.selectedSkill != null) 
      { 
         yield return null; 
      }

      isEnabled = true;
      Exit();
   }

   public IEnumerator MoveToPosition(GameObject moveObj, Vector3 targetPosition, float moveSpeed, Sprite sprite = null, string description = "")
   {
      Vector3 startPosition = moveObj.transform.position;
      float distance = Vector3.Distance(startPosition, targetPosition);

      while (distance > 0.01f)
      {
         float step = moveSpeed * Time.deltaTime;
         moveObj.transform.position = Vector3.MoveTowards(moveObj.transform.position, targetPosition, step);

         distance = Vector3.Distance(moveObj.transform.position, targetPosition);

         yield return null;
      }

      moveObj.transform.position = targetPosition;
      if(sprite != null)
      {
         CenterSkill.sprite = sprite;
         Description.text = description;
      }
   }
}
