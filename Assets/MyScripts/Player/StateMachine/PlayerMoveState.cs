using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.EventSystems;


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


    public PlayerMoveState(Player player, PlayerStateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {

    }


    public override void Enter()
    {
        base.Enter();

        moveTimer = 0;
    }

    public override void Update()
    {
        base.Update();

        if (player.stateMachine.currentState.GetType() != this.GetType())
            return;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            player.stateMachine.ChangeState(player.runState);
        }

        //캐릭터 방향 설정
        dirVec = new Vector3(xInput, 0, zInput);
        dirVec.Normalize();
        Quaternion rot = Quaternion.LookRotation(dirVec);
        if (dirVec != Vector3.zero)
            player.transform.rotation = Quaternion.Euler(player.transform.rotation.eulerAngles.x, player.currentPlayerCamera.transform.rotation.eulerAngles.y + rot.eulerAngles.y, player.transform.rotation.eulerAngles.z);

        float xInputAbs = Mathf.Abs(xInput);
        float zInputAbs = Mathf.Abs(zInput);

        if (player.isWallCollision) //벽에 부딛힐 경우
        {
            //moveVec = player.contectNormal + player.transform.forward;
            moveVec = player.MovingResult(player.transform.forward, player.contectNormal) * player.MoveSpeed * (xInputAbs > zInputAbs ? xInputAbs : zInputAbs);
            //moveVec = player.MovingResult(player.transform.forward, player.wallHitInfo[0].normal) * player.moveSpeed * (xInputAbs > zInputAbs ? xInputAbs : zInputAbs);
            //Debug.Log(moveVec.magnitude + "Move \nplayer.contectNormal : " + player.contectNormal + "\nplayer.transform.forward : " + player.transform.forward +
            //    "\n" + moveVec);
        }
        else
        {
            moveVec = player.transform.forward * player.MoveSpeed * (xInputAbs > zInputAbs ? xInputAbs : zInputAbs);
        }

        //moveVec = player.transform.forward * player.moveSpeed * (xInputAbs > zInputAbs ? xInputAbs : zInputAbs);


        if (xInput == 0 && zInput == 0)//|| player.IsWallDetected())
        {
            moveTimer += Time.deltaTime;

            if (moveTimer > 0.1f)
            {
                player.stateMachine.ChangeState(player.idleState);
            }
        }
                
        else
        {
            moveTimer = 0;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        player.SetVelocity(new Vector3(moveVec.x, rb.velocity.y, moveVec.z));

    }



    public override void Exit()
    {
        base.Exit();
    }


}
