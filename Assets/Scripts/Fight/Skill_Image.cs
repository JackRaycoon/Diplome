using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Skill_Image : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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

   private void Start()
   {
      startPos_part1 = part1.transform.position;
      startPos_part2 = part2.transform.position;
      startPosVeil = Veil.transform.position;
   }

   private void Update()
   {
      if (externalSkill != null) { Enter(true); externalSkill = null; }
      if (isDisableExSkill)
      {
         isDisableExSkill = false;
         Exit();
      }
   }

   public void OnPointerEnter(PointerEventData eventData)
   {
      if (!isEnabled) { return; }
      Enter(false);
   }
   public void OnPointerClick(PointerEventData eventData)
   {
      if (!isEnabled || !isClickable) { return; }

      Fight.selectedSkill = skill;
      isEnabled = false;

      Enter(false);
      if (wait != null) StopCoroutine(wait);
      wait = StartCoroutine(Wait());
      if (move3 != null) StopCoroutine(move3);
      move3 = StartCoroutine(MoveToPosition(Veil, pos_Veil.position, 1000f));
   }

   public void OnPointerExit(PointerEventData eventData)
   {
      if (!isEnabled) { return; }
      Exit();
   }

   public void Enter(bool external)
   {
      if (external)
      {
         CenterSkill.sprite = externalSkill.Icon;
         Description.text = externalSkill.Description;
      }
      else
      {
         CenterSkill.sprite = skill.Icon;
         Description.text = skill.Description;
      }

      StartCoroutine(Open());
   }
   public void Exit()
   {
      CenterSkill.sprite = null;

      if (move1 != null) { StopCoroutine(move1); StopCoroutine(move2); }
      move1 = StartCoroutine(MoveToPosition(part1, startPos_part1, 1000f));
      move2 = StartCoroutine(MoveToPosition(part2, startPos_part2, 1000f));
   }

   public IEnumerator Open()
   {
      if (move1 != null) { yield return move1; yield return move2; }
      move1 = StartCoroutine(MoveToPosition(part1, pos_part1.position, 200f));
      move2 = StartCoroutine(MoveToPosition(part2, pos_part2.position, 200f));
   }
   public IEnumerator Wait()
   {
      while (Fight.selectedSkill != null) 
      { 
         //Debug.Log("Wait"); 
         yield return null; 
      }

      isEnabled = true;
      Exit();
      if (move3 != null) StopCoroutine(move3);
      move3 = StartCoroutine(MoveToPosition(Veil, startPosVeil, 1000f));
   }

   public IEnumerator MoveToPosition(GameObject moveObj, Vector3 targetPosition, float moveSpeed)
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
   }
}
