//using UnityEngine;

//public class PlayerMovement : MonoBehaviour
//{
//    private Animator animator;
//    private CharacterController controller;
//    public float speed = 12f;
//    public float gravity = -9.81f * 2;
//    public float jumpHeight = 3f;

//    public Transform groundCheck;
//    public float groundDistance = 0.4f;
//    public LayerMask groundMask;

//    Vector3 velocity;
//    bool isGrounded;
//    bool isMoving;

//    private Vector3 lastPosition = Vector3.zero;

//    void Start()
//    {
//        controller = GetComponent<CharacterController>();
//        animator = GetComponent<Animator>();
//    }

//    void Update()
//    {
//        // Ground check
//        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

//        if (isGrounded && velocity.y < 0)
//        {
//            velocity.y = -2f;
//        }

//        // Input
//        float x = Input.GetAxis("Horizontal");
//        float z = Input.GetAxis("Vertical");

//        Vector3 move = transform.right * x + transform.forward * z;
//        if (move != Vector3.zero)
//        {
//            Quaternion targetRotation = Quaternion.LookRotation(move);
//            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
//        }
//        // Jump
//        if (Input.GetButtonDown("Jump") && isGrounded)
//        {
//            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
//        }

//        // Gravity
//        velocity.y += gravity * Time.deltaTime;

//        // Move character
//        Vector3 totalMove = move * speed;
//        totalMove.y = velocity.y;
//        controller.Move(totalMove * Time.deltaTime);

//        // IsMoving check
//        isMoving = Vector3.Distance(transform.position, lastPosition) > 0.001f && isGrounded;
//        lastPosition = transform.position;

//        float speedValue = move.magnitude;
//        animator.SetFloat("Speed", speedValue);
//    }
//}

using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Di chuyển")]
    public float moveSpeed = 6f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Camera & Model")]
    public Transform cameraTransform;         // Gắn MainCamera
    public Transform modelTransform;          // Gắn object là model (có Animator)

    private CharacterController controller;
    private Animator animator;

    private Vector3 velocity;
    private bool isGrounded;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = modelTransform.GetComponent<Animator>(); // Animator nằm trong model
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Lấy input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Tính hướng dựa theo camera
        Vector3 camForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = cameraTransform.right;
        Vector3 moveInput = (camForward * v + camRight * h).normalized;

        // DASH
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0f && moveInput != Vector3.zero)
        {
            isDashing = true;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;
        }

        if (isDashing)
        {
            controller.Move(moveInput * dashSpeed * Time.deltaTime);
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
            }
        }
        else
        {
            // Move thường
            controller.Move(moveInput * moveSpeed * Time.deltaTime);
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 🔁 Quay model theo hướng di chuyển (KHÔNG quay camera)
        if (moveInput != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveInput);
            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRot, Time.deltaTime * 10f);
        }

        // 🎞️ Gửi tốc độ vào Animator
        if (animator != null)
        {
            animator.SetFloat("Speed", moveInput.magnitude);
        }

        // Cập nhật cooldown dash
        dashCooldownTimer -= Time.deltaTime;
    }
}