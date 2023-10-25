using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraMode
{
    BasicCamera = 0,
    BattleCamera = 1
}

public class Player : Entity
{
    //[Header("Attack Info")]
    //public Vector2[] attackMovement;
    //public float counterAttackDuration = 0.2f;

    [Header("Camera Info")]
    public GameObject[] playerCameras;
    public GameObject currentPlayerCamera;

    public bool isBusy { get; private set; }

    [Header("Move Info")]
    public float moveSpeed = 6f;
    public float runSpeed = 10f;
    public float rotSpeed = 2f;
    public float jumpForce = 12f;

    [Header("Status Info")]
    public int hp = 1000;
    public int mp = 100;

    [Header("Dash Info")]
    //public float dashUsageTimer;
    //[SerializeField] float dashCooldown;
    public float dashSpeed;
    public float dashDuration;
    public float dashDir { get; private set; }

    [Header("Attack Info")]
    public Transform firePoint;
    public GameObject attackPrefab;
    public GameObject skillPrefab;

    [Header("Rest")]
    public bool isCollision = false;
    public Vector3 contectNormal = Vector3.zero;

    public bool isAttack = true;
    public int comboCount = 0;

    //public SkillManager skill { get; private set; }
    //public GameObject sword { get; private set; }


    #region State
    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerRunState runState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerAttackState attackState { get; private set; }
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
        runState = new PlayerRunState(this, stateMachine, "Run");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Fall");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        attackState = new PlayerAttackState(this, stateMachine, "Attack");
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

        for (int i = 0; i < playerCameras.Length; i++)
        {
            playerCameras[i].SetActive(false);
        }
        currentPlayerCamera = playerCameras[(int)CameraMode.BasicCamera];
        currentPlayerCamera.SetActive(true);

        //skill = SkillManager.instance;

        stateMachine.Initialize(idleState);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();

        //마우스 커서 숨김
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = !Cursor.visible;
            if (Cursor.visible)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }
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


    public Vector3 MovingResult(Vector3 Input, Vector3 normal)
    {
        Vector3 result;

        result = Input - Vector3.Dot(normal, Input) / Vector3.Dot(normal, normal) * normal;

        return result;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!IsGroundDetected() || !IsGroundCollision(collision))
        {
            contectNormal = collision.contacts[0].normal;
            isCollision = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isCollision = false;
    }


    public IEnumerator BusyFor(float seconds)
    {
        isBusy = true;
        Debug.Log("바쁨");
        yield return new WaitForSeconds(seconds);
        Debug.Log("안바쁨");
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

    public void CameraChange(CameraMode cameraMode)
    {
        //이전의 카메라 rotation값 저장
        Quaternion tempCamRot = currentPlayerCamera.transform.rotation;

        //카메라 전환
        currentPlayerCamera.SetActive(false);
        currentPlayerCamera = playerCameras[(int)cameraMode];
        currentPlayerCamera.SetActive(true);

        //이전의 카메라 rotation값 현재 카메라에 대입
        currentPlayerCamera.transform.rotation = tempCamRot;
    }


    public void IsAttack_True()
    {
        isAttack = true;
    }

    public void AttackEnd()
    {
        if (comboCount >= 3)
        {
            stateMachine.ChangeState(idleState);
        }
        else if (isAttack)
            stateMachine.ChangeState(idleState);
    }

    public void Attack()
    {
        Instantiate(attackPrefab, firePoint.position, transform.rotation);
    }

}

