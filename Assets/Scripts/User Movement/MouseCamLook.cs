using TMPro;
using UnityEngine;

public class MouseCamLook : MonoBehaviour
{
   public Animator animator;
   public Vector3 offset;

   public float mouseSensitivity = 100f;
   public Transform playerBody;
   float xRotation = 0f;

   public TextMeshProUGUI compassText;

   private Transform headBone;
   void Start()
   {
      Cursor.lockState = CursorLockMode.Locked;
   }

   private void LateUpdate()
   {
      float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
      float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

      xRotation -= mouseY;
      xRotation = Mathf.Clamp(xRotation, -50f, 50f);

      transform.rotation = Quaternion.Euler(0, 0, 0);
      transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

      playerBody.Rotate(Vector3.up * mouseX);

      CompassUpdate(playerBody.rotation.eulerAngles.y);
   }

   private void CompassUpdate(float rotationY)
   {
      string[] directions = {
        "С",     // 0°   (North)
        "С-В",   // 45°
        "В",     // 90°  (East)
        "Ю-В",   // 135°
        "Ю",     // 180° (South)
        "Ю-З",   // 225°
        "З",     // 270° (West)
        "С-З"    // 315°
      };

      int sector = Mathf.RoundToInt(rotationY / 45f) % 8;
      string direction = directions[sector];

      compassText.text = direction;
   }
}