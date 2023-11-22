using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundedState : EnemyState
{
    protected float moveTimer;

    public EnemyGroundedState(Enemy enemy, EnemyStateMachine stateMachine, string animBoolName)
        : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        //if (!enemy.IsGroundDetected())
        //    stateMachine.ChangeState(enemy.airState);

        //if (Input.GetKeyDown(KeyCode.Space) && enemy.IsGroundDetected())
        //{
        //    stateMachine.ChangeState(enemy.jumpState);
        //}

        if (enemy.DistanceCheck(DistanceCheckType.Attack) && stateMachine.currentState != enemy.attackState && enemy.isAttack)
        {
            stateMachine.ChangeState(enemy.attackState);
        }

        //if (Input.GetKeyDown(KeyCode.E) && stateMachine.currentState != enemy.skillState && enemy.isAttack && !enemy.anim.GetCurrentAnimatorStateInfo(0).IsName("SwordSkill"))
        //{
        //    stateMachine.ChangeState(enemy.skillState);
        //}

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
