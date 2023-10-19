using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player player, PlayerStateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        rb.velocity = new Vector2(rb.velocity.x, player.jumpForce);
    }

    public override void Update()
    {
        base.Update();

        if (rb.velocity.y < 0)
        {
            stateMachine.ChangeState(player.airState);
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
