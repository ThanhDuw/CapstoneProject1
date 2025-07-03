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

    [Header("Combat")]
    public GameObject staff;                  // Gậy (hiện/ẩn)
    public float staffHideDelay = 5f;         // 5s không dùng thì ẩn
    public float comboMaxDelay = 0f;          // 0s để nối combo
    private int comboStep = 0;                // 1 → 2 → 3
    private float lastComboTime = -999f;      // Thời điểm nhấn gần nhất
    private float staffTimer = 0f;            // Đếm ngược để ẩn gậy

    [Header("VFX")]
    public GameObject meleeHitEffect; // Prefab VFX
    public Transform vfxSpawnPoint;   // Gắn điểm đầu gậy (vị trí spawn)

    private CharacterController controller;
    private Animator animator;

    private Vector3 velocity;
    private bool isGrounded;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;

    public float sprintBonus = 5f; // Tăng thêm khi giữ Q

    [SerializeField] private Transform firePoint; // Gắn điểm bắn (nòng súng) trong Inspector

    [Header("Hiệu ứng")]
    public GameObject hitEffectPrefab; // Prefab hiệu ứng trúng đạn (tùy chọn)
    public Animator gunAnimator; // Gắn animator từ model/súng



    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = modelTransform.GetComponent<Animator>(); // Animator nằm trong model
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0)) // Chuột trái
        {
            Shoot();
        }
        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (animator != null)
                animator.SetBool("isJumping", true); // Bắt đầu Jump
        }
     

        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;

            if (animator != null)
                animator.SetBool("isJumping", false); // Kết thúc Jump khi chạm đất
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

            if (animator != null)
                animator.SetBool("isDashing", true); // Bắt đầu Dash
        }

        if (isDashing)
        {
            controller.Move(moveInput * dashSpeed * Time.deltaTime);
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                if (animator != null)
                    animator.SetBool("isDashing", false); // Kết thúc Dash
            }
        }
        else
        {
            // Chạy nhanh khi đè Q + có hướng di chuyển
            bool isSprinting = Input.GetKey(KeyCode.Q) && moveInput.magnitude > 0f;
            float currentSpeed = isSprinting ? moveSpeed + sprintBonus : moveSpeed;

            controller.Move(moveInput * currentSpeed * Time.deltaTime);
            //// Move thường
            //controller.Move(moveInput * moveSpeed * Time.deltaTime);
        }

        

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        //  Quay model theo hướng di chuyển (KHÔNG quay camera)
        if (moveInput != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveInput);
            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRot, Time.deltaTime * 10f);
        }

        // Gửi tốc độ  Animator
        if (animator != null)
        {
            animator.SetFloat("Speed", moveInput.magnitude);
            animator.SetBool("isSprinting", Input.GetKey(KeyCode.Q) && moveInput.magnitude > 0f);
        }

        // Cập nhật cooldown dash
        dashCooldownTimer -= Time.deltaTime;

        HandleComboAttack();
    }

    void Shoot()
    {
        // Kích hoạt animation bắn
        if (gunAnimator != null)
        {
            gunAnimator.SetTrigger("Fire");
        }

        if (firePoint == null)
        {
            Debug.LogWarning("FirePoint is not assigned!");
            return;
        }

        Ray ray = new Ray(firePoint.position, firePoint.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Debug.Log($"Raycast hit: {hit.collider.name}");
        }
        else
        {
            Debug.Log("Raycast missed");
        }
    }
    void HandleComboAttack()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // Hiện gậy nếu đang ẩn
            if (staff != null && !staff.activeSelf)
                staff.SetActive(true);

            comboStep++;
            if (comboStep > 3)
                comboStep = 1;

            staffTimer = staffHideDelay;

            if (animator != null)
            {
                animator.SetInteger("attackIndex", comboStep);
                animator.SetTrigger("Attack");
            }

            Debug.Log("Combo Step: " + comboStep);
        }

        // Ẩn gậy sau thời gian
        if (staff != null && staff.activeSelf)
        {
            staffTimer -= Time.deltaTime;
            if (staffTimer <= 0f)
                staff.SetActive(false);
        }
    }

    public void MeleeAttackEnd()
    {
        if (meleeHitEffect != null && vfxSpawnPoint != null)
        {
            Instantiate(meleeHitEffect, vfxSpawnPoint.position, vfxSpawnPoint.rotation);
            Debug.Log("Spawn VFX at: " + vfxSpawnPoint.position);
        }
        else
        {
            Debug.LogWarning("VFX hoặc Spawn Point chưa được gán.");
        }

        comboStep = 0;

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetInteger("attackIndex", 0);
            animator.CrossFade("Idle", 0.01f); // Ép về Idle
        }
    }
}