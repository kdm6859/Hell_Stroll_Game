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
        //    Debug.Log("던지기");
        //}

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    stateMachine.ChangeState(player.counterAttackState);
        //}

        //if (Input.GetKey(KeyCode.LeftControl))
        //{
        //    stateMachine.ChangeState(player.primaryAttackState);
        //}

        //땅에서 떨어지면 airState로 변경
        if (!player.IsGroundDetected())
            stateMachine.ChangeState(player.airState);

        //땅에서 Space를 누르면 jumpState로 변경
        if (Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.jumpState);
        }

        //
        if (Input.GetButtonDown("Fire1") && stateMachine.currentState != player.attackState && !player.IsAttack)// && !player.anim.GetCurrentAnimatorStateInfo(0).IsName("SwordAttack3") && !player.anim.GetCurrentAnimatorStateInfo(0).IsName("MagicAttack3"))
        {
            stateMachine.ChangeState(player.attackState);
        }

        if (Input.GetKeyDown(KeyCode.E) && stateMachine.currentState != player.skillState && !player.IsAttack && !player.anim.GetCurrentAnimatorStateInfo(0).IsName("SwordSkill"))
        {
            stateMachine.ChangeState(player.skillState);
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
