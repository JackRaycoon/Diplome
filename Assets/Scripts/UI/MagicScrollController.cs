using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class MagicScrollController : MonoBehaviour
{
   public GameObject magicScrollPrefab;
   public Transform player; // ������� ������ � ����������
   private GameObject currentScroll;
   private Coroutine currentRoutine;
   private float showDistance = 1f;
   private float closeDistance = 3f;

   void Update()
   {
      if (Input.GetKeyDown(KeyCode.I))
      {
         if (currentScroll != null)
         {
            // ��������� ������� � ����� ���������� �����
            if (currentRoutine != null) StopCoroutine(currentRoutine);
            currentRoutine = StartCoroutine(CloseScroll(() =>
            {
               SpawnNewScroll();
            }));
         }
         else
         {
            SpawnNewScroll();
         }
      }

      // �������������� ��������, ���� ����� ������
      if (currentScroll != null && Vector3.Distance(player.position, currentScroll.transform.position) > closeDistance)
      {
         if (currentRoutine != null) StopCoroutine(currentRoutine);
         currentRoutine = StartCoroutine(CloseScroll());
      }
   }

   void SpawnNewScroll()
   {
      Vector3 spawnPos = player.position + player.forward * showDistance;
      spawnPos += new Vector3(0, 1.58f, 0);
      Quaternion spawnRot = Quaternion.LookRotation(player.forward);
      currentScroll = Instantiate(magicScrollPrefab, spawnPos, spawnRot);
      currentRoutine = StartCoroutine(OpenScroll(currentScroll));
   }

   IEnumerator OpenScroll(GameObject scroll)
   {
      Transform cube = scroll.transform;
      Transform canvas = cube.GetComponentInChildren<Canvas>().transform;
      CanvasGroup cg = canvas.GetComponent<CanvasGroup>();
      cg.alpha = 0f;

      // ������� ����������
      cube.localScale = new Vector3(0.66f, 0f, 0.03f);

      float duration = 0.4f;
      float t = 0;
      while (t < duration)
      {
         t += Time.deltaTime;
         float y = Mathf.Lerp(0f, 0.98f, t / duration);
         cube.localScale = new Vector3(0.66f, y, 0.03f);
         yield return null;
      }

      cube.localScale = new Vector3(0.66f, 0.98f, 0.03f);

      // ����� �������
      t = 0;
      duration = 0.3f;
      while (t < duration)
      {
         t += Time.deltaTime;
         cg.alpha = Mathf.Lerp(0f, 1f, t / duration);
         yield return null;
      }

      cg.alpha = 1f;
      cg.interactable = true;
      cg.blocksRaycasts = true;
   }

   IEnumerator CloseScroll(System.Action onComplete = null)
   {
      if (currentScroll == null) yield break;

      Transform cube = currentScroll.transform;
      Transform canvas = cube.GetComponentInChildren<Canvas>().transform;
      CanvasGroup cg = canvas.GetComponent<CanvasGroup>();

      // ������ ������
      float t = 0;
      float duration = 0.3f;
      float startAlpha = cg.alpha;

      cg.interactable = false;
      cg.blocksRaycasts = false;

      while (t < duration)
      {
         t += Time.deltaTime;
         cg.alpha = Mathf.Lerp(startAlpha, 0f, t / duration);
         yield return null;
      }

      cg.alpha = 0f;

      // ��������� scale
      t = 0;
      duration = 0.4f;
      Vector3 startScale = cube.localScale;
      while (t < duration)
      {
         t += Time.deltaTime;
         float y = Mathf.Lerp(startScale.y, 0f, t / duration);
         cube.localScale = new Vector3(0.66f, y, 0.03f);
         yield return null;
      }

      Destroy(currentScroll);
      currentScroll = null;

      onComplete?.Invoke();
   }
}
