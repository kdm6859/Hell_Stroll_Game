using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    //[Header("Attack Info")]
    //public Vector2[] attackMovement;
    //public float counterAttackDuration = 0.2f;

    public GameObject playerCamera;

    public bool isBusy { get; private set; }

    [Header("Move Info")]
    public float moveSpeed = 6f;
    public float runSpeed = 10f;
    public float rotSpeed = 2f;
    public float jumpForce = 12f;
    public float swordReturnImpact;

    [Header("Dash Info")]
    //public float dashUsageTimer;
    //[SerializeField] float dashCooldown;
    public float dashSpeed;
    public float dashDuration;
    public float dashDir { get; private set; }


    //public SkillManager skill { get; private set; }
    //public GameObject sword { get; private set; }


    #region State
    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    //public PlayerWallSliderState wallSliderState { get; private set; }
    //public PlayerWallJumpState wallJumpState { get; private set; }
    //public PlayerPrimaryAttackState primaryAttackState { get; private set; }
    //public PlayerCounterAttackState counterAttackState { get; private set; }
    //public PlayerAimSwordState aimSwordState { get; private set; }
    //public PlayerCatchSwordState catchSwordState { get; private set; }
    #endregion





    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        //wallSliderState = new PlayerWallSliderState(this, stateMachine, "WallSlide");
        //wallJumpState = new PlayerWallJumpState(this, stateMachine, "Jump");

        //primaryAttackState = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        //counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");

        //aimSwordState = new PlayerAimSwordState(this, stateMachine, "AimSword");
        //catchSwordState = new PlayerCatchSwordState(this, stateMachine, "CatchSword");

    }

    protected override void Start()
    {
        base.Start();

        //skill = SkillManager.instance;

        stateMachine.Initialize(idleState);

    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();

        //CheckForDashInput();

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        stateMachine.currentState.FixedUpdate();

    }

    //public void AssignNewSword(GameObject _newSword)
    //{
    //    sword = _newSword;
    //}

    //public void CatchTheSword()
    //{
    //    stateMachine.ChangeState(catchSwordState);

    //    Destroy(sword);
    //}


    public IEnumerator BusyFor(float seconds)
    {
        isBusy = true;
        Debug.Log("�ٻ�");
        yield return new WaitForSeconds(seconds);
        Debug.Log("�ȹٻ�");
        isBusy = false;
    }

    public void AnimationTrigger()
        => stateMachine.currentState.AnimationFinishTrigger();

    private void CheckForDashInput()
    {
        //if (IsWallDetected())
        //    return;



        if (Input.GetKeyDown(KeyCode.LeftShift))// && SkillManager.instance.dash.CanUseSkill())
        {


            dashDir = Input.GetAxisRaw("Horizontal");

            //if (dashDir == 0)
            //    dashDir = facingDir;

            stateMachine.ChangeState(dashState);
        }

    }






}

