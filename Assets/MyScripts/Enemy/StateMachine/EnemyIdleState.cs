using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class EnemyIdleState : EnemyGroundedState
{
    public EnemyIdleState(Enemy enemy, EnemyStateMachine stateMachine, string animBoolName)
        : base(enemy, stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        moveTimer = 0f;

        //enemy.agent.stoppingDistance = 0f;
        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;
        enemy.agent.speed = 3.5f;
    }

    public override void Update()
    {
        base.Update();

        if (enemy.stateMachine.currentState.GetType() != this.GetType())
            return;


        moveTimer += Time.deltaTime;

        if (enemy.DistanceCheck(DistanceCheckType.Chase))
        {
            enemy.stateMachine.ChangeState(enemy.runState);
        }
        else if (moveTimer >= 1f)
        {
            enemy.stateMachine.ChangeState(enemy.moveState);
        }

        

    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();


    }

    public override void Exit()
    {
        base.Exit();

        enemy.agent.isStopped = false;
    }
}
