using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("movement")]
    public float moveSpeed;
    public float crouchMoveSpeed;
    private float normalMoveSpeed;
    public float sprintMoveSpeed;

    public float GroundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplayer;
    public float crouchCooldown;

    bool readyToCrouch;
    bool readyToJump;
    bool isCrouching;
    bool isSprinting;

    [Header("keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode sprintKey = KeyCode.LeftShift;



    [Header("Ground Check")]
    public float playerheight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 normalSize;
    Vector3 crouchSize;
    Vector3 moveDirection;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        
        normalMoveSpeed = moveSpeed;

        normalSize = transform.localScale;
        crouchSize = new Vector3(1f, 0.6f, 1f);
        ResetJump();

        readyToJump = true;
        readyToCrouch = true;
        isCrouching = false;
        isSprinting = false;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerheight * 0.5f + 0.2f, whatIsGround);

        MyInput();

        SpeedControl();

        // handle drag

        if (grounded)
            rb.drag = GroundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        //when to jump 
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
        //when to crouch
        if (Input.GetKey(crouchKey) && readyToCrouch && grounded)
        {
            readyToCrouch = false;

            Crouch();

            Invoke(nameof(ResetCrouch), crouchCooldown);
        }
        if (Input.GetKeyDown(sprintKey) && grounded)
        {
            Sprint();
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground 
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplayer, ForceMode.Force);

    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        //limit of velocity  if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);

        }
    }
    private void Crouch()
    {
        if (!isCrouching)
        {
            transform.localScale = crouchSize;
            moveSpeed *= crouchMoveSpeed;
            isCrouching = true;
        }
        else
        {
            transform.localScale = normalSize;
            moveSpeed = normalMoveSpeed;
            isCrouching = false;
        }
    }
    private void Jump()
    {
        //reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void Sprint()
    {
        if (!isSprinting)
        {
            moveSpeed *= sprintMoveSpeed;
            isSprinting = true;
        }
        else
        {
            moveSpeed = normalMoveSpeed;
            isSprinting = false;
        }
    }

    private void ResetCrouch()
    {
        readyToCrouch = true;
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
}