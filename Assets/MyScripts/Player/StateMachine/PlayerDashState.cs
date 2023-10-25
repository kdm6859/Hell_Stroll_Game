using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player player, PlayerStateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        //player.skill.clone.CreateClone(player.transform);
        //SkillManager.instance.clone.CreateClone(player.transform);

        stateTimer = player.dashDuration;

        //player.dashCooldownTimer = player.dashCooldown;
    }

    public override void Update()
    {
        base.Update();

        //if (!player.IsGroundDetected() && player.IsWallDetected())
        //{
        //    stateMachine.ChangeState(player.wallSliderState);
        //}

        //player.SetVelocity(player.dashDir * player.dashSpeed, 0);

        if (stateTimer < 0)
            stateMachine.ChangeState(player.idleState);


    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Exit()
    {
        base.Exit();

        //player.SetVelocity(0, rb.velocity.y);
    }


}
