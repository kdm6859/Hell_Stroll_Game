using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using static UnityEngine.Rendering.DebugUI.Table;

public class PlayerAttackState : PlayerGroundedState
{


    public PlayerAttackState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.CameraChange(CameraMode.BattleCamera);

        player.IsAttack = true;
        player.comboCount = 1;
        player.anim.SetInteger("ComboCount", player.comboCount);
        player.anim.SetInteger("AttackForm", player.attackFormNum);
        player.transform.rotation = Quaternion.Euler(player.transform.rotation.eulerAngles.x, player.currentPlayerCamera.transform.rotation.eulerAngles.y, player.transform.rotation.eulerAngles.z);

        //player.currentPlayerCamera.GetComponent<CinemachineVirtualCamera>().AddCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = 30f;
        ///player.currentPlayerCamera.GetComponent<CinemachineVirtualCamera>().AddCinemachineComponent<CinemachinePOV>().enabled = false;
    }

    public override void Update()
    {
        base.Update();

        if (player.stateMachine.currentState.GetType() != this.GetType())
            return;
        ////캐릭터 방향 설정
        //dirVec = new Vector3(xInput, 0, zInput);
        //dirVec.Normalize();
        //Quaternion rot = Quaternion.LookRotation(dirVec);
        //if (dirVec != Vector3.zero)
        //    player.transform.rotation = Quaternion.Euler(player.transform.rotation.eulerAngles.x, player.playerCamera.transform.rotation.eulerAngles.y + rot.eulerAngles.y, player.transform.rotation.eulerAngles.z);



        if (Input.GetButtonDown("Fire1") && !player.IsAttack && player.comboCount < player.GetCurrentAttackForm().AttackScript.AttackFormat.comboMaxCount && !player.anim.GetCurrentAnimatorStateInfo(0).IsName("SwordAttack3") && !player.anim.GetCurrentAnimatorStateInfo(0).IsName("MagicAttack3"))
        {
            player.IsAttack = true;
            player.comboCount++;
            player.anim.SetInteger("ComboCount", player.comboCount);
            player.transform.rotation = Quaternion.Euler(player.transform.rotation.eulerAngles.x, player.currentPlayerCamera.transform.rotation.eulerAngles.y, player.transform.rotation.eulerAngles.z);
        }


    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Exit()
    {
        base.Exit();

        player.comboCount = 0;
        player.IsAttack = false;

        player.CameraChange(CameraMode.BasicCamera);
    }


}
