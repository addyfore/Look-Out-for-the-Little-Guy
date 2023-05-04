using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private Animator anim;
    private SpriteRenderer sprite;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private Transform wallCheck;


    private float dirX = 0f;
    private bool isFacingRight = true;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;


    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    private bool isDoubleJumping = false;
    private bool isJumping = false;
    private bool isFalling;
    private float maxFallingSpeed = 15f;
    private float jumpCounter;

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 0.5f;
    private float originalGravity;

    private bool isWallSliding;
    private float wallSlidingSpeed = 5f;
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.2f;
    private Vector2 wallJumpingPower = new Vector2(10f, 12f);

    private enum MovementState { idle, running, jumping, falling, wallSliding, doubleJump }

    [SerializeField] private AudioSource jumpSoundEffect;
    [SerializeField] private AudioSource dashSoundEffect;

    // Start is called before the first frame update
    private void Start()
    {
        // Initializing all components used in script
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        tr = GetComponent<TrailRenderer>();

        originalGravity = rb.gravityScale;
    }

    // Update is called once per frame
    private void Update()
    {

        // Prevents movement while player is dashing
        if (isDashing)
        {
            UpdateAnimationState();
            return;
        }
        if (isWallJumping)
        {
            UpdateAnimationState();
            return;
        }

        // Checks if the player is falling
        isFalling = rb.velocity.y < 0f;

        // Checks which direction the player is moving
        dirX = Input.GetAxisRaw("Horizontal");

        // Sets the velocity of the player
        rb.velocity = new Vector2(dirX * moveSpeed, Mathf.Clamp(rb.velocity.y, -maxFallingSpeed, float.MaxValue));

        // Establishing coyote time and a jump buffer
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (IsGrounded() && !Input.GetButton("Jump"))
        {
            isJumping = false;
            isDoubleJumping = false;
            tr.emitting = false;
        }

        if (IsGrounded() || IsWalled())
        {
            canDash = true;
            rb.gravityScale = originalGravity;
        }

        // Jump logic
        if (jumpBufferCounter > 0f && CanJump())
        {
            jumpSoundEffect.Play();
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpBufferCounter = 0f;
            if (isDoubleJumping)
            {
                tr.emitting = true;
            }

        }

        // Cut Jump Short
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            coyoteTimeCounter = 0f;
        }

        // Dash logic
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            dashSoundEffect.Play();
            StartCoroutine(Dash());
        }

        WallSlide();
        WallJump();
        UpdateAnimationState();
    }

    // Sets the animation of the player to correspond with their movement
    private void UpdateAnimationState()
    {
        MovementState state;

        if (dirX != 0f || isDashing)
        {
            state = MovementState.running;
            Flip();
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        if (isDoubleJumping)
        {
            state = MovementState.doubleJump;
        }

        if (isWallSliding)
        {
            state = MovementState.wallSliding;
        }

        anim.SetInteger("state", (int)state);
    }

    // Checks if player is currently on the ground
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, jumpableGround);
    }

    // Checks if player is currently connected to a wall
    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, jumpableGround);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && dirX != 0)
        {
            isWallSliding = true;
            isJumping = false;
            isDoubleJumping = false;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && CanJump())
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    // Flips the character to correspond with their movement
    private void Flip()
    {
        if (isFacingRight && dirX < 0f || !isFacingRight && dirX > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    // Dash shoots the player forward with zero gravity for a moment
    // and prevents this from happening again for a short time
    private IEnumerator Dash()
    {
        float originalGravity = rb.gravityScale;
        canDash = false;
        isDashing = true;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
    }

    /* After double jumping, return to the falling state
    private void AfterDouble()
    {
        isDoubleJumping = false;
    } */

    private bool CanJump()
    {
        if (!isDoubleJumping)
        {
            if (isWallSliding && wallJumpingCounter > 0f)
            {
                return true;
            }
            else if (jumpBufferCounter > 0f)
            {
                if (coyoteTimeCounter > 0f)
                {
                    isJumping = true;
                    return true;
                }
                else if (isJumping || isFalling)
                {
                    isDoubleJumping = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
