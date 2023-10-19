using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Flags]
enum asd
{
    None = 0,
    a = 1 << 0,
    b = 1 << 1,
    c = 1 << 2,
    d = 1 << 3,
    Everything = ~None,

    x = a | b
}
public class PlayerMoveState : PlayerGroundedState
{
    Vector3 dirVec;

    public PlayerMoveState(Player player, PlayerStateMachine stateMachine, string animBoolName)
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
        //moveVec = player.transform.forward * zInput * player.moveSpeed;

        //캐릭터 방향 설정
        dirVec = new Vector3(xInput, 0, zInput);
        dirVec.Normalize();
        Quaternion rot = Quaternion.LookRotation(dirVec);
        if (dirVec != Vector3.zero)
            player.transform.rotation = Quaternion.Euler(player.transform.rotation.eulerAngles.x, player.playerCamera.transform.rotation.eulerAngles.y + rot.eulerAngles.y, player.transform.rotation.eulerAngles.z);



        if (xInput == 0 && zInput == 0)//|| player.IsWallDetected())
        {
            player.stateMachine.ChangeState(player.idleState);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        
        Vector3 moveVec = player.transform.forward * player.moveSpeed;
        player.SetVelocity(new Vector3(moveVec.x, rb.velocity.y, moveVec.z));

    }

    

    public override void Exit()
    {
        base.Exit();
    }


}
