using KeySystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // Не забудь
using UnityEngine.UI;

public class Raycast : MonoBehaviour
{
   [Header("3D Raycast")]
   private float rayLength = 1.25f;
   private float rayUILength = 1.75f;
   [SerializeField] private LayerMask layerMaskInteract;
   [SerializeField] private string excluseLayerName = null;

   private KeyItemController raycastedObject;
   [SerializeField] private KeyCode openDoorKey = KeyCode.Mouse0;
   [SerializeField] private Image crosshair = null;
   private bool isCrosshairActive;
   private bool doOnce;
   private string interactableTag = "InteractiveObject";

   private EventSystem eventSystem;

   public static Raycast Instance { get; private set; }
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

   private void Update()
   {
      bool hitSomething = false;

      // ---------- 3D Raycast ----------
      RaycastHit hit;
      Vector3 fwd = transform.TransformDirection(Vector3.forward);

      int mask = 1 << LayerMask.NameToLayer(excluseLayerName) | layerMaskInteract.value;

      if (Physics.Raycast(transform.position, fwd, out hit, rayLength, mask))
      {
         if (hit.collider.CompareTag(interactableTag))
         {
            if (!doOnce)
            {
               raycastedObject = hit.collider.GetComponent<KeyItemController>();
               CrosshairChange(true);
            }

            isCrosshairActive = true;
            doOnce = true;

            if (Input.GetKeyDown(openDoorKey))
            {
               raycastedObject.ObjectInteraction();
            }

            hitSomething = true;
         }
      }

      // ---------- UI Raycast ----------
      if (!hitSomething)
      {
         Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

         PointerEventData pointerData = new PointerEventData(eventSystem)
         {
            position = screenCenter
         };

         List<RaycastResult> results = new();

         foreach (var raycaster in raycasters)
         {
            raycaster.Raycast(pointerData, results);

            foreach (var result in results)
            {
               if (result.gameObject.TryGetComponent<Button>(out var button))
               {
                  float distance = Vector3.Distance(Camera.main.transform.position, result.gameObject.transform.position);
                  if (distance > rayUILength) continue;
                  CrosshairChange(true);

                  if (Input.GetKeyDown(openDoorKey))
                     button.onClick.Invoke();

                  hitSomething = true;
                  break;
               }
            }

            if (hitSomething) break;
         }
      }



      // ---------- Nothing hit ----------
      if (!hitSomething && isCrosshairActive)
      {
         CrosshairChange(false);
         doOnce = false;
      }
   }

   private void CrosshairChange(bool on)
   {
      if (on && !doOnce)
      {
         crosshair.color = Color.red;
         isCrosshairActive = true;
      }
      else
      {
         crosshair.color = Color.white;
         isCrosshairActive = false;
      }
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
   }
}
