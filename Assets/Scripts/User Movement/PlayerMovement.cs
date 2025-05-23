using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
   public CharacterController controller;
   Transform cameraTransform;
   public Animator animator;

   public float baseSpeed = 1.5f;
   public float gravity = -9.81f;
   public float sprintSpeed = 3f;

   public float bobbingSpeed = 14f; // �������� �����������
   public float sprintBobbingSpeed = 16f;
   public float bobbingAmount = 0.05f; // ��������� �����������
   public float smoothTransition = 10f; // �������� ����������� �����������

   float speedBoost = 1f;
   Vector3 velocity;
   float timer = 0f;

   private float timerSave = 0f;
   private float intervalSave = 5f;

   Vector3 initialCameraPosition;

   public static CharacterController controllerStatic = null;
   public static Transform cameraTransformStatic;

   void Start()
   {
      controllerStatic = controller;
      cameraTransform = transform;
      cameraTransformStatic = transform;
      var runInfo = SaveLoadController.runInfo;

      controller.enabled = false;
      controller.transform.SetPositionAndRotation(new(runInfo.positionX, runInfo.positionY, runInfo.positionZ), 
         Quaternion.Euler(0f, runInfo.rotationY, 0f));
      controller.enabled = true;
      cameraTransform.rotation = Quaternion.Euler(runInfo.rotationX, 0f, 0f);
      initialCameraPosition = cameraTransform.localPosition; // ��������� ����������� ������� ������
   }

   public static void SavePosition()
   {
      if (controllerStatic == null) return;
      SaveLoadController.runInfo.positionX = controllerStatic.transform.position.x;
      SaveLoadController.runInfo.positionY = controllerStatic.transform.position.y;
      SaveLoadController.runInfo.positionZ = controllerStatic.transform.position.z;
      SaveLoadController.runInfo.rotationX = cameraTransformStatic.eulerAngles.x;
      SaveLoadController.runInfo.rotationY = controllerStatic.transform.eulerAngles.y;
   }

   void FixedUpdate()
   {
      //���������� ���������� � ���� ����������
      timerSave += Time.fixedDeltaTime;

      if (timerSave >= intervalSave)
      {
         timerSave = 0f; // �����
         SaveLoadController.Save();
      }
      // ����������
      if (controller.isGrounded && velocity.y < 0)
      {
         velocity.y = -2f;
      }

      // ��������� ����� ������
      float x = Input.GetAxis("Horizontal");
      float z = Input.GetAxis("Vertical");

      // ����������� ��������
      if (Input.GetButton("Sprint"))
         speedBoost = sprintSpeed;
      else
         speedBoost = 1f;

      // ����������� �������� ������������ ������
      Vector3 camForward = Camera.main.transform.forward;
      camForward.y = 0f;
      camForward.Normalize();

      Vector3 move = camForward * z + Camera.main.transform.right * x;

      // �������� ������
      controller.Move(move * (baseSpeed + speedBoost) * Time.deltaTime);

      // ���������� ����������
      velocity.y += gravity * Time.deltaTime;
      controller.Move(velocity * Time.deltaTime);

      // ���������� ��������
      UpdateAnimator(x, z);

      // ����������� ������
      HandleHeadBobbing(move);
   }

   void UpdateAnimator(float x, float z)
   {
      // ��������� ����� �������� �� ������ �����
      animator.SetBool("isRight", x > 0); // �������� ������
      animator.SetBool("isLeft", x < 0);  // �������� �����
      animator.SetBool("isForward", z > 0); // �������� �����
      animator.SetBool("isBack", z < 0);  // �������� �����

      // ���� ����� �� ���������, ����� ���� ������
      if (x == 0 && z == 0)
      {
         animator.SetBool("isRight", false);
         animator.SetBool("isLeft", false);
         animator.SetBool("isForward", false);
         animator.SetBool("isBack", false);
         animator.SetBool("isSprint", false);
      }
      else
      {
         if (speedBoost != 1f) animator.SetBool("isSprint", true);
         else animator.SetBool("isSprint", false);
      }
   }
   void HandleHeadBobbing(Vector3 movement)
   {
      if (movement.magnitude > 0.1f && controller.isGrounded) // ������ ��� ��������
      {
         if(speedBoost != 1f) 
            timer += Time.deltaTime * (sprintBobbingSpeed);
         else
            timer += Time.deltaTime * (bobbingSpeed);
         float bobbingOffsetY = Mathf.Sin(timer) * (bobbingAmount);
         float bobbingOffsetX = Mathf.Cos(timer * 0.5f) * (bobbingAmount / 2f); // ������� ����� �������������� �����������

         Vector3 targetPosition = initialCameraPosition + new Vector3(bobbingOffsetX, bobbingOffsetY, 0);
         cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPosition, smoothTransition * Time.deltaTime);
      }
      else
      {
         // ���������� ������ � ����������� ���������, ���� ����� �� ���������
         timer = 0f;
         cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, initialCameraPosition, smoothTransition * Time.deltaTime);
      }
   }
}
