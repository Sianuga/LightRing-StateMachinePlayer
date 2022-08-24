using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="newPlayerData", menuName="Data/Player Data/ Base Data")]

public class PlayerData_StateMachine : ScriptableObject
{
    [Header("Movement settings")]

    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float accelerationTimeGrounded = 0.15f;

    [Header("Jump settings")]

    [SerializeField] private bool longerJumpWhenButtonHeld = false;
    [SerializeField] private bool fasterFalling = false;
    [SerializeField] private float maxJumpHeight = 4f;
    [SerializeField] private float timeToJumpApex = 2f;
    [SerializeField] private int extraJumpCount = 1;
    [SerializeField] private float accelerationTimeAirborne = 0.4f;
    [SerializeField] private float gravityIncreaseOnFall = 1;
    [SerializeField] private float jumpHoldDuration = 0;
    [SerializeField] private float jumpVelocityIncreaseOnHold = 0;
    [SerializeField] private int numCoyoteTime;
    [SerializeField] private int numJumpBufferTime;


    private int jumpBufferTime = 0;
    private int coyoteTime = 0;
    private float jumpBeingHeld = 0;
    private int jumpsUsed = 0;
    private bool reachedApex = true;
    private bool jumpBufferTriggered = false;
    private float velocityXSmoothing;
    private float gravity;
    private float maxHeightReached = Mathf.NegativeInfinity;
    private float startHeight = Mathf.NegativeInfinity;
    private float jumpVelocity;
    private float jumpTimer = 0;
    private float horizontalInput;
    private bool isFacingRight = true;
    private bool isHoldingJump = false;
    private Controls controls;
    public Vector3 velocity;
    private Vector3 prevVelocity;

    private Controller2D_variant controller;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
