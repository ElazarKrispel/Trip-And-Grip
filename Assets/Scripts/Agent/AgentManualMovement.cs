using UnityEngine;

public class AgentMovementController : MonoBehaviour
{
    public float moveSpeed = 5f; // Forward movement speed
    public float rotationSpeed = 200f; // Rotation speed
    public float jumpForce = 7f; // Jump force

    private Rigidbody rb;
    private Animator animator;
    private bool isJumping = false;
    private bool isRolling = false;
    private bool isTouchingSurface = true;
    private int jumpCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        // Forward movement
        if (Input.GetKey(KeyCode.W))
        {
            rb.MovePosition(rb.position + transform.forward * moveSpeed * Time.deltaTime);
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        // Rotation in place
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.Rotate(Vector3.up, 180f); // 180-degree rotation
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && (isTouchingSurface || jumpCount < 2))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetBool("isJumping", true);
            isTouchingSurface = false;
            jumpCount++;
        }

        // Roll
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isRolling && !isJumping)
        {
            isRolling = true;
            animator.SetBool("isRolling", true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRolling = false;
            animator.SetBool("isRolling", false);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        // Detect when the agent touches a surface
        foreach (ContactPoint contact in collision.contacts) // Iterate through all contact points with the collided object
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f) // Check if the contact normal is close to upwards, indicating contact with the ground and not a wall
            {
                isTouchingSurface = true;
                jumpCount = 0;
                animator.SetBool("isJumping", false);
                break;
            }
        }
        {
            isTouchingSurface = true;
            jumpCount = 0;
            animator.SetBool("isJumping", false);
        }
    }
}
