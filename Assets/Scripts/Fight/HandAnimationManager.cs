using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class HandAnimationManager : MonoBehaviour
{
   public static HandAnimationManager Instance; // —инглтон дл€ простоты доступа

   private Coroutine openRoutine;
   private Coroutine closeRoutine;
   private bool isClosing = false;

   private Coroutine move1;
   private Coroutine move2;
   private bool isMoving = false;
   private bool pendingOpenRequest = false;
   private Skill pendingSkill;
   private Fighter pendingFighter;

   public Transform part1, part2;
   private Vector3 startPos_part1, startPos_part2;
   public Transform pos_part1, pos_part2;

   public TMP_Text Description;

   public static bool isOpen;

   private void Awake()
   {
      Instance = this;
      startPos_part1 = part1.transform.position;
      startPos_part2 = part2.transform.position;
   }
   public void RequestOpen(Skill externalSkill, Fighter caster)
   {
      if (isClosing)
      {
         // »дЄт закрытие Ч запоминаем, что нужно открыть после
         pendingOpenRequest = true;
         pendingSkill = externalSkill;
         pendingFighter = caster;
         return;
      }

      // »дЄт открытие Ч прерываем
      if (openRoutine != null)
      {
         StopCoroutine(openRoutine);
         openRoutine = null;
      }

      openRoutine = StartCoroutine(OpenCoroutine(externalSkill, caster));
   }

   public void RequestClose()
   {
      if (isClosing)
         return;

      if (openRoutine != null)
      {
         StopCoroutine(openRoutine);
         openRoutine = null;
      }

      closeRoutine = StartCoroutine(CloseCoroutine());
   }

   private IEnumerator OpenCoroutine(Skill externalSkill, Fighter caster)
   {
      // «десь можно помен€ть текст (если нужно)
      // Ќапример:
      Description.text = externalSkill.Name + "\n" + "<i>" + externalSkill.Description(true, caster) + "</i>";

      Vector3 target1 = pos_part1.position;
      Vector3 target2 = pos_part2.position;

      while (Vector3.Distance(part1.position, target1) > 0.01f || Vector3.Distance(part2.position, target2) > 0.01f)
      {
         part1.position = Vector3.MoveTowards(part1.position, target1, 100f * Time.deltaTime);
         part2.position = Vector3.MoveTowards(part2.position, target2, 100f * Time.deltaTime);
         yield return null;
      }

      part1.position = target1;
      part2.position = target2;

      openRoutine = null;
      isOpen = true;
   }

   private IEnumerator CloseCoroutine()
   {
      isClosing = true;
      pendingOpenRequest = false; // —бросим пока что

      Vector3 target1 = startPos_part1;
      Vector3 target2 = startPos_part2;

      while (Vector3.Distance(part1.position, target1) > 0.01f || Vector3.Distance(part2.position, target2) > 0.01f)
      {
         part1.position = Vector3.MoveTowards(part1.position, target1, 200f * Time.deltaTime);
         part2.position = Vector3.MoveTowards(part2.position, target2, 200f * Time.deltaTime);
         yield return null;
      }

      part1.position = target1;
      part2.position = target2;

      isClosing = false;
      closeRoutine = null;

      // ѕосле полного закрыти€ Ч если запрашивали открытие, делаем его
      isOpen = false;
      if (pendingOpenRequest && pendingSkill != null)
      {
         RequestOpen(pendingSkill, pendingFighter);
         pendingOpenRequest = false;
         pendingSkill = null;
         pendingFighter = null;
      }
   }

   /*
   private IEnumerator MoveToPosition(Transform part, Vector3 targetPos, float speed)
   {
      while (Vector3.Distance(part.position, targetPos) > 0.01f)
      {
         part.position = Vector3.MoveTowards(part.position, targetPos, speed * Time.deltaTime);
         yield return null;
      }
      part.position = targetPos;
   }*/
}
