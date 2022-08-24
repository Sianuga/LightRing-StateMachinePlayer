using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    protected Vector2 input;
    private bool jumpInput;
    protected bool isGrounded;
    private bool canUseCoyote;

    public PlayerGroundedState(Player_StateMachine player, PlayerStateMachine stateMachine, PlayerData_StateMachine playerData, string animBoolName, PlayerController_StateMachine playerController) : base(player, stateMachine, playerData, animBoolName, playerController)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        isGrounded = playerController.Grounded;

        playerController.resetJumpCounter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        input = player.inputHandler.MovementInput;
        jumpInput = player.inputHandler.JumpInput;
     

        if (jumpInput)
        {
            player.inputHandler.UseJumpInput();
            stateMachine.ChangeState(player.jumpState);
        }
        if(!isGrounded)
        {
            playerController.incrementJumpCounter();
            stateMachine.ChangeState(player.inAirState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
