using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerGroundedState
{
    public PlayerRunState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
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

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            player.stateMachine.ChangeState(player.moveState);
        }
        Debug.Log("run");
        //캐릭터 방향 설정
        dirVec = new Vector3(xInput, 0, zInput);
        dirVec.Normalize();
        Quaternion rot = Quaternion.LookRotation(dirVec);
        if (dirVec != Vector3.zero)
            player.transform.rotation = Quaternion.Euler(player.transform.rotation.eulerAngles.x, player.currentPlayerCamera.transform.rotation.eulerAngles.y + rot.eulerAngles.y, player.transform.rotation.eulerAngles.z);

        float xInputAbs = Mathf.Abs(xInput);
        float zInputAbs = Mathf.Abs(zInput);


        if (player.isCollision)
        {
            //moveVec = player.contectNormal + player.transform.forward;
            moveVec = player.MovingResult(player.transform.forward, player.contectNormal) * player.moveSpeed * (xInputAbs > zInputAbs ? xInputAbs : zInputAbs);
            //moveVec = player.MovingResult(player.transform.forward, player.wallHitInfo[0].normal) * player.runSpeed * (xInputAbs > zInputAbs ? xInputAbs : zInputAbs);
            //Debug.Log(moveVec.magnitude + "Move \nplayer.contectNormal : " + player.contectNormal + "\nplayer.transform.forward : " + player.transform.forward +
            //    "\n" + moveVec);
        }
        else
        {
            moveVec = player.transform.forward * player.runSpeed * (xInputAbs > zInputAbs ? xInputAbs : zInputAbs);
        }


        if (xInput == 0 && zInput == 0)//|| player.IsWallDetected())
        {
            moveTimer += Time.deltaTime;

            if (moveTimer > 0.5f)
                player.stateMachine.ChangeState(player.idleState);
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
