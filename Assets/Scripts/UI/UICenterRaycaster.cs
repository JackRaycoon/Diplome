using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UICenterRaycaster : MonoBehaviour
{
   /*
   public static UICenterRaycaster Instance { get; private set; }

   public Image crosshair;
   public Color normalColor = Color.white;
   public Color hoverColor = Color.red;

   private PointerEventData pointerData;
   private EventSystem eventSystem;
   private readonly List<GraphicRaycaster> raycasters = new();

   void Awake()
   {
      if (Instance != null && Instance != this)
      {
         Destroy(gameObject);
         return;
      }

      Instance = this;
      eventSystem = EventSystem.current;
   }

   void Update()
   {
      if (crosshair == null || eventSystem == null || raycasters.Count == 0)
         return;

      pointerData = new PointerEventData(eventSystem)
      {
         position = new Vector2(Screen.width / 2, Screen.height / 2)
      };

      List<RaycastResult> results = new();
      bool hovered = false;

      foreach (var raycaster in raycasters)
      {
         raycaster.Raycast(pointerData, results);

         foreach (var result in results)
         {
            if (result.gameObject.TryGetComponent<Button>(out var button))
            {
               hovered = true;

               if (Input.GetButtonDown("Fire1"))
                  button.onClick.Invoke();

               break;
            }
         }

         if (hovered) break;
         results.Clear();
      }

      crosshair.color = hovered ? hoverColor : normalColor;
   }

   public void RegisterRaycaster(GraphicRaycaster gr)
   {
      if (gr != null && !raycasters.Contains(gr))
         raycasters.Add(gr);
   }

   public void UnregisterRaycaster(GraphicRaycaster gr)
   {
      if (gr != null && raycasters.Contains(gr))
         raycasters.Remove(gr);
   }*/
}
