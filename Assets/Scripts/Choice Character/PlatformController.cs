using System.Collections;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
   private readonly float maxTiltAngle = 30f;
   private readonly float tiltSpeed = 5f;
   private readonly float Z_koef = 26f;
   public GameObject Model;
   private Rigidbody rb;
   private Quaternion defaultRotationCard;
   private Vector3 platformExtents;

   void Start()
   {
      Init();
   }

   public void Init()
   {
      rb = Model.GetComponent<Rigidbody>();
      defaultRotationCard = transform.rotation;
      platformExtents = GetComponent<Collider>().bounds.extents;
   }

   void Update()
   {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;

      if (Physics.Raycast(ray, out hit) && hit.transform == transform && !PlatformMove.is_Block)
      {
         TiltPlatform(hit.point);
      }
      else
      {
         ResetPlatformRotation();
      }
   }
   

   void TiltPlatform(Vector3 hitPoint)
   {
      if (PlatformMove.is_Block) return;
      Vector3 localHitPoint = transform.InverseTransformPoint(hitPoint);

      float offsetX = localHitPoint.x / platformExtents.x;
      float offsetZ = localHitPoint.z / (platformExtents.z * Z_koef);

      float distanceFromCenter = Mathf.Sqrt(offsetX * offsetX + offsetZ * offsetZ);

      float normalizedDistance = Mathf.Clamp01(distanceFromCenter / Mathf.Max(platformExtents.x, platformExtents.z));

      float tiltAngleX = Mathf.Clamp(-offsetZ * maxTiltAngle * normalizedDistance, -maxTiltAngle, maxTiltAngle);
      float tiltAngleZ = Mathf.Clamp(offsetX * maxTiltAngle * normalizedDistance, -maxTiltAngle, maxTiltAngle);

      Quaternion targetRotation = Quaternion.Euler(defaultRotationCard.eulerAngles.x + tiltAngleX, defaultRotationCard.eulerAngles.y, defaultRotationCard.eulerAngles.z + tiltAngleZ);

      rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, tiltSpeed * Time.deltaTime);
   }



   void ResetPlatformRotation()
   {
      // Получаем локальное вращение родителя
      Quaternion parentRotation = rb.transform.parent != null ? rb.transform.parent.rotation : Quaternion.identity;
      // Переводим целевое локальное вращение в глобальные координаты
      Quaternion targetRotation = parentRotation * Quaternion.identity;

      rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, tiltSpeed * Time.fixedDeltaTime);
   }

}

