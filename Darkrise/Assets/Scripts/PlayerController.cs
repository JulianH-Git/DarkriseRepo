using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb; // player rigid body

    [Header("Horizontal Movement Settings")]
    [SerializeField] private float walkSpeed;
    private float xAxis;

    [Header("Ground Check Settings")]

    [SerializeField] private float jumpForce;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckX;
    [SerializeField] private float groundCheckY;
    [SerializeField] private LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Jump();

        // debugging code for collision detection stuff
        Debug.DrawRay(groundCheckPoint.position, Vector2.down * groundCheckY, Color.red);
        Debug.DrawRay(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down * groundCheckY, Color.green);
        Debug.DrawRay(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down * groundCheckY, Color.blue);
    }

    void GetInput()
    {
        xAxis = Input.GetAxisRaw("Horizontal"); //default left/right keys are the arrow keys or A and D
    }

    void Move()
    {
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
    }

    public bool Grounded()
    {
        /*
         * variables in order of appearance
         * 1) origin of the raycast
         * 2) the direction the raycast is going
         * 3) how far the ray will travel
         * 4) and the layer that the raycast is trying to detect
         * 
         * the other two checks add and subtract the groundCheckX variable to make sure that this works when the player is on the edge of a platform as well.
         */

        

        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, groundLayer)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, groundLayer)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, groundLayer)) 
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Jump()
    {
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0) // jump button is spacebar
        {
            rb.velocity = new Vector3(rb.velocity.x, 0);
        }


        if (Input.GetButtonDown("Jump") && Grounded()) // jump button is spacebar
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
        }
    }
}
