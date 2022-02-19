using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] Transform orientation;

    [Header("Detection")]
    [SerializeField] float wallDistance = 0.5f;
    [SerializeField] float minimumJumpHeight = 1.5f;

    [Header("Wall Running")]
    [SerializeField] private float wallRunGravity;
    [SerializeField] private float wallRunJumpForce;

    [Header("Camera")]
    [SerializeField] private Camera cam;
    [SerializeField] private float fov;
    [SerializeField] private float wallRunFov;
    [SerializeField] private float wallRunFovTime;
    [SerializeField] private float camTilt;
    [SerializeField] private float camTiltTime;

    public float tilt { get; private set; }


    bool wallLeft = false;
    bool wallRight = false;

    private Rigidbody rb;
    private GravityBody gb;

    RaycastHit leftWallHit;
    RaycastHit rightWallHit;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gb = GetComponent<GravityBody>();
    }

    bool CanWallRun()
    {
        //Debug.DrawRay(transform.position, Vector3.down, Color.white);
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);
        
    }

    void CheckWall()
    {
        //Debug.DrawRay(transform.position, -orientation.right, Color.blue);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallDistance);
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallDistance);
    }

    // Update is called once per frame
    void Update()
    {
        CheckWall();

        if (CanWallRun())
        {
            if (wallLeft)
            {
                BeginWallRun();
            }

            else if (wallRight)
            {
                BeginWallRun();
            }
            else
            {
                EndWallRun();
            }
        }
        else
        {
            //EndWallRun();
        }
    }

    void BeginWallRun()
    {
        //Debug.Log("Wall Before Running State: " + GetComponent<PlayerMovement>().state);
        //GetComponent<PlayerMovement>().state = PlayerMovement.State.WallRunning;
        gb.enabled = false;
        //Debug.Log("Wall After Running State: " + GetComponent<PlayerMovement>().state);

        rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, wallRunFov, wallRunFovTime * Time.deltaTime);

        if (wallLeft)
            tilt = Mathf.Lerp(tilt, -camTilt, camTiltTime * Time.deltaTime);
        else if(wallRight)
            tilt = Mathf.Lerp(tilt, camTilt, camTiltTime * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (wallLeft)
            {
                Vector3 wallRunJumpDirection = transform.up + leftWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
            }
            else if (wallRight)
            {
                Vector3 wallRunJumpDirection = transform.up + rightWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
            }
        }

    }

    void EndWallRun()
    {
        gb.enabled = true;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, wallRunFovTime * Time.deltaTime);
        tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);
        //GetComponent<PlayerMovement>().state = PlayerMovement.State.Normal;
        //Debug.Log("Exit Wall Running");
    }

}
