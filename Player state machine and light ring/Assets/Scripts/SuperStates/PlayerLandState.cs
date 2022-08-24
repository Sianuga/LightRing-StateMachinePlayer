using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLandState : PlayerGroundedState
{
    public PlayerLandState(Player_StateMachine player, PlayerStateMachine stateMachine, PlayerData_StateMachine playerData, string animBoolName, PlayerController_StateMachine playerController) : base(player, stateMachine, playerData, animBoolName, playerController)
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

        if (input.x !=0)
        {
            stateMachine.ChangeState(player.moveState);
        }
        else
        {
            stateMachine.ChangeState(player.idleState);
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
