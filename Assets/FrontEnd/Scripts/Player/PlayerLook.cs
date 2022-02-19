using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] WallRun wallRun;

    [SerializeField] private float sensX = 100f;
    [SerializeField] private float sensY = 100f;

    [SerializeField] Transform cam = null;
    [SerializeField] Transform orientation = null;

    float mouseX;
    float mouseY;

    float multiplier = 0.01f;

    float xRotation;
    float yRotation;

    float verticalLookRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -60f, 60f);

        verticalLookRotation += Input.GetAxis("Mouse Y") * Time.deltaTime * sensY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90, 90);
        cam.localEulerAngles = -Vector3.right * verticalLookRotation;
        cam.transform.localRotation = Quaternion.Euler(cam.localEulerAngles.x, cam.localEulerAngles.y, wallRun.tilt);
        //transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * sensX);
        //cam.transform.rotation = Quaternion.Euler(xRotation, 0, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, yRotation, 0);
    }
}
