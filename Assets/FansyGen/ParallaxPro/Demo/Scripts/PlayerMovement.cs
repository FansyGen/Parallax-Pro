using UnityEngine;

namespace FansyGen.Demo
{
    public class PlayerMovement : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float runSpeedMultiplier = 2f;
        public float jumpForce = 5f;
        public Transform groundCheck;
        public float groundCheckDistance = 0.2f;

        private Rigidbody2D rb;
        private bool isGrounded;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            // Check if the player is grounded
            isGrounded = IsGrounded();

            // Get horizontal input for movement
            float moveHorizontal = Input.GetAxis("Horizontal");

            // Apply movement velocity
            rb.velocity = new Vector2(moveHorizontal * moveSpeed, rb.velocity.y);

            // Flip the player sprite based on movement direction
            if (moveHorizontal > 0)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
            else if (moveHorizontal < 0)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }

            // Run if the "Run" input is pressed
            if (Input.GetButton("Fire1"))
            {
                rb.velocity = new Vector2(moveHorizontal * moveSpeed * runSpeedMultiplier, rb.velocity.y);
            }

            // Jump if the "Jump" input is pressed and the player is grounded
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            }
        }

        private bool IsGrounded()
        {
            // Cast a ray below the player to check for ground collision
            RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance);

            // Check if the ray hits any collider
            if (hit.collider != null)
            {
                // Adjust the ground check distance based on the collider's thickness
                float thickness = hit.collider.bounds.size.y;
                groundCheckDistance = thickness * 0.5f + 0.01f;
                return true;
            }

            return false;
        }
    }
}