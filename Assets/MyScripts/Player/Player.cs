using System;
using System.Collections;
using System.Collections.Generic;
using TriangleNet;
using UnityEngine;

public enum CameraMode
{
    BasicCamera = 0,
    BattleCamera = 1
}

[Serializable]
public class CharacterAttackInfo
{
    public GameObject weapon;
    public Transform firePoint;
    public Transform[] skillEffectTransforms;
}

public class Player : Entity, IUnitStats, IDamageable, IAttackable
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
    [SerializeField] AttackFormManager attackFormManager;
    AttackForm currentAttackForm;
    public int attackFormNum = 0;
    public CharacterAttackInfo[] characterAttackInfos;
    public GameObject[] weapons;
    public Transform[] firePoints;
    //public GameObject attackPrefab;
    //public GameObject skillPrefab;


    [Header("Status Info")]
    [SerializeField]
    protected int maxHealthPoint = 1000;
    public int MaxHealthPoint { get { return maxHealthPoint; } set { maxHealthPoint = value; } }
    [SerializeField]
    protected int healthPoint;
    public int HealthPoint { get { return healthPoint; } set { healthPoint = value; } }
    [SerializeField]
    protected int maxManaPoint = 100;
    public int MaxManaPoint { get { return maxManaPoint; } set { maxManaPoint = value; } }
    [SerializeField]
    protected int manaPoint;
    public int ManaPoint { get { return manaPoint; } set { manaPoint = value; } }
    [SerializeField]
    protected int attackPower = 10;
    public int AttackPower { get { return attackPower; } set { attackPower = value; } }
    [SerializeField]
    protected int defense = 0;
    public int Defense { get { return defense; } set { defense = value; } }
    [SerializeField]
    protected int strength = 1;
    public int Strength { get { return strength; } set { strength = value; } }
    [SerializeField]
    protected int dexterity = 1;
    public int Dexterity { get { return dexterity; } set { dexterity = value; } }
    [SerializeField]
    protected int endurance = 1;
    public int Endurance { get { return endurance; } set { endurance = value; } }
    [SerializeField]
    protected int experience = 1;
    public int Experience { get { return experience; } set { experience = value; } }


    [Header("Rest")]
    public bool isWallCollision = false;
    public Vector3 contectNormal = Vector3.zero;

    public bool isAttack = false;
    public bool IsAttack { get { return isAttack; } set { isAttack = value; } }
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

        //플레이어 카메라 초기화
        for (int i = 0; i < playerCameras.Length; i++)
        {
            playerCameras[i].SetActive(false);
        }
        currentPlayerCamera = playerCameras[(int)CameraMode.BasicCamera];
        currentPlayerCamera.SetActive(true);

        //어택폼 초기화
        //attackFormManager = AttackFormManager.instance;
        currentAttackForm = attackFormManager.GetAttackForm(attackFormNum);
        Debug.Log(characterAttackInfos.Length);
        for (int i = 0; i < attackFormManager.GetAttackFormsMaxNum(); i++)
        {
            characterAttackInfos[i].weapon.SetActive(false);
            if (characterAttackInfos[i].skillEffectTransforms.Length != 0)
            {
                Debug.Log(i + "??");
                attackFormManager.SkillInitialized(i, characterAttackInfos[i].skillEffectTransforms);
            }
        }
        characterAttackInfos[attackFormNum].weapon.SetActive(true);

        //skill = SkillManager.instance;

        //스테이트 머신 초기화
        stateMachine.Initialize(idleState);

        //마우스 커서 숨김
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //체력 마나 초기화
        HealthPoint = MaxHealthPoint;
        ManaPoint = MaxManaPoint;

    }

    protected override void Update()
    {
        base.Update();

        //현재 스테이트 업데이트
        stateMachine.currentState.Update();

        //마우스 커서 숨김 (임시)
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
            characterAttackInfos[attackFormNum].weapon.SetActive(false);

            attackFormNum++;
            if (attackFormNum >= attackFormManager.GetAttackFormsMaxNum())
            {
                attackFormNum = 0;
            }

            characterAttackInfos[attackFormNum].weapon.SetActive(true);
            currentAttackForm = attackFormManager.GetAttackForm(attackFormNum);
        }

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        //현재 스테이트 Fixed업데이트
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
        //콜리전 충돌 중인데 땅이 아닐 때
        if (!IsGroundDetected())// || !IsGroundCollision(collision))
        {
            contectNormal = collision.contacts[0].normal;
            isWallCollision = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isWallCollision = false;
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

    public AttackForm GetCurrentAttackForm()
    {
        return currentAttackForm;
    }

    public void Attack() //애니메이션 이벤트
    {
        currentAttackForm.Attack(transform, characterAttackInfos[attackFormNum].firePoint, comboCount, AttackPower);

        //Instantiate(attackPrefab, firePoint.position, transform.rotation);
    }

    public void NextAttackInputPossible() //애니메이션 이벤트
    {
        IsAttack = false;
    }

    public void AttackEnd() //애니메이션 이벤트, 공격 애니메이션 종료시점에서 다음 콤보공격 가능여부 확인 후 idleState로 전환
    {
        //Debug.Log("AttackEnd");

        //최대 콤보에 도달하거나 콤보 중간에 공격입력을 멈추면
        if (comboCount >= currentAttackForm.AttackScript.AttackFormat.comboMaxCount || !IsAttack)
        {
            stateMachine.ChangeState(idleState);
        }

    }

    public void MagicSkillEnd() //애니메이션 이벤트
    {
        if (currentAttackForm.GetType() == typeof(MagicForm))
        {
            ((MagicForm)currentAttackForm).IsSkill_False();
        }
    }

    //public void SwordSkillAura() //애니메이션 이벤트
    //{
    //    if (currentAttackForm.GetType() == typeof(SwordForm))
    //    {
    //        ((SwordForm)currentAttackForm).SkillCharging(aura);
    //    }
    //}

    public void SkillStart() //애니메이션 이벤트
    {
        currentAttackForm.SkillStart();
    }

    public void Skill() //애니메이션 이벤트
    {
        currentAttackForm.Skill(transform, characterAttackInfos[attackFormNum].firePoint, AttackPower);
    }

    public void SkillEnd() //애니메이션 이벤트
    {
        currentAttackForm.SkillEnd();

        //Idle로 상태 전환
        stateMachine.ChangeState(idleState);
    }

    public void TakeDamage(int Damage)
    {
        //base.TakeDamage(Damage);

        HealthPoint -= Damage;

        if (HealthPoint <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("플레이어 죽음");
    }

    public void RestoreHealth()
    {
        HealthPoint = MaxHealthPoint;
    }
}

