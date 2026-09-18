using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float crawlVelocity = 20f;
    public float walkVelocity = 150f;
    public float runVelocity = 250f;
    public float jumpWalkLinear = 3f;
    public float jumpRunLinear = 6f;

    public float orgMoveSpeed = 5f;
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float swimSpeed = 20f;
    public float maxLinearVelocity = 6.4f;
    public float orgMaxLinearVelocity;

    public Transform groundCheck;
    public float groundDistance = 0.0f;
    public LayerMask groundMask;

    [HideInInspector]
    public Rigidbody rb;
    private Vector2 moveInput;
    public bool isGrounded;
    public float minSlopeThreshold = 10f; // degrees
    public float maxSlopeThreshold = 60f;

    public PlayerInput playerInput;
    public PlayerControllerInput playerControllerInput;

    public AudioClip footstepSFX;
    public float volume = 1f;

    public Transform org_trans;
    public bool isCrouching;
    public float crouchDistance;
    public PlayerShooting playerShooting;

    public bool isCrawling = false;
    public float rotationSpeed = 45f;
    public Transform ceilingCheck;
    public float ceilingDistance = 0.5f;
    
    private InputAction runAction;
    public bool isRunning = false;
    public double maxStamina = 100;
    public double stamina = 100;

    private UIManager ui;

    public bool isSwimming;
    public Transform swimTarget;

    public bool moveDisabled;

    public float currentCoyoteTime;
    public float maxCoyoteTime = 3f;

    public bool isJumping;
    public Inventory inv;

    public Transform respawnPoint;

    public BoolValue hasDoubleJump;
    public bool isDoubleJumping;

    public BoolValue hasDash;
    public bool isDashing;
    public float dashTime;
    public float dashResetTime;
    public int dashCost = 50;
    public float maxDashVelocity;
    public float dashVelocity;
    public float maxYVelocity;
    public float orgMaxYVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveSpeed = orgMoveSpeed;

        //StartCoroutine(PlayFootstepSFX());
        org_trans = this.transform;

        if (this.CompareTag("Player"))
        {
            playerInput = new PlayerInput();
            playerInput.OnFoot.Enable();
            ui = GameObject.Find("PlayerKeyboardUI").GetComponent<UIManager>();
            runAction = playerInput.FindAction("Run");
            //playerControllerInput = null;
        } else if(this.CompareTag("PlayerController"))
        {
            playerControllerInput = new PlayerControllerInput();
            playerControllerInput.GroundTraversal.Enable();
            ui = GameObject.Find("PlayerControllerUI").GetComponent<UIManager>();
            //playerInput = null;
        }

        playerShooting = GetComponent<PlayerShooting>();
        maxStamina = inv.maxStamina;

        Application.targetFrameRate = 60;

        orgMaxLinearVelocity = maxLinearVelocity;
        orgMaxYVelocity = maxYVelocity;
    }

    void Update()
    {
        maxStamina = inv.maxStamina;

        if (!isSwimming)
        {
            rb.useGravity = true;
            if (isCrouching)
            {
                orgMoveSpeed = crawlVelocity;
                if (isGrounded)
                {
                    rb.drag = 10f;
                }
                else
                {
                    rb.drag = 0.1f;
                }
            }
            else
            {
                if (isGrounded)
                {
                    rb.drag = 10f;
                    if (isRunning)
                    {
                        if (stamina >= 0.5)
                        {
                            stamina -= 0.5;
                            ui.SetStaminaValue(stamina);
                            orgMoveSpeed = runVelocity;
                        }
                        else
                        {
                            orgMoveSpeed = walkVelocity;
                        }
                    }
                    else
                    {
                        if (stamina < maxStamina)
                        {
                            stamina += 0.5;
                            ui.SetStaminaValue(stamina);
                        }
                        orgMoveSpeed = walkVelocity;
                    }
                }
                else
                {
                    rb.drag = 0.1f;
                    if (isRunning)
                    {
                        orgMoveSpeed = jumpRunLinear;
                    }
                    else
                    {
                        if (stamina < maxStamina)
                        {
                            stamina += 0.5;
                            ui.SetStaminaValue(stamina);
                        }
                        orgMoveSpeed = jumpWalkLinear;
                    }
                    LimitSpeed();
                }
            }
        }
        else
        {
            rb.useGravity = false;
        }

        if (!isDashing)
        {
            moveSpeed = orgMoveSpeed;
        }

        if (!isSwimming && !isCrawling)
        {
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        }else if (isCrawling)
        {
            transform.rotation = Quaternion.Euler(90, transform.eulerAngles.y, transform.eulerAngles.z);
        }

        if (isGrounded && !isJumping && rb.velocity.y <= 0) // need to check velocity because isGrounded is true within a certain distance of the ground (not when player touches ground mask)
        {
            currentCoyoteTime = maxCoyoteTime;
        }
        else
        {
            currentCoyoteTime -= Time.deltaTime;
        }
        //CheckFootstep();
    }

    public void FixedUpdate()
    {
        CheckGround();
        CheckYVelocity();

        if (!isSwimming)
        {
            MovePlayer();
        }
        else
        {
            MovePlayerUnderwater();
        }
    }

    public void OnJump()
    {
        if (!isSwimming && !isCrouching && !moveDisabled && currentCoyoteTime > 0f && !isJumping) // got rid of isGrounded
        {
            currentCoyoteTime = 0f;
            isJumping = true;

            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            //rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }else if((hasDoubleJump.RuntimeValue == true) && !isDoubleJumping)
        {
            isDoubleJumping = true;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && rb.velocity.y <= 0) {
            isJumping = false;
            isDoubleJumping = false;
        }
    }

    public void OnDash()
    {
        if (!isDashing && hasDash.RuntimeValue == true && !isSwimming && !isCrouching && !moveDisabled && stamina >= dashCost)
        {
            StartCoroutine(Dash());
        }
    }

    public IEnumerator Dash()
    {
        maxYVelocity = 0f;
        stamina -= dashCost;
        maxLinearVelocity = maxDashVelocity;
        isDashing = true;
        moveSpeed = dashVelocity;

        yield return new WaitForSeconds(dashTime);

        maxYVelocity = orgMaxYVelocity;
        maxLinearVelocity = orgMaxLinearVelocity;
        if (isRunning)
        {
            moveSpeed = runVelocity;
        }
        else
        {
            moveSpeed = walkVelocity;
        }
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        
        yield return new WaitForSeconds(dashResetTime);
        isDashing = false;
    }

    public void CheckYVelocity()
    {
        if (rb.velocity.y > maxYVelocity)
        {
            rb.velocity = new Vector3(rb.velocity.x, maxYVelocity, rb.velocity.z);
        }
    }

    public void OnReset()
    {
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    public void OnRun()
    {
        isRunning = !isRunning;
    }

    public void OnMovement(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void MovePlayer()
    {
        if (isCrawling)
        {
            float translateInput = 0f;

            if (CompareTag("Player"))
            {
                translateInput = playerInput.OnFoot.Movement.ReadValue<Vector2>().y;
                Vector3 movement = transform.up * translateInput * 5f * 0.5f * Time.fixedDeltaTime;
                transform.Translate(movement, Space.World);

                float rotateInput = playerInput.OnFoot.Movement.ReadValue<Vector2>().x;
                transform.Rotate(Vector3.forward * -rotateInput * 50f * Time.fixedDeltaTime);
                //ceilingCheck.eulerAngles = new Vector3(-1f * this.transform.rotation.x,0,0);
            }
            else if(CompareTag("PlayerController"))
            {
                translateInput = playerControllerInput.GroundTraversal.Movement.ReadValue<Vector2>().y;
                Vector3 movement = transform.up * translateInput * 5f * 0.5f * Time.fixedDeltaTime;
                transform.Translate(movement, Space.World);

                float rotateInput = playerControllerInput.GroundTraversal.Movement.ReadValue<Vector2>().x;
                transform.Rotate(Vector3.forward * -rotateInput * 50f * Time.fixedDeltaTime);
                //ceilingCheck.eulerAngles = new Vector3(-1f * this.transform.rotation.x,0,0);
            }

            if(translateInput == 0f && rb.velocity.magnitude > 0)
            {
                ResetVelocity();
            }
        }
        else
        {
            Vector3 inputDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
            inputDirection.Normalize();

            Vector3 moveDirection = inputDirection;

            RaycastHit hit;

            if(Physics.Raycast(groundCheck.position, Vector3.down, out hit, 1.1f, groundMask))
            {
                if (!(hit.collider.gameObject.name == "Terrain"))
                {
                    Vector3 groundNormal = hit.normal;
                    moveDirection = Vector3.ProjectOnPlane(inputDirection, groundNormal).normalized;
                    rb.AddForce(moveDirection * moveSpeed, ForceMode.Force);
                }
                else
                {
                    rb.AddForce(new Vector3(moveDirection.x * moveSpeed, 0f, moveDirection.z * moveSpeed));
                }
            }
            else
            {
                //rb.velocity = new Vector3(direction.x * moveSpeed, rb.velocity.y, direction.z * moveSpeed);
                rb.AddForce(new Vector3(moveDirection.x * moveSpeed, 0f, moveDirection.z * moveSpeed));
            }
        }
    }

    public void MovePlayerUnderwater()
    {
        if (CompareTag("Player"))
        {
            if (playerInput.OnFoot.Movement.ReadValue<Vector2>().y > 0)
            {
                ResetVelocity();
                transform.position += swimTarget.forward * swimSpeed * Time.deltaTime;
            }
            else if (playerInput.OnFoot.Movement.ReadValue<Vector2>().y < 0)
            {
                ResetVelocity();
                transform.position -= swimTarget.forward * swimSpeed * Time.deltaTime;
            }
        }
        else if (CompareTag("PlayerController"))
        {
            if (playerControllerInput.GroundTraversal.Movement.ReadValue<Vector2>().y > 0)
            {
                ResetVelocity();
                transform.position += swimTarget.forward * swimSpeed * Time.deltaTime;
            }
            else if (playerControllerInput.GroundTraversal.Movement.ReadValue<Vector2>().y < 0)
            {
                ResetVelocity();
                transform.position -= swimTarget.forward * swimSpeed * Time.deltaTime;
            }
        }
    }

    public void OnCrouch()
    {
        if ((!isCrouching  && !isCrawling || !CheckCeiling() && !isCrawling) && !isSwimming && isGrounded && !moveDisabled)
        {
            isCrouching = !isCrouching;
            if (isCrouching)
            {
                this.transform.localScale = new Vector3(transform.localScale.x, org_trans.localScale.y * crouchDistance, transform.localScale.z);
                orgMoveSpeed = crawlVelocity;
            }
            else
            {
                this.transform.localScale = new Vector3(transform.localScale.x, org_trans.localScale.y / crouchDistance, transform.localScale.z);
            }
            ResetVelocity();
        }
    }

    public void OnCrawl()
    {
        if (!isSwimming && (isGrounded || isCrawling) && !moveDisabled)
        {
            isCrawling = !isCrawling;
            if (isCrawling)
            {
                Quaternion targetRotation = Quaternion.Euler(90, transform.eulerAngles.y, 0);
                StartCoroutine(RotateOverTime(targetRotation, 1f));
                isCrouching = true;
                this.transform.localScale = new Vector3(transform.localScale.x, crouchDistance, transform.localScale.z);
            }
            else
            {
                Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
                StartCoroutine(RotateOverTime(targetRotation, 1f));
                isCrouching = true;
                this.transform.localScale = new Vector3(transform.localScale.x, crouchDistance, transform.localScale.z);
            }
            ResetVelocity();
        }
    }

    public bool CheckCeiling()
    {
        return Physics.Raycast(ceilingCheck.position, Vector3.up, ceilingDistance);
    }

    public IEnumerator RotateOverTime(Quaternion targetRotation, float duration)
    {
        Quaternion startRotation = transform.rotation;
        float timeElapsed = 0f;

        while(timeElapsed < duration)
        {
            float progress = timeElapsed / duration;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, progress);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    public void CheckFootstep()
    {
        if(rb.velocity.magnitude > 0.1f && isGrounded)
        {
            if (!AudioManager.instance.isPlaying)
            {
                AudioManager.instance.PlaySFX(footstepSFX, volume);
            }
        }
        else
        {
            if (AudioManager.instance.isPlaying)
            {
                AudioManager.instance.StopSFX(footstepSFX);
            }
        }
    }

    IEnumerator PlayFootstepSFX()
    {
        while (true)
        {
            if(rb.velocity.magnitude > 1f && isGrounded)
            {
                AudioManager.instance.PlaySFX(footstepSFX,volume);
                yield return new WaitForSeconds(0.2f);
            }else if(rb.velocity.magnitude > 0.1f && isGrounded)
            {
                AudioManager.instance.PlaySFX(footstepSFX, volume);
            }

            if(rb.velocity.magnitude < 0.1f || !isGrounded)
            {
                AudioManager.instance.StopSFX(footstepSFX);
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void ResetVelocity()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public int CheckSlope()
    {
        // 0: no slope; 1: movable slope; 2: immovable slope
        if(Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit hit, 1.2f, groundMask))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if(angle > maxSlopeThreshold)
            {
                return 2;
            }else if(angle > minSlopeThreshold)
            {
                return 1;
            }
        }
        return 0;
    }

    public void LimitSpeed()
    {
        Vector3 horVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if(horVel.magnitude > maxLinearVelocity)
        {
            horVel = horVel.normalized * maxLinearVelocity;

            rb.velocity = new Vector3(
                horVel.x,
                rb.velocity.y,
                horVel.z);
        }
    }

    public void StandUpright()
    {
        if (isCrawling)
        {
            isCrawling = !isCrawling;
            if (isCrawling)
            {
                Quaternion targetRotation = Quaternion.Euler(90, transform.eulerAngles.y, 0);
                StartCoroutine(RotateOverTime(targetRotation, 1f));
                isCrouching = true;
                transform.localScale = new Vector3(transform.localScale.x, crouchDistance, transform.localScale.z);
            }
            else
            {
                Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
                StartCoroutine(RotateOverTime(targetRotation, 1f));
                isCrouching = true;
                transform.localScale = new Vector3(transform.localScale.x, crouchDistance, transform.localScale.z);
            }
        }
        if (isCrouching)
        {
            isCrouching = !isCrouching;
            if (isCrouching)
            {
                transform.localScale = new Vector3(transform.localScale.x, org_trans.localScale.y * crouchDistance, transform.localScale.z);
                orgMoveSpeed = crawlVelocity;
            }
            else
            {
                transform.localScale = new Vector3(transform.localScale.x, org_trans.localScale.y / crouchDistance, transform.localScale.z);
            }
        }
    }
}
