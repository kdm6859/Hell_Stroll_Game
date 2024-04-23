using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss1 : Enemy
{
    [Header("Attack Info")]
    AttackFormManager_Enemy attackFormManager_enemy;
    AttackForm currentAttackForm;
    public int attackFormNum = 0;
    public GameObject[] weapons;
    public Transform[] firePoints;
    //public GameObject attackPrefab;
    //public GameObject skillPrefab;

    public bool isCollision { get; set; } = false;
    [Header("Rest")]
    public Vector3 contectNormal = Vector3.zero;

    public bool isAttack = true;
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

    //public override void TakeDamage(int attackPower)
    //{
    //    base.TakeDamage(attackPower);

    //    HealthPoint -= attackPower;
    //    if (HealthPoint <= 0)
    //    {
    //        Debug.Log("적 죽음");
    //        Destroy(gameObject);
    //    }
    //}

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

    public void IsAttack_True()
    {
        isAttack = true;
    }

    public void AttackEnd()
    {
        if (comboCount >= currentAttackForm.attackFormData.comboMaxCount) //최대 콤보에 도달하면
        {
            if (DistanceCheck(DistanceCheckType.Attack))
            {
                isAttack = false;
                comboCount = 1;
                anim.SetInteger("ComboCount", comboCount);
                transform.LookAt(GameManager.instance.player.transform.position);
            }
            else
                stateMachine.ChangeState(idleState);
        }
        else if (isAttack) //콤보 중간에 공격입력을 멈추면
        {
            stateMachine.ChangeState(idleState);
        }
    }

    public void Attack()
    {
        currentAttackForm.Attack(transform, firePoints[attackFormNum], comboCount, AttackPower);

        //Instantiate(attackPrefab, firePoint.position, transform.rotation);
    }
}
