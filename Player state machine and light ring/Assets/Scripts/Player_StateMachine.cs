using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_StateMachine : MonoBehaviour
{
    #region StateVariables
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerInAirState inAirState { get; private set; }
    public PlayerLandState landState { get; private set; }

    #endregion
    public Animator animator { get; private set; }
    public PlayerInputHandler inputHandler { get; private set; }

    public PlayerController_StateMachine playerController;

    [SerializeField]
    private PlayerData_StateMachine playerData;

    bool isFacingRight = true;

    float gravity = Physics.gravity.y;
    float distance;
    GameObject[] blocks;
    GameObject lightRing, brick;
    [SerializeField] float lerpTime = 1f;
    bool lightRingIsAlive = false;
    float currentLerpTime, perc;
    [SerializeField] float timeToPlatformOutlineDissapear=1f;
    [SerializeField] float renderDistanceForLightRing=30f;
    bool startReversing=false;
    int blockBeingChanged = 0;

    void Awake()
    {
        playerData = new PlayerData_StateMachine();
        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this, stateMachine, playerData, "idle", playerController);
        moveState = new PlayerMoveState(this, stateMachine, playerData, "move", playerController);
        jumpState = new PlayerJumpState(this, stateMachine, playerData, "inAir", playerController);
        inAirState = new PlayerInAirState(this, stateMachine, playerData, "inAir", playerController);
        landState = new PlayerLandState(this, stateMachine, playerData, "land", playerController);
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        inputHandler = GetComponent<PlayerInputHandler>();
        playerController = GetComponent<PlayerController_StateMachine>();
        blocks = GameObject.FindGameObjectsWithTag("Blocks");
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        stateMachine.currentState.LogicUpdate();

        if (Input.GetKeyDown(KeyCode.Q) && !lightRingIsAlive)
        {
            foreach (GameObject block in blocks)
            {
                block.GetComponent<Block>().SetUpStartingDistance(transform.position);
            }
            FindObjectOfType<LightRing>().CreateLightRing();
        }


        SearchForLightRing();
        if(startReversing)
        {

        }
        if (lightRingIsAlive)
        {
            Change();
        }
    }
    void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();


  
 
            
    }
    private void Change()
    {
        foreach (GameObject block in blocks)
        {
            if (block.GetComponent<Block>().GetStartingDistance() < renderDistanceForLightRing)
            {
                block.GetComponent<Block>().setBeingChanged(true);
                changeShaderDirection(block);
                changeShaderVisibility(block);
            }
           // returnToShaderRestState(3);
        }

    }
    public void returnToShaderRestState(float timeToReturn)
    {
        foreach (GameObject block in blocks)
        {

            //block.GetComponent<Renderer>().material.SetFloat
            StartCoroutine(visibilityOverTime(timeToReturn,block));


           /* if (block.GetComponent<Renderer>().material.GetFloat("_Fade")>0.1f)
            {
                float fadeValue = block.GetComponent<Renderer>().material.GetFloat("_Fade");
                StartCoroutine(increaseShaderVisibility(fadeValue, block));
              var yMovement =  block.GetComponent<Renderer>().material.GetFloat("_yMovement");
               var xMovement = block.GetComponent<Renderer>().material.GetFloat("_xMovement");
                block.GetComponent<Renderer>().material.SetFloat("_yMovement", -yMovement);
                block.GetComponent<Renderer>().material.SetFloat("_xMovement", -xMovement);
                fadeValue = block.GetComponent<Renderer>().material.GetFloat("_Fade");
                StartCoroutine(platformOutlineVisibilityDuration(fadeValue,block));
            }
            block.GetComponent<Renderer>().material.SetFloat("_Fade", 0);*/
        }
    }

    private IEnumerator visibilityOverTime(float duration, GameObject block)
    {
        yield return new WaitForSeconds(5);
        float time = 0;
        float startValue = block.GetComponent<Renderer>().material.GetFloat("_Fade");
        while (time < duration)
        {
            block.GetComponent<Renderer>().material.SetFloat("_Fade", Mathf.Lerp(startValue, duration, time / duration));
            time += Time.deltaTime;
        }
        block.GetComponent<Renderer>().material.SetFloat("_Fade",0f);


    }

    IEnumerator platformOutlineVisibilityDuration(float timeToReturn, GameObject block)
    {
        yield return new WaitForSeconds(timeToPlatformOutlineDissapear);
        StartCoroutine(decreaseShaderVisibility(timeToReturn, block));
    }

    IEnumerator decreaseShaderVisibility(float timeToReturn, GameObject block)
    {
        while (timeToReturn > 0)
        {
            timeToReturn -= 0.01f;
            block.GetComponent<Renderer>().material.SetFloat("_OutlineVisibility", timeToReturn);
            yield return new WaitForSeconds(0.003f);
        }
    }
    IEnumerator increaseShaderVisibility(float timeToFullVisibility, GameObject block)
    {
        while (timeToFullVisibility < 1)
        {
            timeToFullVisibility += 0.01f;
            block.GetComponent<Renderer>().material.SetFloat("_Fade", timeToFullVisibility);
            block.GetComponent<Renderer>().material.SetFloat("_OutlineVisibility", timeToFullVisibility);
            yield return new WaitForSeconds(0.001f);
        }
    }
    
    private void changeShaderVisibility(GameObject block)
    {
        distance = Vector2.Distance(lightRing.GetComponent<CircleCollider2D>().ClosestPoint(block.transform.position), block.transform.position);
        perc = distance / block.GetComponent<Block>().GetStartingDistance();
        lerpTime = Mathf.Lerp(0, 1, perc * perc * perc * (perc * (6f * perc - 15f) + 10f));
        //lerpTime = Mathf.Lerp(0, 1, (6.052f * Mathf.Pow(perc, 5) + 18.155f * Mathf.Pow(perc, 4) + 16.743f * Mathf.Pow(perc, 3) - 3.994f * Mathf.Pow(perc, 2) + 0.366f * perc - 0.012f));
        //lerpTime = Mathf.Lerp(0, 1, Mathf.Pow(10f * perc * 1.235f - 1f, 3) * ((Mathf.Pow(150f * perc, 2) - 405f * perc + 289f) / 2f*24790f));
        block.GetComponent<Renderer>().material.SetFloat("_Fade", 1 - lerpTime);
        block.GetComponent<Renderer>().material.SetFloat("_OutlineVisibility", 1 - lerpTime);
    }

    private void changeShaderDirection(GameObject block)
    {
        float a = (block.transform.position.x - lightRing.transform.position.x)
        / Mathf.Sqrt(Mathf.Pow(block.transform.position.x - lightRing.transform.position.x, 2) + Mathf.Pow(block.transform.position.y - lightRing.transform.position.y, 2));
        if (lightRing.transform.position.y >= block.transform.position.y)
        {
            if (a < 0)
                block.GetComponent<Renderer>().material.SetFloat("_yMovement", 1f + a);
            else
                block.GetComponent<Renderer>().material.SetFloat("_yMovement", 1f - a);

            block.GetComponent<Renderer>().material.SetFloat("_xMovement", -a);
        }
        else
        {
            if (a < 0)
                block.GetComponent<Renderer>().material.SetFloat("_yMovement", -1f - a);
            else
                block.GetComponent<Renderer>().material.SetFloat("_yMovement", -1f + a);
            block.GetComponent<Renderer>().material.SetFloat("_xMovement", -a);
        }
    }

    private void SearchForLightRing()
    {
        if (!FindObjectOfType<Ringo>())
        {
            if (lightRingIsAlive)
                startReversing = true;
            lightRingIsAlive = false;
            return;
        }
        else
        {
            lightRingIsAlive = true;
            lightRing = FindObjectOfType<Ringo>().gameObject;

        }
    }

    public void checkFlipping(float inputX)
    {
        if (!isFacingRight && inputX > 0f)
        {
            flip();
        }
        else if (isFacingRight && inputX < 0f)
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

}
