using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private CharacterController controller;
    public float speed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;
    bool isMoving;

    private Vector3 lastPosition = Vector3.zero;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
        }
        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;

        // Move character
        Vector3 totalMove = move * speed;
        totalMove.y = velocity.y;
        controller.Move(totalMove * Time.deltaTime);

        // IsMoving check
        isMoving = Vector3.Distance(transform.position, lastPosition) > 0.001f && isGrounded;
        lastPosition = transform.position;

        float speedValue = move.magnitude;
        animator.SetFloat("Speed", speedValue);
    }
}