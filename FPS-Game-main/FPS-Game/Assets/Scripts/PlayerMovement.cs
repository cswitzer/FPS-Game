using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] float moveSpeed = 6;
    [SerializeField] float jumpForce = 10f;

    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask wallMask;
    [SerializeField] Transform orientation;
    [SerializeField] float groundDistance = 0.4f;
    bool isWallRight, isWallLeft;
    bool isWallRunning = false;

    Vector3 velocity;
    bool isGrounded = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfGrounded();

        // Input
        float x = Input.GetAxisRaw("Horizontal") * moveSpeed;
        float z = Input.GetAxisRaw("Vertical") * moveSpeed;

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }

        // moving
        Vector3 movePos = transform.right * x + transform.forward * z;
        Vector3 newMovePos = new Vector3(movePos.x, rb.velocity.y, movePos.z);
        rb.velocity = newMovePos;

    }

    
    private void CheckIfGrounded()
    {
        isGrounded = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z), /*radius*/ 1.67f, groundMask);
        Debug.Log(isGrounded);
        if (isGrounded && velocity.y < 0)
        {
            isWallRunning = false;
        }
    }
}
