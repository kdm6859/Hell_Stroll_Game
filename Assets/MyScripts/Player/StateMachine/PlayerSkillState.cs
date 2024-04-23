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

        //�ٸ� ������Ʈ�� �Ѿ�� �� ������ ������Ʈ�� Update���� ����Ǵ� ���� �������� ���� ������Ʈ�� �������� Update�� Ÿ��(Ŭ����)�� ���Ѵ�.
        if (player.stateMachine.currentState.GetType() != this.GetType())
            return;


        if (player.attackFormNum == 0) //Magic_Form �� ��
        {
            if (Input.GetKey(KeyCode.E))
            {
                //ĳ���� ���� ���� (ī�޶� ���� �ٶ󺸰�)
                dirVec = Camera.main.transform.forward;
                dirVec.Normalize();
                Quaternion rot = Quaternion.LookRotation(dirVec);
                if (dirVec != Vector3.zero)
                    player.transform.rotation = Quaternion.Euler(0, rot.eulerAngles.y, 0);
                //���� ���� �Ҹ�
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
