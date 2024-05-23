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

        //�÷��̾� ī�޶� �ʱ�ȭ
        for (int i = 0; i < playerCameras.Length; i++)
        {
            playerCameras[i].SetActive(false);
        }
        currentPlayerCamera = playerCameras[(int)CameraMode.BasicCamera];
        currentPlayerCamera.SetActive(true);

        //������ �ʱ�ȭ
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

        //������Ʈ �ӽ� �ʱ�ȭ
        stateMachine.Initialize(idleState);

        //���콺 Ŀ�� ����
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //ü�� ���� �ʱ�ȭ
        HealthPoint = MaxHealthPoint;
        ManaPoint = MaxManaPoint;

    }

    protected override void Update()
    {
        base.Update();

        //���� ������Ʈ ������Ʈ
        stateMachine.currentState.Update();

        //���콺 Ŀ�� ���� (�ӽ�)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = !Cursor.visible;
            if (Cursor.visible)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }
        //CheckForDashInput();

        //������ ��ȯ
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

        //���� ������Ʈ Fixed������Ʈ
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
        //�ݸ��� �浹 ���ε� ���� �ƴ� ��
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

    public void CameraChange(CameraMode cameraMode)
    {
        //������ ī�޶� rotation�� ����
        Quaternion tempCamRot = currentPlayerCamera.transform.rotation;

        //ī�޶� ��ȯ
        currentPlayerCamera.SetActive(false);
        currentPlayerCamera = playerCameras[(int)cameraMode];
        currentPlayerCamera.SetActive(true);

        //������ ī�޶� rotation�� ���� ī�޶� ����
        currentPlayerCamera.transform.rotation = tempCamRot;
    }

    public AttackForm GetCurrentAttackForm()
    {
        return currentAttackForm;
    }

    public void Attack() //�ִϸ��̼� �̺�Ʈ
    {
        currentAttackForm.Attack(transform, characterAttackInfos[attackFormNum].firePoint, comboCount, AttackPower);

        //Instantiate(attackPrefab, firePoint.position, transform.rotation);
    }

    public void NextAttackInputPossible() //�ִϸ��̼� �̺�Ʈ
    {
        IsAttack = false;
    }

    public void AttackEnd() //�ִϸ��̼� �̺�Ʈ, ���� �ִϸ��̼� ����������� ���� �޺����� ���ɿ��� Ȯ�� �� idleState�� ��ȯ
    {
        //Debug.Log("AttackEnd");

        //�ִ� �޺��� �����ϰų� �޺� �߰��� �����Է��� ���߸�
        if (comboCount >= currentAttackForm.AttackScript.AttackFormat.comboMaxCount || !IsAttack)
        {
            stateMachine.ChangeState(idleState);
        }

    }

    public void MagicSkillEnd() //�ִϸ��̼� �̺�Ʈ
    {
        if (currentAttackForm.GetType() == typeof(MagicForm))
        {
            ((MagicForm)currentAttackForm).IsSkill_False();
        }
    }

    //public void SwordSkillAura() //�ִϸ��̼� �̺�Ʈ
    //{
    //    if (currentAttackForm.GetType() == typeof(SwordForm))
    //    {
    //        ((SwordForm)currentAttackForm).SkillCharging(aura);
    //    }
    //}

    public void SkillStart() //�ִϸ��̼� �̺�Ʈ
    {
        currentAttackForm.SkillStart();
    }

    public void Skill() //�ִϸ��̼� �̺�Ʈ
    {
        currentAttackForm.Skill(transform, characterAttackInfos[attackFormNum].firePoint, AttackPower);
    }

    public void SkillEnd() //�ִϸ��̼� �̺�Ʈ
    {
        currentAttackForm.SkillEnd();

        //Idle�� ���� ��ȯ
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
        Debug.Log("�÷��̾� ����");
    }

    public void RestoreHealth()
    {
        HealthPoint = MaxHealthPoint;
    }
}

