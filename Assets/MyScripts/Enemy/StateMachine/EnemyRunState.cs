using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyRunState : EnemyGroundedState
{
    public EnemyRunState(Enemy enemy, EnemyStateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.stoppingDistance = enemy.AttackDistance;
        enemy.agent.speed = 9f;
    }

    public override void Update()
    {
        base.Update();

        if (enemy.stateMachine.currentState.GetType() != this.GetType())
            return;

        enemy.agent.SetDestination(GameManager.instance.player.transform.position);

        if (!enemy.DistanceCheck(DistanceCheckType.Chase))
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
    }
}
