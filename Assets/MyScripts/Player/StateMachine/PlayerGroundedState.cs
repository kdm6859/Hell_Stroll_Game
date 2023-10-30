using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    protected float moveTimer;

    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, string animBoolName)
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

        //if (Input.GetKeyDown(KeyCode.Mouse1) && HasNoSword())
        //{
        //    stateMachine.ChangeState(player.aimSwordState);
        //    Debug.Log("´øÁö±â");
        //}

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    stateMachine.ChangeState(player.counterAttackState);
        //}

        //if (Input.GetKey(KeyCode.LeftControl))
        //{
        //    stateMachine.ChangeState(player.primaryAttackState);
        //}

        if (!player.IsGroundDetected())
            stateMachine.ChangeState(player.airState);

        if (Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.jumpState);
        }

        if (Input.GetButtonDown("Fire1") && stateMachine.currentState != player.attackState && player.isAttack)
        {
            stateMachine.ChangeState(player.attackState);
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

    private bool HasNoSword()
    {
        //if (!player.sword)
        //{
        //    return true;
        //}

        //player.sword.GetComponent<SwordSkillController>().ReturnSword();

        return false;
    }

}
