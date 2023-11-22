using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyGroundedState
{

    public EnemyAttackState(Enemy enemy, EnemyStateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.isAttack = false;
        enemy.comboCount = 1;
        enemy.anim.SetInteger("ComboCount", enemy.comboCount);
        enemy.anim.SetInteger("AttackForm", enemy.attackFormNum);
        enemy.transform.LookAt(GameManager.instance.player.transform.position);

        enemy.agent.stoppingDistance = enemy.ChaseDistance;
        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;
        enemy.agent.speed = 9f;
    }

    public override void Update()
    {
        base.Update();

        if (enemy.stateMachine.currentState.GetType() != this.GetType())
            return;


        if (enemy.DistanceCheck(DistanceCheckType.Attack) && enemy.isAttack && enemy.comboCount < enemy.CurrentAttackForm().attackFormData.comboMaxCount && !enemy.anim.GetCurrentAnimatorStateInfo(0).IsName("SwordAttack3") && !enemy.anim.GetCurrentAnimatorStateInfo(0).IsName("MagicAttack3"))
        {
            enemy.isAttack = false;
            enemy.comboCount++;
            enemy.anim.SetInteger("ComboCount", enemy.comboCount);
            enemy.transform.LookAt(GameManager.instance.player.transform.position);
        }
        //else if(enemy.comboCount == enemy.CurrentAttackForm().attackFormData.comboMaxCount)
        //{
        //    Debug.Log("°ø°Ý ³¡?");
        //    enemy.stateMachine.ChangeState(enemy.idleState);
        //}


    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Exit()
    {
        base.Exit();

        enemy.comboCount = 0;
        enemy.isAttack = true;

        enemy.agent.isStopped = false;
    }

}
