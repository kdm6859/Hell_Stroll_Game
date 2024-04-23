using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillState : PlayerGroundedState
{
    public PlayerSkillState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.CameraChange(CameraMode.BattleCamera);
        player.IsAttack = true;
        player.transform.rotation = Quaternion.Euler(player.transform.rotation.eulerAngles.x, player.currentPlayerCamera.transform.rotation.eulerAngles.y, player.transform.rotation.eulerAngles.z);

        player.anim.SetInteger("AttackForm", player.attackFormNum);
        player.anim.SetBool("SkillKeyDown", true);
    }

    public override void Update()
    {
        base.Update();

        //다른 스테이트로 넘어갔을 때 기존의 스테이트의 Update문이 실행되는 것을 막기위해 현재 스테이트와 실행중인 Update의 타입(클래스)를 비교한다.
        if (player.stateMachine.currentState.GetType() != this.GetType())
            return;


        if (player.attackFormNum == 0) //Magic_Form 일 때
        {
            if (Input.GetKey(KeyCode.E))
            {
                //캐릭터 방향 설정 (카메라 방향 바라보게)
                dirVec = Camera.main.transform.forward;
                dirVec.Normalize();
                Quaternion rot = Quaternion.LookRotation(dirVec);
                if (dirVec != Vector3.zero)
                    player.transform.rotation = Quaternion.Euler(0, rot.eulerAngles.y, 0);
                //마나 지속 소모
            }
            else
            {
                player.anim.SetBool("SkillKeyDown", false);

                //player.CurrentAttackForm().

                //stateMachine.ChangeState(player.idleState);
            }
        }

    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Exit()
    {
        base.Exit();

        player.IsAttack = false;

        player.CameraChange(CameraMode.BasicCamera);
    }
}
