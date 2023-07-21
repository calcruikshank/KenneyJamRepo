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
        FaceLookDirection();

        switch (state)
        {
            case State.Normal:
                HandleMovement();
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
        grounded = true;
        return;
        var groundedRay = Physics2D.Raycast(transform.position, Vector2.down, 0.1f + col.bounds.size.y / 2, groundLayer);

        if (groundedRay.collider != null)
        {
            colliderIsTouchingGround = true;
        }
        if (groundedRay.collider == null)
        {
            colliderIsTouchingGround = false;
            groundHasNotBeenLeftAfterJumping = false;
            grounded = false;
        }

        if (colliderIsTouchingGround && !groundHasNotBeenLeftAfterJumping)
        {
            grounded = true;
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
                        rb.velocity = new Vector2(movement.x * maxSpeed, 0);
                        Jump();
                        inputQueue.Dequeue();
                    }
                }
                if (currentBufferedInput.actionType == KenneyJamData.InputActionType.DASH)
                {
                    if (state == State.Normal)
                    {
                        if (grounded)
                        {
                            Dash(new Vector2(currentBufferedInput.directionOfAction.x, 0));
                        }
                        else
                        {
                            Dash(currentBufferedInput.directionOfAction);
                        }
                        inputQueue.Dequeue();
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
        dashDir = directionOfDash.normalized;
        currentDashSpeed = dashSpeed;
        SetStateToDashing();
    }
    private void HandleDash()
    {
        float powerDashSpeedMulti = 6f;
        currentDashSpeed -= currentDashSpeed * powerDashSpeedMulti * Time.deltaTime;

        if (currentDashSpeed < 5f)
        {
            SetStateToNormal();
        }
    }
    private void FixedHandleDash()
    {
        rb.velocity = new Vector3(dashDir.x * currentDashSpeed, dashDir.y * currentDashSpeed);
    }


    private void Jump()
    {
        groundHasNotBeenLeftAfterJumping = true;
        grounded = false;
        rb.velocity = new Vector2(rb.velocity.x, 0);
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
                rb.velocity = Vector3.MoveTowards(rb.velocity, new Vector3(movement.x * maxSpeed, rb.velocity.y, movement.z * maxSpeed), 2);
            }
            if (rb.velocity.x > inputMovement.x * maxSpeed && inputMovement.x < 0)
            {
                rb.velocity = Vector3.MoveTowards(rb.velocity, new Vector3(movement.x * maxSpeed, rb.velocity.y, movement.z * maxSpeed), 2);
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
