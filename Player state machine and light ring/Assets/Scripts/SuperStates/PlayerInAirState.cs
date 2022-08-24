using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInAirState : PlayerState
{
    private bool isGrounded;
    private bool isJumping;
    private bool jumpInput;
    private bool jumpInputFinished;
    private float inputX;
    private bool canUseCoyote;
    private bool hasBuffered;
    public PlayerInAirState(Player_StateMachine player, PlayerStateMachine stateMachine, PlayerData_StateMachine playerData, string animBoolName, PlayerController_StateMachine playerController) : base(player, stateMachine, playerData, animBoolName, playerController)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = playerController.Grounded;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override bool Equals(object other)
    {
        return base.Equals(other);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        hasBuffered = playerController.hasBuffered();
        canUseCoyote = playerController.canUseCoyote();
        inputX = player.inputHandler.MovementInput.x;
        jumpInput = player.inputHandler.JumpInput;
        jumpInputFinished = player.inputHandler.JumpInputFinished;


      
        if (isGrounded && playerController.getCurrentVerticalSpeed() < 0.01f)
        {
            stateMachine.ChangeState(player.landState);
        }
        else
        {
            player.checkFlipping(inputX);
            player.playerController.walk(inputX);
        }

        if (jumpInput)
        {
            player.inputHandler.UseJumpInput();
            stateMachine.ChangeState(player.jumpState);
        }


        //Add option to multi jump while jumping or falling

        if (isJumping)
        {
         
            if(jumpInputFinished)
            {
                player.playerController.jumpFinished(jumpInputFinished);
                isJumping = false;
                Debug.Log("Finished jump");
            }
            else if(player.playerController.getCurrentVerticalSpeed()<=0)
            {
                isJumping = false;
            }
        }
    




    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override string ToString()
    {
        return base.ToString();
    }

    public void SetIsJumping() => isJumping = true;
}
