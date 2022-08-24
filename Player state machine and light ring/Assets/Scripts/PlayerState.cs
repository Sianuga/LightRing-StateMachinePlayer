using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    protected Player_StateMachine player;
    protected PlayerStateMachine stateMachine;
    protected PlayerData_StateMachine playerData;
    protected PlayerController_StateMachine playerController;

    protected float startTime;

    private string animBoolName;

    public PlayerState(Player_StateMachine player, PlayerStateMachine stateMachine, PlayerData_StateMachine playerData, string animBoolName, PlayerController_StateMachine playerController)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.playerData = playerData;
        this.playerController = playerController;
        this.animBoolName = animBoolName;
    }

 

    public virtual void Enter()
    {
        DoChecks();
   //   player.animator.SetBool(animBoolName, true);
        startTime = Time.time;

    }  
    public virtual void Exit()
    {
      //player.animator.SetBool(animBoolName, false);
    }
    public virtual void LogicUpdate()
    {

    }
    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }
    public virtual void DoChecks()
    {

    }
}
