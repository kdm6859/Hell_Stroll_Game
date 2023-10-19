using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(Player player, PlayerStateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        //if (player.IsWallDetected())
        //{
        //    stateMachine.ChangeState(player.wallSliderState);
        //}

        if (player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }

        //if (xInput != 0)
        //    player.SetVelocity(player.moveSpeed * 0.8f * xInput, rb.velocity.y);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Exit()
    {
        base.Exit();
    }


}
