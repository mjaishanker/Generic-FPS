using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float playerHeight = 2f;

    [SerializeField] Transform orientation;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float airMultiplier = 0.4f;
    float movementMultiplier = 10f;

    [Header("Sprinting")]
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float sprintSpeed = 6f;
    [SerializeField] float acceleration = 10f;

    [Header("Jumping")]
    public float jumpForce = 5f;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Drag")]
    [SerializeField] float groundDrag = 6f;
    [SerializeField] float airDrag = 2f;

    float horizontalMovement;
    float verticalMovement;

    [Header("Ground Detection")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask orbitMask;
    [SerializeField] float groundDistance = 0.2f;
    public bool isGrounded { get; private set; }

    Vector3 moveDirection;
    Vector3 slopeMoveDirection;

    Rigidbody rb;

    RaycastHit slopeHit;

    [SerializeField] public Transform onPlanet;

    [Header("Grapple Variables")]
    [SerializeField] private Transform debugHitPointTransform;
    [SerializeField] Transform hookShotStartTransform;
    [SerializeField] private float grappleForce;
    private LineRenderer grappleLine;
    public State state;
    private Vector3 hookshotPosition;
    private Vector3 hookShotDir;
    private float timeCount = 0.0f;
    bool isGrappling;
    [SerializeField] Transform playerCamera;


    public enum State
    {
        Normal,
        HookshotFlyingPlayer,
        WallRunning
    }

    private bool OnSlope()
    {
        Vector3 planetVectorUp = Vector3.ProjectOnPlane(onPlanet.transform.position - (transform.position), -onPlanet.transform.up);
        if (Physics.Raycast(transform.localPosition, -planetVectorUp/*Vector3.down*/, out slopeHit, playerHeight / 2 + 0.5f))
        {
            //Debug.DrawRay(slopeHit.transform.position, slopeHit.normal * 100f, Color.black);

            //Vector3 planetVectorUp = Vector3.ProjectOnPlane(onPlanet.transform.position - (transform.position), -onPlanet.transform.up);
            //Debug.DrawRay(transform.position, planetVectorUp * 100f, Color.magenta);
            //Debug.DrawRay(transform.localPosition, Vector3.up * 100f, Color.magenta);

            // Instead of Vector3.up it should be planet's transform.up, to find the slope
            if (slopeHit.normal != planetVectorUp)//(slopeHit.normal != planetVectorUp)//(slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        grappleLine = GetComponent<LineRenderer>();
        grappleLine.positionCount = 2;
    }

    private void Update()
    {
        giveState();

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //if (isGrounded)
        //    state = State.Normal;

        //Debug.Log("Check State: " + state);

        MyInput();
        ControlDrag();
        ControlSpeed();
        HandleHookshotStart();
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.transform.forward * verticalMovement + orientation.transform.right * horizontalMovement;
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    void ControlSpeed()
    {
        if (Input.GetKey(sprintKey) && isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }
    }

    void ControlDrag()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
        HandleHookshotStart();
        Vector3 hookshotDir = (hookshotPosition - transform.position).normalized;

        //Debug.DrawRay(transform.position, hookshotDir * 100f, Color.red);
        if (state == State.HookshotFlyingPlayer)
        {
            //Debug.Log("Rotation");
            grappleLine.enabled = true;
            grappleLine.SetPosition(0, new Vector3(debugHitPointTransform.position.x, debugHitPointTransform.position.y, debugHitPointTransform.position.z));
            grappleLine.SetPosition(1, new Vector3(hookShotStartTransform.position.x, hookShotStartTransform.position.y, hookShotStartTransform.position.z));
            GrapplePlayer();

            //transform.rotation = Quaternion.Slerp(transform.localRotation, debugHitPointTransform.localRotation, timeCount);
            //timeCount = timeCount + Time.fixedDeltaTime / 2;

        }
    }

    void MovePlayer()
    {
        if (isGrounded /*&& !OnSlope()*/)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }
    }

    private void HandleHookshotStart()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Check State: " + state);
            if (state != State.HookshotFlyingPlayer)
            {
                Debug.Log("Hookshot Button pressed");

                if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit raycastHit))
                {
                    Debug.Log("Raycast hit");
                    // Hit Something
                    //if(raycastHit.distance < 100)
                    //{
                    debugHitPointTransform.position = raycastHit.point;
                    hookshotPosition = raycastHit.point;

                    Vector3 hookShotDist = hookshotPosition - transform.position;
                    hookShotDir = (hookshotPosition - transform.position).normalized;

                    state = State.HookshotFlyingPlayer;
                    isGrappling = true;
                    //rb.AddForce(hookShotDir * 50f, ForceMode.Impulse);

                    Debug.Log("State After Grapple: " + state);

                    

                    //transform.rotation = Quaternion.FromToRotation(transform.up, hookshotPosition) * transform.rotation;
                    //Debug.DrawRay(transform.position, hookshotDirNormalized * 100f, Color.magenta);

                    //}
                }
            }
            
        }
    }

    private void GrapplePlayer()
    {
        if (isGrappling)
        {
            Vector3 movePosition = Vector3.Slerp(transform.position, hookshotPosition, 1f * Time.fixedDeltaTime);
            //rb.MovePosition(movePosition);
            rb.AddForce(hookShotDir * 4f, ForceMode.Impulse);
            isGrappling = Vector3.Distance(transform.position, hookshotPosition) <= 1 ? false : true;
            if(Vector3.Distance(transform.position, hookshotPosition) <= 1)
            {
                state = State.Normal;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Halo" || collision.transform.tag == "Planet")
        {
            //Debug.Log("Entered Collision");
            isGrounded = true;
            state = State.Normal;
            grappleLine.enabled = false;
        }
    }

    void giveState()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("State: " + state);
        }
    }

}
