using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player_StateMachine player, PlayerStateMachine stateMachine, PlayerData_StateMachine playerData, string animBoolName, PlayerController_StateMachine playerController) : base(player, stateMachine, playerData, animBoolName, playerController)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        player.checkFlipping(input.x);
        if(input.x==0)
        {
            stateMachine.ChangeState(player.idleState);
        }
        else
        {
            player.playerController.walk(input.x);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
