using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Rörelseinställningar")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Komponenter")]
    private Rigidbody rb;
    private SpriteRenderer spriteRenderer;
    private float verticalInput; 

    // Variabler för att hålla koll på tillstånd
    private float horizontalInput;
    private bool isGrounded;

    void Start()
    {
        // Hämta komponenterna när spelet startar
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

   

    void Update()
    {
        // Läs in både höger/vänster (A/D) och upp/ner (W/S)
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical"); // Ny rad för W/S

        FlipSprite();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        // ändrar nu både X (höger/vänster) och Z (in/ut i rummet)
        // Y behålls som den är för att gravitationen ska funka
        Vector3 newVelocity = new Vector3(horizontalInput * moveSpeed, rb.linearVelocity.y, verticalInput * moveSpeed);
        rb.linearVelocity = newVelocity;
    }

    void Move()
    {
        // Vi behåller Y-hastigheten (för gravitation) men ändrar X
        Vector3 newVelocity = new Vector3(horizontalInput * moveSpeed, rb.linearVelocity.y, 0f);
        rb.linearVelocity = newVelocity;
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false; // Förhindra dubbelhopp direkt
    }

    void FlipSprite()
    {
        if (horizontalInput > 0.01f)
            spriteRenderer.flipX = false;
        else if (horizontalInput < -0.01f)
            spriteRenderer.flipX = true;
    }

    // Enkel kollisionskoll för att se om vi står på marken
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}