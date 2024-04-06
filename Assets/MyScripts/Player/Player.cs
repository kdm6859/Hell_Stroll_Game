using System.Collections;
using System.Collections.Generic;
using TriangleNet;
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

    [Header("Dash Info")]
    //public float dashUsageTimer;
    //[SerializeField] float dashCooldown;
    public float dashSpeed;
    public float dashDuration;
    public float dashDir { get; private set; }

    [Header("Attack Info")]
    AttackFormManager attackFormManager;
    AttackForm currentAttackForm;
    public int attackFormNum = 0;
    public GameObject[] weapons;
    public Transform[] firePoints;
    //public GameObject attackPrefab;
    //public GameObject skillPrefab;

    [Header("Rest")]
    public bool isCollision = false;
    public Vector3 contectNormal = Vector3.zero;

    public bool isAttack = true;
    public int comboCount = 0;

    [SerializeField] GameObject aura;
    [SerializeField] GameObject magicCircle;
    

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
    public PlayerSkillState skillState { get; private set; }
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
        skillState = new PlayerSkillState(this, stateMachine, "Skill");
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
        
        attackFormManager = AttackFormManager.instance;
        currentAttackForm = attackFormManager.SetAttackForm(attackFormNum);
        for(int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }
        weapons[attackFormNum].SetActive(true);

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

        //어택폼 전환
        if (Input.GetKeyDown(KeyCode.Q))
        {
            weapons[attackFormNum].SetActive(false);

            attackFormNum++;
            if(attackFormNum >= attackFormManager.AttackFormsMaxNum())
            {
                attackFormNum = 0;
            }

            weapons[attackFormNum].SetActive(true);
            currentAttackForm = attackFormManager.SetAttackForm(attackFormNum);
        }

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

    public AttackForm CurrentAttackForm()
    {
        return currentAttackForm;
    }

    public void IsAttack_True()
    {
        isAttack = true;
    }

    public void AttackEnd()
    {
        //Debug.Log("AttackEnd");
        if (comboCount >= currentAttackForm.attackFormData.comboMaxCount) //최대 콤보에 도달하면
        {
            stateMachine.ChangeState(idleState);
        }
        else if (isAttack) //콤보 중간에 공격입력을 멈추면
        {
            stateMachine.ChangeState(idleState);
        }
    }

    public void Attack()
    {
        currentAttackForm.Attack(transform, firePoints[attackFormNum], comboCount, attackPower);

        //Instantiate(attackPrefab, firePoint.position, transform.rotation);
    }

    public void Skill()
    {
        currentAttackForm.Skill(transform, firePoints[attackFormNum], attackPower);
    }

    public void MagicSkillEnd()
    {
        if (currentAttackForm.GetType() == typeof(MagicForm))
        {
            ((MagicForm)currentAttackForm).IsSkill_False();
        }
    }

    public void SwordSkillAura()
    {
        if (currentAttackForm.GetType() == typeof(SwordForm))
        {
            ((SwordForm)currentAttackForm).SkillCharging(aura);
        }
    }

    public void SkillEnd()
    {
        stateMachine.ChangeState(idleState);
    }

    public override void Damage(int attackPower)
    {
        base.Damage(attackPower);

        hp -= attackPower;
        if (hp <= 0)
        {
            Debug.Log("플레이어 죽음");
        }
    }
}

