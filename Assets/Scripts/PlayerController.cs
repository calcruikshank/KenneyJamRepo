using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Queue<BufferInput> inputQueue = new Queue<BufferInput>();

    Vector2 rightStickValue = new Vector2();

    Vector3 movement;
    Vector3 inputMovement;
    Vector3 lastMoveDir;
    Vector3 moveDir;
    float maxSpeed = 15;

    float dashSpeed = 50;
    float currentDashSpeed;
    Vector3 dashDir = new Vector2();

    [SerializeField] LayerMask groundLayer;
    Collider col;
    Rigidbody rb;

    float jumpHeight = 20f;

    int numOfExtraJumps = 1;
    int currentNumOfExtraJumps = 0;

    bool groundHasNotBeenLeftAfterJumping = false;
    bool grounded;
    bool colliderIsTouchingGround;
    bool canDash = false;

    public Transform cam;


    public enum State
    {
        Normal,
        Knockback,
        Dashing
    }

    public State state;
    float bufferTimerThreshold = .2f;


    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        col = this.GetComponent<Collider>();
        state = State.Normal;
    }

    // Update is called once per frame
    void Update()
    {
        HandleBufferInput();
        CheckForGround();
        GetLastMoveDirection();

        switch (state)
        {
            case State.Normal:
                HandleMovement();
                FaceLookDirection();
                break;
            case State.Dashing:
                HandleDash();
                break;
        }
    }

    private void GetLastMoveDirection()
    {
        if (inputMovement.magnitude != 0)
        {
            lastMoveDir = new Vector3( inputMovement.x, 0, inputMovement.y);
        }
    }

    private void CheckForGround()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1.1f, groundLayer))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
        }

        if (hit.collider != null)
        {
            Debug.Log("Collider touching ground");
            colliderIsTouchingGround = true;
        }
        if (hit.collider == null)
        {
            colliderIsTouchingGround = false;
            groundHasNotBeenLeftAfterJumping = false;
            grounded = false;
        }

        if (colliderIsTouchingGround && !groundHasNotBeenLeftAfterJumping)
        {
            grounded = true;
            canDash = true;
            currentNumOfExtraJumps = 0;
        }

        if (groundHasNotBeenLeftAfterJumping)
        {
            grounded = false;
        }
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case State.Normal:
                HandleFixedMovement();
                break;
            case State.Dashing:
                FixedHandleDash();
                break;
        }
    }


    void HandleBufferInput()
    {
        if (inputQueue.Count > 0)
        {
            BufferInput currentBufferedInput = (BufferInput)inputQueue.Peek();

            if (Time.time - currentBufferedInput.timeOfInput < bufferTimerThreshold)
            {
                if (currentBufferedInput.actionType == KenneyJamData.InputActionType.JUMP)
                {
                    if (grounded)
                    {
                        Jump();
                        inputQueue.Dequeue();
                    }
                    if (!grounded && !groundHasNotBeenLeftAfterJumping && currentNumOfExtraJumps < numOfExtraJumps)
                    {
                        currentNumOfExtraJumps++;
                        rb.velocity = new Vector3(moveDir.x * maxSpeed * movement.magnitude, rb.velocity.y, moveDir.z * maxSpeed * movement.magnitude);//todo
                        Jump();
                        inputQueue.Dequeue();
                    }
                }
                if (currentBufferedInput.actionType == KenneyJamData.InputActionType.DASH)
                {
                    if (state == State.Normal)
                    {
                        if (canDash)
                        {
                            float targetAngleDash = Mathf.Atan2(currentBufferedInput.directionOfAction.x, currentBufferedInput.directionOfAction.y) * Mathf.Rad2Deg + cam.eulerAngles.y;
                            Vector3 moveDirDash = Quaternion.Euler(0, targetAngleDash, 0) * Vector3.forward;
                            Dash(moveDirDash);
                            inputQueue.Dequeue();
                        }
                    }
                }
            }
            if (Time.time - currentBufferedInput.timeOfInput >= bufferTimerThreshold)
            {
                inputQueue.Dequeue();
            }
        }
    }

    private void Dash(Vector2 directionOfDash)
    {
        transform.rotation = Quaternion.Euler(0f, targetAngle, 0);
        dashDir = directionOfDash;
        currentDashSpeed = dashSpeed;
        SetStateToDashing();
        canDash = false;
    }
    private void HandleDash()
    {
        float powerDashSpeedMulti = 2f;
        currentDashSpeed -= currentDashSpeed * powerDashSpeedMulti * Time.deltaTime;

        if (currentDashSpeed < 20f) { 
            SetStateToNormal();
        }
    }
    private void FixedHandleDash()
    {
        rb.velocity = new Vector3(transform.forward.x * currentDashSpeed, 0, transform.forward.z * currentDashSpeed);
    }


    private void Jump()
    {
        groundHasNotBeenLeftAfterJumping = true;
        grounded = false;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector2.up * jumpHeight, ForceMode.Impulse);
        SetStateToNormal();
    }

    private void HandleMovement()
    {
        movement.x = inputMovement.x;
        movement.y = 0;
        movement.z = inputMovement.y;
        moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
    }
    private void HandleFixedMovement()
    {
        //instead of using move towards i want to keep the momentum of the rb if the current velocity is higher than the input velocity this way it will only ever add force in the direction of input
        if (grounded)
        {
            rb.velocity = Vector3.MoveTowards(rb.velocity, new Vector3(moveDir.x * maxSpeed * movement.magnitude, rb.velocity.y, moveDir.z * maxSpeed * movement.magnitude), 3);
        }

        if (!grounded)
        {
            if (rb.velocity.x < inputMovement.x * maxSpeed && inputMovement.x > 0)
            {
                rb.velocity = Vector3.MoveTowards(rb.velocity, new Vector3(moveDir.x * maxSpeed * movement.magnitude, rb.velocity.y, moveDir.z * maxSpeed * movement.magnitude * maxSpeed), .5f);
            }
            if (rb.velocity.x > inputMovement.x * maxSpeed && inputMovement.x < 0)
            {
                rb.velocity = Vector3.MoveTowards(rb.velocity, new Vector3(moveDir.x * maxSpeed * movement.magnitude, rb.velocity.y, moveDir.z * maxSpeed * movement.magnitude), .5f);
            }
        }
    }
    float targetAngle;
    void FaceLookDirection()
    {
        targetAngle = Mathf.Atan2(lastMoveDir.x, lastMoveDir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;


        if (movement.magnitude > 0)
        {
            transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.Euler(0f, targetAngle, 0), 1000f * Time.deltaTime);
        }
    }


    void OnLook(InputValue inputValue)
    {
        rightStickValue = inputValue.Get<Vector2>();
    }

    void OnMove(InputValue inputValue)
    {
        inputMovement = inputValue.Get<Vector2>();
    }

    void OnJump()
    {
        BufferInput jumpBuffer = new BufferInput(KenneyJamData.InputActionType.JUMP, inputMovement, Time.time);
        inputQueue.Enqueue(jumpBuffer);
    }

    void OnDash()
    {
        BufferInput dashBuffer = new BufferInput(KenneyJamData.InputActionType.DASH, lastMoveDir.normalized, Time.time);
        inputQueue.Enqueue(dashBuffer);
    }

    private void SetStateToDashing()
    {
        state = State.Dashing;
    }
    private void SetStateToNormal()
    {
        state = State.Normal;
    }
}
