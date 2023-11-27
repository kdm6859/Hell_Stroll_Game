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
        enemy.agent.SetDestination(enemy.moveArea.PointPositions[enemy.movePointNum]);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.stateMachine.currentState.GetType() != this.GetType())
            return;



        ////ĳ���� ���� ����
        //enemy.transform.LookAt(movePoint);
        Debug.Log(enemy.agent.remainingDistance);
        if (enemy.DistanceCheck(DistanceCheckType.Chase))
        {
            enemy.stateMachine.ChangeState(enemy.runState);
        }
        else if (enemy.agent.remainingDistance <= 0.1f)
        {
            Debug.Log("���� �Ÿ� : " + enemy.agent.remainingDistance);
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
