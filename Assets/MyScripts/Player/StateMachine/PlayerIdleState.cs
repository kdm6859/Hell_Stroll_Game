using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(Player player, PlayerStateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        //rb.velocity = new Vector3(0, rb.velocity.y, 0);
    }

    public override void Update()
    {
        base.Update();

        //if (xInput == player.facingDir && player.IsWallDetected())
        //{
        //    return;
        //}

        


        if (xInput != 0 || zInput != 0)//&& !player.isBusy)
        {
            player.stateMachine.ChangeState(player.moveState);
        }


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
