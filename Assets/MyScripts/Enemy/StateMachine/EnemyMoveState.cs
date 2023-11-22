using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveState : EnemyGroundedState
{


    public EnemyMoveState(Enemy enemy, EnemyStateMachine stateMachine, string animBoolName)
        : base(enemy, stateMachine, animBoolName)
    {

    }


    public override void Enter()
    {
        base.Enter();

        enemy.agent.stoppingDistance = 0f;
        enemy.agent.speed = 3.5f;
        enemy.agent.SetDestination(enemy.movePoint[enemy.movePointNum]);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.stateMachine.currentState.GetType() != this.GetType())
            return;



        ////캐릭터 방향 설정
        //enemy.transform.LookAt(movePoint);

        if (enemy.DistanceCheck(DistanceCheckType.Chase))
        {
            enemy.stateMachine.ChangeState(enemy.runState);
        }
        else if (enemy.agent.remainingDistance <= 0.1f)
        {
            enemy.stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

    }



    public override void Exit()
    {
        base.Exit();

        enemy.movePointNum++;
        if (enemy.movePointNum >= enemy.movePointNum_Max)
        {
            enemy.movePointNum = 0;
        }
    }
}
