using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class PlayerController : NetworkBehaviour
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
    public bool jumpedOutOfDash = false;

    public bool isOnWall = false;

    public Transform cam;

     float timerForDashing;
     float timerForDashingThreshold = .5f;

    public Animator playerAnim;

    public enum State
    {
        Normal,
        Knockback,
        Dashing,
        Diving
    }

    public State state;
    float bufferTimerThreshold = .2f;

    public ParticleSystem groundParticles;
    public ParticleSystem jumpParticles;



    float timerThresholdForFootstepMax = .15f;
    public float footstepAudioTimer = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner)
        {
            return;
        }
        cam = Camera.main.transform;



        FindObjectOfType<CinemachineFreeLook>().LookAt = this.transform;
        FindObjectOfType<CinemachineFreeLook>().Follow = this.transform;
        FindObjectOfType<FollowTarget>().target = this.transform;
        rb = this.GetComponent<Rigidbody>();
        col = this.GetComponent<Collider>();
        state = State.Normal;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        HandleBufferInput();
        CheckForGround();
        CheckForWall();
        GetLastMoveDirection();
        HandleTimers();
        CheckForLowDeath();

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

    private void CheckForLowDeath()
    {
        if (transform.position.y < -100)
        {
            transform.position = Vector3.zero;
            rb.velocity = Vector3.zero;
            SetStateToNormal();
        }
    }

    private void HandleTimers()
    {
        timerForDashing += Time.deltaTime;
    }

    private void GetLastMoveDirection()
    {
        if (inputMovement.magnitude != 0)
        {
            lastMoveDir = new Vector3( inputMovement.x, 0, inputMovement.y);
        }
    }

    float coyoteTimerThreshold = .2f;
    float coyoteTimer = 0f;
    private void CheckForGround()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.BoxCast(transform.position, transform.GetComponent<Collider>().bounds.size / 3f, transform.TransformDirection(Vector3.down), out hit, Quaternion.identity, 1.25f, groundLayer))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

            colliderIsTouchingGround = CheckColliderGround(hit);
            if (!colliderIsTouchingGround)
            {
                groundHasNotBeenLeftAfterJumping = false;
            }

        }
        else
        {
            if (hit.collider == null)
            {
                if (colliderIsTouchingGround)
                {
                    coyoteTimer = 0;
                    colliderIsTouchingGround = false;
                }

                coyoteTimer += Time.deltaTime;

                if (coyoteTimer >= coyoteTimerThreshold && grounded)
                {
                    grounded = false;
                }
                groundHasNotBeenLeftAfterJumping = false;

                
            }
        }

        if (colliderIsTouchingGround && !groundHasNotBeenLeftAfterJumping)
        {
            if (grounded == false)
            {
                if (AudioManager.singleton != null )
                {
                    AudioManager.singleton.PlayRandomImpact();
                }
            }
            grounded = true;
            if (timerForDashing >= timerForDashingThreshold)
            {
                canDash = true;
            }
            jumpedOutOfDash = false;

            currentNumOfExtraJumps = 0;
        }

        if (groundHasNotBeenLeftAfterJumping)
        {
            grounded = false;
        }

        var emission = groundParticles.emission;
        emission.enabled = (grounded || isOnWall);
        playerAnim.SetBool("airborne", !grounded);
        if (grounded)
        {
            playerAnim.SetBool("falling", false);
        }
    }

    private void CheckForWall()
    {
        RaycastHit hit;


        Collider[] cols = Physics.OverlapSphere(transform.position, 1.5f, groundLayer);
        if (cols.Length > 0)
        {
            foreach(Collider col in cols)
            {
                if(col != null)
                {
                    if(Physics.Raycast(transform.position,col.transform.position - transform.position, out hit, 5f, groundLayer))
                    {
                        Debug.DrawRay(transform.localPosition, hit.normal * 20f, Color.red);

                        if (!grounded && rb.velocity.y < -5f && CheckColliderWall(hit))
                        {
                            Debug.Log("Wall Checker Hit!");
                            isOnWall = true;

                            var lookDir = Quaternion.LookRotation(hit.normal.normalized, Vector3.up);
                            transform.localRotation = lookDir;

                        }
                        else
                        {
                            isOnWall = false;
                        }
                    }
                }
            }
        }
        else
        {
            isOnWall = false;
        }

        playerAnim.SetBool("wallslide", isOnWall);

        if (isOnWall)
        {
            rb.drag = 4f;
        }
        else
        {
            rb.drag = 0f;
        }
    }

    private bool CheckColliderWall(RaycastHit col)
    {
        Debug.Log($"{(Vector3.Dot(col.normal.normalized, Vector3.up))} returning {(Vector3.Dot(col.normal.normalized, Vector3.up) <= 0.8f && Vector3.Dot(col.normal.normalized, Vector3.up) >= -0.8f)}");
        return (Vector3.Dot(col.normal.normalized, Vector3.up) <= 0.65f && Vector3.Dot(col.normal.normalized, Vector3.up) >= -0.65f);

    }

    private bool CheckColliderGround(RaycastHit col)
    {
        //Debug.Log($"{Vector3.Dot(col.normal.normalized, Vector3.up)} bool result: {Vector3.Dot(col.normal.normalized, Vector3.up) >= 0.90f}");
        return (Vector3.Dot(col.normal.normalized, Vector3.up) >= 0.65f);

    }

    void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }
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
                        canDash = true;
                        inputQueue.Dequeue();
                    }
                    else
                    {
                        if(isOnWall)
                        {
                            rb.velocity = new Vector3(transform.forward.x * maxSpeed, 0f, transform.forward.z * maxSpeed);
                            Jump();
                            inputQueue.Dequeue();
                        }
                        else
                        {
                            //double jump
                            if (!groundHasNotBeenLeftAfterJumping && currentNumOfExtraJumps < numOfExtraJumps)
                            {
                                currentNumOfExtraJumps++;
                                if (movement.magnitude > 0)
                                {
                                    rb.velocity = new Vector3(moveDir.x * maxSpeed, rb.velocity.y, moveDir.z * maxSpeed);
                                }
                                Jump();
                                inputQueue.Dequeue();
                            }
                        }           
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
                if (currentBufferedInput.actionType == KenneyJamData.InputActionType.DIVE)
                {
                    if (state == State.Normal)
                    {
                        if (!grounded)
                        {
                            float targetAngleDive = Mathf.Atan2(currentBufferedInput.directionOfAction.x, currentBufferedInput.directionOfAction.y) * Mathf.Rad2Deg + cam.eulerAngles.y;
                            Vector3 diveDir = Quaternion.Euler(0, targetAngleDive, 0) * Vector3.forward;
                            //Dive();
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
    private void Dive()
    {
        SetStateToDiving();
    }
    private void HandleDive()
    {
        float powerDashSpeedMulti = 2f;
        currentDashSpeed -= currentDashSpeed * powerDashSpeedMulti * Time.deltaTime;

        if (currentDashSpeed < 20f)
        {
            SetStateToNormal();
        }
    }
    private void Dash(Vector2 directionOfDash)
    {
        timerForDashing = 0f;
        FaceLookDirection();
        transform.rotation = Quaternion.Euler(0f, targetAngle, 0);
        dashDir = directionOfDash;
        currentDashSpeed = dashSpeed;
        SetStateToDashing();
        canDash = false;
        groundHasNotBeenLeftAfterJumping = false;
        //anim
        playerAnim.SetBool("dashing", true);
        playerAnim.SetBool("falling", false);
    }
    private void HandleDash()
    {
        float powerDashSpeedMulti = 2f;
        currentDashSpeed -= currentDashSpeed * powerDashSpeedMulti * Time.deltaTime;

        if (currentDashSpeed < 30f) { 
            SetStateToNormal();
            playerAnim.SetBool("dashing", false);
        }
    }
    private void FixedHandleDash()
    {
        rb.velocity = new Vector3(transform.forward.x * currentDashSpeed, 0, transform.forward.z * currentDashSpeed);
    }


    private void Jump()
    {
        timerForDashing = timerForDashingThreshold + 1;
        if (state == State.Dashing)
        {
            jumpedOutOfDash = true;
        }
        if (state == State.Normal)
        {
            jumpedOutOfDash = false;
        }
        groundHasNotBeenLeftAfterJumping = true;
        grounded = false;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector2.up * jumpHeight, ForceMode.Impulse);

        //anim
        if (state == State.Dashing)
        {
            if (jumpedOutOfDash)
            {
                playerAnim.SetTrigger("longjump");
            }
            else
            {
                if(currentNumOfExtraJumps > 0)
                {
                    if (isOnWall)
                    {
                        playerAnim.SetTrigger("walljump");
                    }
                    else
                    {
                        playerAnim.SetTrigger("doublejump");

                    }
                }
                else
                {
                    if (isOnWall)
                    {
                        playerAnim.SetTrigger("walljump");
                    }
                    else
                    {
                        playerAnim.SetTrigger("jump");

                    }
                }
            }
        }
        else
        {
            if (currentNumOfExtraJumps > 0)
            {
                if (isOnWall)
                {
                    playerAnim.SetTrigger("walljump");
                }
                else
                {
                    playerAnim.SetTrigger("doublejump");

                }
            }
            else
            {
                if (isOnWall)
                {
                    playerAnim.SetTrigger("walljump");
                }
                else
                {
                    playerAnim.SetTrigger("jump");

                }
            }
        }
        jumpParticles.Play();
        playerAnim.SetBool("dashing", false);
        playerAnim.SetBool("falling", false);
        SetStateToNormal();
    }


    private void HandleMovement()
    {
        movement.x = inputMovement.x;
        movement.y = 0;
        movement.z = inputMovement.y;
        moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;



        if (new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude >= 0.15f)
        {
            playerAnim.SetBool("moving", true);
            if (grounded)
            {
                float currentFootstepThreshold = timerThresholdForFootstepMax / (new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude / maxSpeed);
                footstepAudioTimer += Time.deltaTime;
                if (footstepAudioTimer >= currentFootstepThreshold)
                {
                    if (AudioManager.singleton != null)
                    {
                        AudioManager.singleton.PlayRandomCarpetStep();
                    }
                    footstepAudioTimer = 0;
                }
                playerAnim.speed = new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude / maxSpeed;

            }
            else
            {
                playerAnim.speed = 1f;
            }
        }
        else
        {
            playerAnim.SetBool("moving", false);
            playerAnim.speed = 1f;
        }

        if (rb.velocity.y <= -5f && playerAnim.GetBool("falling") == false)
        {
            playerAnim.SetBool("falling", true);
        }

    }

    private void HandleFixedMovement()
    {
        //instead of using move towards i want to keep the momentum of the rb if the current velocity is higher than the input velocity this way it will only ever add force in the direction of input
        if (grounded)
        {
            rb.velocity = Vector3.MoveTowards(rb.velocity, new Vector3(moveDir.x * maxSpeed * movement.magnitude, rb.velocity.y, moveDir.z * maxSpeed * movement.magnitude), 3);
        }

        if (!grounded && !jumpedOutOfDash)
        {
            rb.velocity = Vector3.MoveTowards(rb.velocity, new Vector3(moveDir.x * maxSpeed * movement.magnitude, rb.velocity.y, moveDir.z * maxSpeed * movement.magnitude), .5f);
        }

    }
    float targetAngle;
    void FaceLookDirection()
    {
        targetAngle = Mathf.Atan2(lastMoveDir.x, lastMoveDir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;


        if (movement.magnitude > 0)
        {
            if (jumpedOutOfDash || isOnWall)
            {
                return;
            }
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
        if (DialogueManager.singleton != null)
        {
            DialogueManager.singleton.DisplayNextSentence();
        }

        BufferInput jumpBuffer = new BufferInput(KenneyJamData.InputActionType.JUMP, inputMovement, Time.time);
        inputQueue.Enqueue(jumpBuffer);
    }

    void OnDash()
    {
        BufferInput dashBuffer = new BufferInput(KenneyJamData.InputActionType.DASH, lastMoveDir.normalized, Time.time);
        inputQueue.Enqueue(dashBuffer);
    }
    void OnDive()
    {
        BufferInput diveBuffer = new BufferInput(KenneyJamData.InputActionType.DIVE, lastMoveDir.normalized, Time.time);
        inputQueue.Enqueue(diveBuffer);
    }

    private void SetStateToDashing()
    {
        state = State.Dashing;
    }
    private void SetStateToNormal()
    {
        state = State.Normal;
    }
    private void SetStateToDiving()
    {
        state = State.Diving;
    }

    void OnSelect()
    {
        if (GameManager.singleton == null)
        {
            return;
        }
        if (!GameManager.singleton.fullMapTransform.gameObject.activeSelf)
        {
            GameManager.singleton.ActivateFullMap();
            return;
        }
        if (GameManager.singleton.fullMapTransform.gameObject.activeSelf)
        {
            GameManager.singleton.DeactivateFullMap();
            return;
        }
    }

}
