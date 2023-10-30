using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(Player player, PlayerStateMachine stateMachine, string animBoolName)
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

        if (player.stateMachine.currentState.GetType() != this.GetType())
            return;

        //if (player.IsWallDetected())
        //{
        //    stateMachine.ChangeState(player.wallSliderState);
        //}

        //캐릭터 방향 설정
        dirVec = new Vector3(xInput, 0, zInput);
        dirVec.Normalize();
        Quaternion rot = Quaternion.LookRotation(dirVec);
        if (dirVec != Vector3.zero)
            player.transform.rotation = Quaternion.Euler(player.transform.rotation.eulerAngles.x, player.currentPlayerCamera.transform.rotation.eulerAngles.y + rot.eulerAngles.y, player.transform.rotation.eulerAngles.z);

        float xInputAbs = Mathf.Abs(xInput);
        float zInputAbs = Mathf.Abs(zInput);

        if(player.isCollision)
        {
            //moveVec = player.contectNormal + player.transform.forward;
            moveVec = player.MovingResult(player.transform.forward, player.contectNormal) * (Input.GetKey(KeyCode.LeftShift) ? player.runSpeed : player.moveSpeed) * (xInputAbs > zInputAbs ? xInputAbs : zInputAbs);
            //moveVec = player.MovingResult(player.transform.forward, player.wallHitInfo[0].normal) * player.moveSpeed * (xInputAbs > zInputAbs ? xInputAbs : zInputAbs);
            //Debug.Log(moveVec.magnitude + "Jump \nplayer.contectNormal : " + player.contectNormal + "\nplayer.transform.forward : " + player.transform.forward +
            //    "\n" + moveVec);
        }
        else
        {
            moveVec = player.transform.forward * (Input.GetKey(KeyCode.LeftShift) ? player.runSpeed : player.moveSpeed) * (xInputAbs > zInputAbs ? xInputAbs : zInputAbs);
        }

        if (player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }

        //if (xInput != 0)
        //    player.SetVelocity(player.moveSpeed * 0.8f * xInput, rb.velocity.y);
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
