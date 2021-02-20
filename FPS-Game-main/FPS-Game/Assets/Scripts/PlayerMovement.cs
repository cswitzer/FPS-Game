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
    public float wallJumpHorizontalForce = 5f;
    public float wallJumpVerticalForce = 5f;
    public float wallRunExitSlowDownRate;
    public float wallRunCameraRotationSpeed;
    public float wallRunCameraRotation;
    public float maxWallCameraRotation;
    public float timeElapsed = 0;
    public float waitTime = 3f;
    Vector3 orientationNormal;
    Vector3 orientationPoint;
    bool isWallRunning = false;
    bool isWallRight, isWallLeft = false;
    bool isWallJumping = false;
    bool cameraIsRotating = false;

    Vector3 velocity;
    bool isInAir = false;
    bool isGrounded;

    void Start()
    {

    }

    void Update()
    {
        GroundCheck();
        if (isGrounded)
            MovePlayer();
        else if (isInAir)
            MovePlayerInAir();
        Jumping();
        CheckForWall();
        if (isWallRunning && Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(HandleWallJump());
    }

    private void MovePlayer()
    {
        // I don't want any control of player when I am in the air
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

    private void MovePlayerInAir()
    {
        // I don't want any control of player when I am in the air
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // air movement speed is slower because this isn't Doom Eternal
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * airMovementSpeed * Time.deltaTime);

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
            isInAir = false;
            // Debug.Log("Is in the air: " + isInAir);
            velocity.y = -1f;
            // Keep the player from sliding on the horizontal axes
            velocity.x = 0;
            velocity.z = 0;
        }
        else
        {
            isInAir = true;
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
    }

    private void CheckForWall()
    {
        isWallRight = Physics.Raycast(orientation.position, orientation.right, 1f, whatIsWall);
        isWallLeft = Physics.Raycast(orientation.position, -orientation.right, 1f, whatIsWall);

        if (isWallRight || isWallLeft)
        {
            // check out what isWallRunning = true enables in the MovePlayer() and Jumping() functions
            isWallRunning = true;
            cameraIsRotating = true;
            // camera tilting is handled in here as well
            PerformWallRun();
        }
        else if (!isWallRight || !isWallLeft)
        {

            TiltCameraToOriginalPos();
            // I don't want my x direction to go on for forever, so only run the StopWallRun() once each wall run exit
            if (isWallRunning == false)
                return;
            if (isWallRunning)
                StopWallRun();
        }
    }

    private void PerformWallRun()
    {
        RaycastHit hit = new RaycastHit();

        if (isWallRight)
        {
            Physics.Raycast(transform.position, orientation.right, out hit, whatIsWall);

            // return the normal and point of the wall we hit with our ray
            orientationNormal = hit.normal.normalized;
            orientationPoint = hit.point;

            // negative must be attached to Vector3 otherwise alongWall will go in opposite direction
            Vector3 alongWall = -Vector3.Cross(hit.normal, Vector3.up);

            velocity = alongWall * wallRunningSpeed;
            controller.Move(velocity * Time.deltaTime);

            HandleWallRunRotation();
            //Debug.DrawRay(transform.position, alongWall.normalized * 10, Color.green);
        }
        else if (isWallLeft)
        {
            Physics.Raycast(transform.position, -orientation.right, out hit, whatIsWall);
            // return the normal and point of the wall we hit with our ray
            orientationNormal = hit.normal.normalized;
            orientationPoint = hit.point;

            // Get the direction the player should run in using the cross product
            Vector3 alongWall = Vector3.Cross(hit.normal, Vector3.up);

            velocity = alongWall * wallRunningSpeed;
            controller.Move(velocity * Time.deltaTime);

            HandleWallRunRotation();
            //Debug.DrawRay(transform.position, alongWall.normalized * 10, Color.green);
        }

    }

    private void StopWallRun()
    {
        velocity.x = 0;
        controller.Move(velocity * Time.deltaTime);
        isWallRunning = false; // will not allow this function to run again until after wallrunning is set to true
    }

    private void HandleWallRunRotation()
    {
        // rotate the camera every iterative call to this function...
        playerCam.transform.localRotation = Quaternion.Euler(0, 0, wallRunCameraRotation);
        ///...if the player is wallrunning on a wall to their RIGHT and they are not rotated completely 30 degrees
        if (Math.Abs(wallRunCameraRotation) < maxWallCameraRotation && isWallRunning && isWallRight)
        {
            wallRunCameraRotation += Time.deltaTime * maxWallCameraRotation * wallRunCameraRotationSpeed;
        }
        ///...if the player is wallrunning on a wall to their LEFT and they are not rotated completely 30 degrees
        else if (Math.Abs(wallRunCameraRotation) < maxWallCameraRotation && isWallRunning && isWallLeft)
        {
            wallRunCameraRotation -= Time.deltaTime * maxWallCameraRotation * wallRunCameraRotationSpeed;
        }
    }

    private void TiltCameraToOriginalPos()
    {
        // fixes the jittery camera issue since wall run run camera rotation never equals zero
        if (wallRunCameraRotation < 0.5 && wallRunCameraRotation > -0.5)
        {
            wallRunCameraRotation = 0;
        }
        // rotate the camera to original position on every iteration...
        playerCam.transform.localRotation = Quaternion.Euler(0, 0, wallRunCameraRotation);
        //... if the player is not attached to a wall, which means wallrunning is false
        if (wallRunCameraRotation > 0 && !isWallLeft && !isWallRight)
        {
            // the camera
            wallRunCameraRotation -= Time.deltaTime * maxWallCameraRotation * wallRunCameraRotationSpeed;
        }
        else if (wallRunCameraRotation < 0 && !isWallLeft && !isWallRight)
        {
            wallRunCameraRotation += Time.deltaTime * maxWallCameraRotation * wallRunCameraRotationSpeed;
        }
    }

    private IEnumerator HandleWallJump()
    {
        RaycastHit hit = new RaycastHit();

        isWallRunning = false;

        if (isWallRight)
            Physics.Raycast(transform.position, orientation.right, out hit, whatIsWall);
        else if (isWallLeft)
            Physics.Raycast(transform.position, -orientation.right, out hit, whatIsWall);

        // for jumping away from the wall
        Vector3 wallNormal = hit.normal.normalized;

        // start from the vector (velocity) that we are currently at
        Vector3 startPosition = transform.position;
        // this is the vector we want to get to from the start
        Vector3 destination = new Vector3(0,0,0);

        while (timeElapsed < waitTime)
        {
            if (isGrounded)
            {
                velocity.y = -1f;
                yield break;
            }

            velocity.y += (gravity / 2) * Time.deltaTime;
            // destination = new Vector3(transform.position.x, (transform.position.y + 50) + velocity.y, wallNormal.z * 2);
            destination = new Vector3(transform.position.x, (transform.position.y + 19f) + velocity.y, wallNormal.z * 2f);

            this.transform.position = Vector3.Lerp(startPosition, destination, (timeElapsed / waitTime));
            timeElapsed += .00375f;

            yield return null;
        }

        // make sure we get there
        transform.position = destination;
        timeElapsed = 0;
    }
}
