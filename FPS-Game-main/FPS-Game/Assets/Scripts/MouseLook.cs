using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 100f;

    public Transform playerBody;

    float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // so our cursor doesn't exit the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseRotation();
    }

    public void HandleMouseRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.fixedDeltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.fixedDeltaTime;

        // x rotation rotates player up and down
        xRotation -= mouseY;
        // do not rotate behind the player
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        // make the up and down rotation about the x-axis
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // mouse x rotates horizontally around y axis
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
