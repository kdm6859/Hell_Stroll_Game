using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected Player player;

    protected Rigidbody rb;

    protected float xInput;
    protected float zInput;
    private string animBoolName;

    protected float stateTimer;

    protected bool triggerCalled;

    public PlayerState(Player player, PlayerStateMachine stateMachine, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        //Debug.Log("���� �Լ� " + animBoolName);

        //�ִϸ��̼� ��ȯ
        player.anim.SetBool(animBoolName, true);

        triggerCalled = false;

        rb = player.rb;
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;

        xInput = Input.GetAxis("Horizontal");
        zInput = Input.GetAxis("Vertical");

        

        //Debug.Log(animBoolName);
    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void Exit()
    {
        //Debug.Log("����Ʈ �Լ� " + animBoolName);
        player.anim.SetBool(animBoolName, false);
    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }
}
