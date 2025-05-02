using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Skill_Image : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
   public static Skill externalSkill = null;
   public static Fighter externalCaster = null;
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

   public static bool isOnCard = false;

   public static bool isNeedClose = false;
   public bool isIntention;

   private void Start()
   {
      startPos_part1 = part1.transform.position;
      startPos_part2 = part2.transform.position;
      startPosVeil = Veil.transform.position;
      externalSkill = null;
      externalCaster = null;
   }

   private void Update()
   {
      if (externalSkill != null && externalCaster != null)
      { 
         Enter(true, externalCaster, externalSkill); 
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

      if (!isOnCard && HandAnimationManager.isOpen && !Fight.isEnemyTurn)
      {
         Exit();
      }
   }

   public void OnPointerEnter(PointerEventData eventData)
   {
      if (!isEnabled || isOneLocked) return;
      if (isIntention && !Fight.seeIntension) return;
      if (Fight.isEnemyTurn) return;
      isOnCard = true;

      Enter(false, Fight.SelectedCharacter());
   }

   public void Action()
   {
      //if(!Fight.isEnemyTurn) 
      if (!isClickable || isIntention) { return; }

      Fight.selectedSkill = skill;
      isEnabled = false;

      Enter(false, Fight.SelectedCharacter());
      if (wait != null) 
         StopCoroutine(wait);
      wait = StartCoroutine(Wait());
   }

   public void OnPointerExit(PointerEventData eventData)
   {
      isOnCard = false;
      if (!isEnabled || isOneLocked) { return; }
      if (isIntention && !Fight.seeIntension) return;
      if (Fight.isEnemyTurn) return;
      Exit();
   }



   public void Enter(bool external, Fighter caster, Skill externalSkill = null)
   {
      //StartCoroutine(Open(external, externalSkill));
      if (external)
         HandAnimationManager.Instance.RequestOpen(externalSkill, caster);
      else
         HandAnimationManager.Instance.RequestOpen(skill, caster);
   }
   public void Exit()
   {
      //if (move1 != null) { StopCoroutine(move1); StopCoroutine(move2); }
      //move1 = StartCoroutine(MoveToPosition(part1, startPos_part1, 200f));
      //move2 = StartCoroutine(MoveToPosition(part2, startPos_part2, 200f));

      //StartCoroutine(Close());
      HandAnimationManager.Instance.RequestClose();
   }

   private bool isMoving = false;

   /*
   public IEnumerator Close()
   {
      // Ждём пока идёт предыдущее движение
      while (isMoving)
         yield return null;

      isMoving = true;

      move1 = StartCoroutine(MoveToPosition(part1, startPos_part1, 200f));
      move2 = StartCoroutine(MoveToPosition(part2, startPos_part2, 200f));

      // Подождём пока обе анимации завершатся
      yield return move1;
      yield return move2;

      isMoving = false;
   }

   public IEnumerator Open(bool external, Skill externalSkill)
   {
      while (isMoving)
         yield return null;

      isMoving = true;

      if (external)
      {
         Description.text = externalSkill.Name + "\n";
         Description.text += "<i>" + externalSkill.Description() + "</i>";
      }
      else
      {
         Description.text = skill.Name + "\n";
         Description.text += "<i>" + skill.Description() + "</i>";
      }

      move1 = StartCoroutine(MoveToPosition(part1, pos_part1.position, 100f));
      move2 = StartCoroutine(MoveToPosition(part2, pos_part2.position, 100f));

      yield return move1;
      yield return move2;

      isMoving = false;
   }
   */

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
