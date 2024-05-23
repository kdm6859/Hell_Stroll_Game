using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum DistanceCheckType
{
    Chase,
    Attack
}

public class Enemy : Entity, IUnitStats, IDamageable, IAttackable
{

    [Header("Attack Info")]
    AttackFormManager_Enemy attackFormManager_enemy;
    AttackForm currentAttackForm;
    public int attackFormNum = 0;
    public GameObject[] weapons;
    public Transform[] firePoints;
    //public GameObject attackPrefab;
    //public GameObject skillPrefab;

    [Header("Status Info")]
    protected int maxHealthPoint;
    public int MaxHealthPoint { get { return maxHealthPoint; } set { maxHealthPoint = value; } }
    protected int healthPoint = 1000;
    public int HealthPoint { get { return healthPoint; } set { healthPoint = value; } }
    protected int maxManaPoint = 100;
    public int MaxManaPoint { get { return maxManaPoint; } set { maxManaPoint = value; } }
    protected int manaPoint = 100;
    public int ManaPoint { get { return manaPoint; } set { manaPoint = value; } }
    protected int attackPower = 10;
    public int AttackPower { get { return attackPower; } set { attackPower = value; } }
    protected int defense = 0;
    public int Defense { get { return defense; } set { defense = value; } }
    protected int strength = 1;
    public int Strength { get { return strength; } set { strength = value; } }
    protected int dexterity = 1;
    public int Dexterity { get { return dexterity; } set { dexterity = value; } }
    protected int endurance = 1;
    public int Endurance { get { return endurance; } set { endurance = value; } }
    protected int experience = 1;
    public int Experience { get { return experience; } set { experience = value; } }


    public bool isCollision { get; set; } = false;
    [Header("Rest")]
    public Vector3 contectNormal = Vector3.zero;

    public bool isAttack = true;
    public bool IsAttack { get { return isAttack; } set { isAttack = value; } }

    public int comboCount = 0;

    public NavMeshAgent agent;
    [SerializeField] float chaseDistance = 10f;
    [SerializeField] float attackDistance = 3f;
    public float ChaseDistance { get { return chaseDistance; } }
    public float AttackDistance { get { return attackDistance; } }

    //[SerializeField] GameObject aura;
    //[SerializeField] GameObject magicCircle;

    //public EnemyMovePoint moveArea;
    //public Vector3[] movePoint { get; private set; }
    public MovePoints moveArea { get; set; }
    bool isSetArea = false;
    public int movePointNum { get; set; } = 0;
    public int movePointNum_Max { get; private set; } = 0;


    #region State
    public EnemyStateMachine stateMachine { get; private set; }

    public EnemyIdleState idleState { get; private set; }
    public EnemyMoveState moveState { get; private set; }
    public EnemyRunState runState { get; private set; }
    //public EnemyJumpState jumpState { get; private set; }
    //public EnemyAirState airState { get; private set; }
    //public EnemyDashState dashState { get; private set; }
    public EnemyAttackState attackState { get; private set; }
    //public EnemySkillState skillState { get; private set; }
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

        stateMachine = new EnemyStateMachine();

        idleState = new EnemyIdleState(this, stateMachine, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, "Move");
        runState = new EnemyRunState(this, stateMachine, "Run");
        //jumpState = new EnemyJumpState(this, stateMachine, "Jump");
        //airState = new EnemyAirState(this, stateMachine, "Fall");
        //dashState = new EnemyDashState(this, stateMachine, "Dash");
        attackState = new EnemyAttackState(this, stateMachine, "Attack");
        //skillState = new EnemySkillState(this, stateMachine, "Skill");
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


        attackFormManager_enemy = AttackFormManager_Enemy.instance;
        currentAttackForm = attackFormManager_enemy.SetAttackForm(attackFormNum);
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }
        weapons[attackFormNum].SetActive(true);

        //skill = SkillManager.instance;

        stateMachine.Initialize(idleState);



        //if (!EnemyMovePointManager.instance.SetArea(out moveArea))
        //    Destroy(gameObject);

        //movePointNum_Max = moveArea.MovePoints.Length;
        //movePoint = new Vector3[movePointNum_Max];

        //for (int i = 0; i < movePointNum_Max; i++)
        //{
        //    movePoint[i] = moveArea.MovePoints[i].position;
        //}
    }

    //Enemy 이동 경로 설정
    public void SetArea(MovePoints pointPositions)
    {
        moveArea = pointPositions;
        movePointNum_Max = moveArea.PointPositions.Count;
        isSetArea = true;
    }

    protected override void Update()
    {
        //이동 경로 설정 시 동작
        if (!isSetArea)
            return;

        base.Update();

        stateMachine.currentState.Update();

        //어택폼 전환
        if (Input.GetKeyDown(KeyCode.Q) && Input.GetKeyDown(KeyCode.P))
        {
            weapons[attackFormNum].SetActive(false);

            attackFormNum++;
            if (attackFormNum >= attackFormManager_enemy.AttackFormsMaxNum())
            {
                attackFormNum = 0;
            }

            weapons[attackFormNum].SetActive(true);
            currentAttackForm = attackFormManager_enemy.SetAttackForm(attackFormNum);
        }

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        stateMachine.currentState.FixedUpdate();

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

    public bool DistanceCheck(DistanceCheckType checkType)
    {
        float checkDistance = 0f;

        if (checkType == DistanceCheckType.Chase)
            checkDistance = chaseDistance;
        else if (checkType == DistanceCheckType.Attack)
            checkDistance = attackDistance;

        if (Vector3.Distance(transform.position, GameManager.instance.player.transform.position) <= checkDistance)
            return true;
        else
            return false;
    }

    public AttackForm CurrentAttackForm()
    {
        return currentAttackForm;
    }

    public void Attack()
    {
        currentAttackForm.Attack(transform, firePoints[attackFormNum], comboCount, AttackPower);

        //Instantiate(attackPrefab, firePoint.position, transform.rotation);
    }

    public void NextAttackInputPossible()
    {
        IsAttack = true;
    }

    public void AttackEnd()
    {
        if (comboCount >= currentAttackForm.AttackScript.AttackFormat.comboMaxCount) //최대 콤보에 도달하면
        {
            if (DistanceCheck(DistanceCheckType.Attack))
            {
                IsAttack = false;
                comboCount = 1;
                anim.SetInteger("ComboCount", comboCount);
                transform.LookAt(GameManager.instance.player.transform.position);
            }
            else
                stateMachine.ChangeState(idleState);
        }
        else if (IsAttack) //콤보 중간에 공격입력을 멈추면
        {
            stateMachine.ChangeState(idleState);
        }
    }

    

    public void Die()
    {
        Debug.Log("적 죽음");
        Destroy(gameObject);
    }

    public void RestoreHealth()
    {
        
    }
}

