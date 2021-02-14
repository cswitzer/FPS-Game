using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    [Header("Movement")]
    public float movementSpeed = 10f;
    public float airMovementSpeed = 10f;
    public float maxSpeed = 10f;

    [Header("Ground Detection")]
    public LayerMask whatIsGround;
    public Transform groundCheck;
    public float groundDistance = 0.4f;

    [Header("Jumping")]
    public float jumpForce = 10f;
    public float jumpCoolDown = .25f;
    public float gravity = -20f;

    [Header("WallRunning")]
    public Transform playerCam;
    public Transform orientation;
    public LayerMask whatIsWall;
    public float wallRunningSpeed;
    public float wallRunExitSlowDownRate;
    Vector3 orientationNormal;
    Vector3 orientationPoint;
    bool isWallRunning = false;
    bool isWallRight, isWallLeft = false;

    Vector3 velocity;
    bool isGrounded;

    void Update()
    {
        MovePlayer();
        GroundCheck();
        Jumping();
        CheckForWall();
    }

    private void MovePlayer()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * movementSpeed * Time.deltaTime);

        // If not wallrunning, apply normal gravity. If wall running, change gravity to be weightless
        if (isWallRunning == false)
        {
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
        else
        {
            velocity.y += (gravity + 15f) * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsGround);
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -1f;
        }
    }

    private void Jumping()
    {
        if (isWallRunning == false)
        {
            if (isGrounded && Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            }
        }
        else if (isWallRunning == true && Input.GetButtonDown("Jump"))
        {
            // add logic here for jumping off the wall in the correct direction
            Debug.Log("I am jumping off a wall");
        }
    }
    
    private void CheckForWall()
    {
        isWallRight = Physics.Raycast(orientation.position, orientation.right, 1f, whatIsWall);
        isWallLeft = Physics.Raycast(orientation.position, -orientation.right, 1f, whatIsWall);

        if (isWallRight || isWallLeft)
        {
            // check out what isWallRunning = true enables in the MovePlayer() and Jumping() functions
            isWallRunning = true;
            PerformWallRun();
        }
        else if (!isWallRight || !isWallLeft)
        {
            // I don't want my x direction to be 0 forever, so only run the StopWallRun() once each wall run exit
            if (isWallRunning == false)
                return;
            if (isWallRunning)
                StopWallRun();
        }
    }

    private void PerformWallRun()
    {
        RaycastHit hit = new RaycastHit();

        if (isWallRight && Input.GetKey(KeyCode.D))
        {
            Physics.Raycast(transform.position, orientation.right, out hit, whatIsWall);
            // return the normal and point of the wall we hit with our ray
            orientationNormal = hit.normal;
            orientationPoint = hit.point;

            // Get the direction the player should run in using the cross product
            // negative must be attached to Vector3 otherwise alongWall will go in opposite direction
            Vector3 alongWall = -Vector3.Cross(hit.normal, Vector3.up);
            
            // Debug.DrawRay(transform.position, alongWall.normalized * 10, Color.green);

            velocity = alongWall * wallRunningSpeed;
            controller.Move(velocity * Time.deltaTime);
        }
        else if (isWallLeft && Input.GetKey(KeyCode.A))
        {
            Physics.Raycast(transform.position, -orientation.right, out hit, whatIsWall);
            // return the normal and point of the wall we hit with our ray
            orientationNormal = hit.normal;
            orientationPoint = hit.point;
  
            // Get the direction the player should run in using the cross product
            Vector3 alongWall = Vector3.Cross(hit.normal, Vector3.up);
            
            // Debug.DrawRay(transform.position, alongWall.normalized * 10, Color.green);

            velocity = alongWall * wallRunningSpeed;
            controller.Move(velocity * Time.deltaTime);
        }
        
    }

    private void StopWallRun()
    {
        velocity.x = 0;
        controller.Move(velocity * Time.deltaTime);
        isWallRunning = false; // will not allow this function to run again until after wallrunning is set to true
    }
}
