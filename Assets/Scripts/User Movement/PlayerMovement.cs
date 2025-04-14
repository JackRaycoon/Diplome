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

   public float bobbingSpeed = 14f; // Скорость покачивания
   public float sprintBobbingSpeed = 16f;
   public float bobbingAmount = 0.05f; // Амплитуда покачивания
   public float smoothTransition = 10f; // Скорость сглаживания покачивания

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
      initialCameraPosition = cameraTransform.localPosition; // Сохраняем изначальную позицию камеры
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
      //Записываем координаты в файл сохранения
      timerSave += Time.fixedDeltaTime;

      if (timerSave >= intervalSave)
      {
         timerSave = 0f; // сброс
         SaveLoadController.Save();
      }
      // Гравитация
      if (controller.isGrounded && velocity.y < 0)
      {
         velocity.y = -2f;
      }

      // Получение ввода игрока
      float x = Input.GetAxis("Horizontal");
      float z = Input.GetAxis("Vertical");

      // Определение скорости
      if (Input.GetButton("Sprint"))
         speedBoost = sprintSpeed;
      else
         speedBoost = 1f;

      // Направление движения относительно камеры
      Vector3 camForward = Camera.main.transform.forward;
      camForward.y = 0f;
      camForward.Normalize();

      Vector3 move = camForward * z + Camera.main.transform.right * x;

      // Движение игрока
      controller.Move(move * (baseSpeed + speedBoost) * Time.deltaTime);

      // Применение гравитации
      velocity.y += gravity * Time.deltaTime;
      controller.Move(velocity * Time.deltaTime);

      // Обновление анимации
      UpdateAnimator(x, z);

      // Покачивание камеры
      HandleHeadBobbing(move);
   }

   void UpdateAnimator(float x, float z)
   {
      // Обновляем флаги анимации на основе ввода
      animator.SetBool("isRight", x > 0); // Движение вправо
      animator.SetBool("isLeft", x < 0);  // Движение влево
      animator.SetBool("isForward", z > 0); // Движение вперёд
      animator.SetBool("isBack", z < 0);  // Движение назад

      // Если игрок не двигается, сброс всех флагов
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
      if (movement.magnitude > 0.1f && controller.isGrounded) // Только при движении
      {
         if(speedBoost != 1f) 
            timer += Time.deltaTime * (sprintBobbingSpeed);
         else
            timer += Time.deltaTime * (bobbingSpeed);
         float bobbingOffsetY = Mathf.Sin(timer) * (bobbingAmount);
         float bobbingOffsetX = Mathf.Cos(timer * 0.5f) * (bobbingAmount / 2f); // Добавим лёгкое горизонтальное покачивание

         Vector3 targetPosition = initialCameraPosition + new Vector3(bobbingOffsetX, bobbingOffsetY, 0);
         cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPosition, smoothTransition * Time.deltaTime);
      }
      else
      {
         // Возвращаем камеру в изначальное положение, если игрок не двигается
         timer = 0f;
         cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, initialCameraPosition, smoothTransition * Time.deltaTime);
      }
   }
}
