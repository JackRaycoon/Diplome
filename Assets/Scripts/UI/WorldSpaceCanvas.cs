using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
public class WorldSpaceCanvas : MonoBehaviour
{
   private GraphicRaycaster raycaster;

   void OnEnable()
   {
      raycaster = GetComponent<GraphicRaycaster>();
      if (Raycast.Instance != null)
         Raycast.Instance.RegisterRaycaster(raycaster);
   }

   void OnDisable()
   {
      if (Raycast.Instance != null)
         Raycast.Instance.UnregisterRaycaster(raycaster);
   }
}
