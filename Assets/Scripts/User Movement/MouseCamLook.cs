using UnityEngine;

public class MouseCamLook : MonoBehaviour
{
   public Animator animator;
   public Vector3 offset;

   public float mouseSensitivity = 100f;
   public Transform playerBody;
   float xRotation = 0f;

   private Transform headBone;
   void Start()
   {
      Cursor.lockState = CursorLockMode.Locked;
      //headBone = animator.GetBoneTransform(HumanBodyBones.Head);
   }

   /*void FixedUpdate()
   {
      float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
      float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

      xRotation -= mouseY;
      xRotation = Mathf.Clamp(xRotation, 40f, 140f);

      transform.rotation = Quaternion.Euler(0,0,0);
      transform.localRotation = Quaternion.Euler(xRotation, -90f, 0f);

      playerBody.Rotate(Vector3.up * mouseX);

     //if (headBone == null) return;
      //transform.parent = headBone;
      // Задаём смещение от кости головы
      //transform.localPosition = offset;
      // Камера автоматически будет следовать за головой
      //transform.localRotation = Quaternion.identity;
      //transform.position = headBone.position + offset;
      //transform.LookAt(headBone);
   }*/

   private void LateUpdate()
   {
      float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
      float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

      xRotation -= mouseY;
      //xRotation = Mathf.Clamp(xRotation, 40f, 140f);
      xRotation = Mathf.Clamp(xRotation, -50f, 50f);

      transform.rotation = Quaternion.Euler(0, 0, 0);
      //transform.localRotation = Quaternion.Euler(xRotation, -90f, 0f);
      transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

      playerBody.Rotate(Vector3.up * mouseX);
   }
}