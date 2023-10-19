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

        rb.velocity = new Vector3(rb.velocity.x, player.jumpForce, rb.velocity.z);
        //player.SetVelocity(1000, rb.velocity.y, 1000);
        //Debug.Log(rb.velocity);

        //player.SetVelocity(100f, 100, 100f);
        //Debug.Log("Enter" + rb.velocity);
        
    }

    public override void Update()
    {
        base.Update();
        
        if (rb.velocity.y < 0)
        {
            stateMachine.ChangeState(player.airState);
        }
        //Debug.Log("Update" + rb.velocity);

        //if (xInput != 0)
        //    player.SetVelocity(player.moveSpeed * 0.8f * xInput, rb.velocity.y);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        //Vector3 moveVec = player.transform.forward * player.moveSpeed;
        //player.SetVelocity(new Vector3(100f, rb.velocity.y, 100f));
        //Debug.Log(rb.velocity);
        //rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
    }

    public override void Exit()
    {
        base.Exit();
    }

   
}
