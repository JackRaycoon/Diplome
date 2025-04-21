using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class DynamicCardSpacing : MonoBehaviour
{
   public int maxVisibleCards = 12;        // ������� ���� ���������� ��� ������
   public float normalSpacing = -80f;      // ����������� ������ ����� �������
   public float minSpacing = -110f;        // ����������� (����� �������������, ���� ����� overlap)

   private HorizontalLayoutGroup layout;

   void Start()
   {
      layout = GetComponent<HorizontalLayoutGroup>();
   }

   void Update()
   {
      int cardCount = transform.childCount;

      float t = Mathf.Clamp01((float)(cardCount - 1) / (maxVisibleCards - 1));
      layout.spacing = Mathf.Lerp(normalSpacing, minSpacing, t);
   }
}
