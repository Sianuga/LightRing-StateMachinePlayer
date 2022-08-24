using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement_OLD : MonoBehaviour
{
    private PlayerInput playerController;

    [Header("Movement settings")]

    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float accelerationTimeGrounded = 0.15f;

    [Header("Jump settings")]

    [SerializeField] private bool longerJumpWhenButtonHeld = false;
    [SerializeField] private bool fasterFalling = false;
    [SerializeField] private float maxJumpHeight = 4f;
    [SerializeField] private float timeToJumpApex = 2f;
    [SerializeField] private int extraJumpCount=1;
    [SerializeField] private float accelerationTimeAirborne= 0.4f;
    [SerializeField] private float gravityIncreaseOnFall=1;
    [SerializeField] private float jumpHoldDuration=0;
    [SerializeField] private float jumpVelocityIncreaseOnHold=0;
    [SerializeField] private int numCoyoteTime;
    [SerializeField] private int numJumpBufferTime;
  
    
    private int jumpBufferTime=0;
    private int coyoteTime=0;
    private float jumpBeingHeld = 0;
    private int jumpsUsed=0;
    private bool reachedApex=true;
    private bool jumpBufferTriggered=false;
    private float velocityXSmoothing;
    private float gravity;
    private float maxHeightReached = Mathf.NegativeInfinity;
    private float startHeight = Mathf.NegativeInfinity;
    private float jumpVelocity;
    private float jumpTimer=0;
    private float horizontalInput;
    private bool isFacingRight=true;
    private bool isHoldingJump = false;
    private Controls controls;
    public Vector3 velocity;
    private Vector3 prevVelocity;

    private Controller2D_variant controller;

      void Awake()
    {
        controller = GetComponent<Controller2D_variant>();
        controls = new Controls();
       controls.Gameplay.Jump.started += jump;
        controls.Gameplay.Jump.performed += jump;
        controls.Gameplay.Movement.performed += move;
        controls.Gameplay.Jump.canceled += jump;
        controls.Gameplay.Movement.canceled += move;
    }
    void Start()
    {
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = 2*Mathf.Abs(gravity) * timeToJumpApex;
    }
    private void Update()
    {
        if (controls.Gameplay.Jump.triggered)
        {
            jumpBufferTime = 0;
            jump();
        }

    }

    private void FixedUpdate()
    {
    
        if (!reachedApex && maxHeightReached > transform.position.y)
        {
            reachedApex = true;
            gravity = gravity * gravityIncreaseOnFall;
        }
        maxHeightReached = Mathf.Max(transform.position.y, maxHeightReached);
        if (!controller.collisions.below && !reachedApex)
        {
            jumpTimer += Time.fixedDeltaTime;
        }
        ++jumpBufferTime;
        ++coyoteTime;
        prevVelocity = velocity;
        velocity.y += gravity * Time.deltaTime;
        if(isHoldingJump && (jumpBeingHeld<=jumpHoldDuration))
        {
            jumpBeingHeld += Time.fixedDeltaTime;
            velocity.y += jumpVelocityIncreaseOnHold;
        }
      Vector3 deltaPosition = (prevVelocity + velocity) * 0.5f * Time.fixedDeltaTime;
        move(deltaPosition);
        handleHorizontalMovement(); 

        if ((controller.collisions.below || controller.collisions.above) )
        {
            velocity.y = 0;
        }
       /* if((controller.collisions.left || controller.collisions.right)&&!controller.collisions.climbingSlope)
        {
            velocity.x = 0;
        }*/
        if(controller.collisions.below)
        {
            coyoteTime = 0;
            jumpsUsed = 0;
            jumpTimer = 0;
        }
  
      
        
       
    }

    private void handleHorizontalMovement()
    {
        Vector2 input = new Vector2(horizontalInput, 0);
        float targetVelocityX = input.x * movementSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x,targetVelocityX,ref velocityXSmoothing,(controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
    }

    public Vector3 getVelocity()
    {
        return velocity;
    }
 
    public void move(Vector3 velocity)
    {
        controller.updateRaycastOrigins();
        controller.collisions.reset();
        controller.collisions.velocityOld = velocity;
     /*   if (velocity.y <0)
        {
            controller.descendSlope(ref velocity);
        }*/
if(velocity.x!=0)
        {
  controller.HorizontalCollisions(ref velocity);
        }
if (velocity.y != 0)
        {
 controller.VerticalCollisions(ref velocity);
        }
        
        
        transform.Translate(velocity);
    }
    public float getHorizontalInput()
    {
        return horizontalInput;
    }
  
    private void OnEnable()
    {
          controls.Gameplay.Enable();
    }
    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

   
    public void handleFlipping()
    {
        if (!isFacingRight && horizontalInput > 0f)
        {
            flip();
        }
        else if (isFacingRight && horizontalInput < 0f)
        {
            flip();
        }
    }
  private void flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
    public void move(InputAction.CallbackContext context)
    {
        if (!context.canceled)
            horizontalInput = context.ReadValue<Vector2>().x;
        else
            horizontalInput = Vector2.zero.x;
  }
    private void jump()
    {
        if ((controller.collisions.below )|| coyoteTime<numCoyoteTime)
        {
            jumpsUsed++;
            controller.collisions.below = false;
            performJump();
            coyoteTime = numCoyoteTime;
            jumpBufferTime = numJumpBufferTime;
        }
        else if ((jumpsUsed <= extraJumpCount)&& extraJumpCount!=0)
        {
            jumpsUsed += 2;
            performJump();
        }
        else if(jumpBufferTime<numJumpBufferTime)
        {
            StartCoroutine(SearchForGround());
        }
        
    }

    private IEnumerator SearchForGround()
    {
        while(jumpBufferTime<numJumpBufferTime)
        {
            if(controller.collisions.below)
            {
                jumpBufferTime = numJumpBufferTime;
                jumpsUsed++;
                performJump();
            }
        yield return new WaitForFixedUpdate();
        }

    }

    private void performJump()
    {
        jumpTimer = 0;
        velocity.y = maxJumpHeight;
        maxHeightReached = Mathf.NegativeInfinity;
        startHeight = transform.position.y;
        reachedApex = false;
        gravity = -2 * maxJumpHeight / Mathf.Pow(timeToJumpApex, 2);
    }

    private void jump(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            isHoldingJump = true;
        }
        if(context.performed)
        {
           
        }
        if (context.canceled)
        {
            isHoldingJump = false;
            jumpBeingHeld = 0;
        }
           
    }
    public Vector2 getPlayerPosition()
    {
        return GetComponent<Transform>().position;
    }

}
