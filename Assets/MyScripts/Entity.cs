using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region component
    public Animator anim { get; private set; }
    public Rigidbody rb { get; private set; }
    //public EntityFX fx { get; private set; }
    #endregion

    //[Header("Knockback Info")]
    //[SerializeField] protected Vector2 knockbackDriction;
    //[SerializeField] protected float knockbackDuration;
    //protected bool isKnocked;


    [Header("Collision Info")]
    //public Transform attackCheck;
    //public float attackCheckRadius;
    [SerializeField] protected Transform[] groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;
    public RaycastHit[] groundHitInfo;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckRadius;
    [SerializeField] protected LayerMask whatIsWall;
    public RaycastHit[] wallHitInfo;

    [Header("Move Info")]
    public float moveSpeed = 6f;
    public float runSpeed = 10f;
    public float rotSpeed = 2f;
    public float jumpForce = 12f;

    [Header("Status Info")]
    public int hp = 1000;
    public int mp = 100;
    public int attackPower = 10;


    //public bool facingRight { get; set; } = true;
    //public int facingDir { get; set; } = 1;



    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        //fx = GetComponent<EntityFX>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        groundHitInfo = new RaycastHit[groundCheck.Length];
    }

    protected virtual void Update()
    {

    }

    protected virtual void FixedUpdate()
    {

    }

    public virtual void Damage(int attackPower)
    {
        //fx.StartCoroutine("FlashFX");
        //StartCoroutine("HitKnockback");

        Debug.Log(gameObject.name + " : 데미지");
    }

    //protected virtual IEnumerator HitKnockback()
    //{
    //    isKnocked = true;

    //    rb.velocity = new Vector2(knockbackDriction.x * -facingDir, knockbackDriction.y);

    //    yield return new WaitForSeconds(knockbackDuration);
    //    isKnocked = false;
    //}

    #region Velocity
    //public virtual void SetZeroVelocity()
    //{
    //    if (isKnocked)
    //        return;

    //    rb.velocity = Vector2.zero;
    //}


    public virtual void SetVelocity(float xVelocity, float yVelocity, float zVelocity)
    {
        //if (isKnocked)
        //    return;

        rb.velocity = new Vector3(xVelocity, yVelocity, zVelocity);
    }

    public virtual void SetVelocity(Vector3 velocity)
    {
        //if (isKnocked)
        //    return;
        rb.velocity = Vector3.zero;
        rb.AddForce(velocity, ForceMode.VelocityChange);
        //rb.velocity = velocity;
    }
    #endregion

    #region Collision
    public virtual bool IsGroundDetected()
    {

        for (int i = 0; i < groundCheck.Length; i++)
        {
            if (Physics.Raycast(groundCheck[i].position, Vector3.down, out groundHitInfo[i], groundCheckDistance, whatIsGround))
                return true;
        }

        return false;
    }

    public virtual bool IsGroundCollision(Collision collision)
    {
        for (int i = 0; i < groundCheck.Length; i++)
        {
            if (groundHitInfo[i].collider == collision.collider)
                return true;
        }

        return false;
    }

    public virtual bool IsWallDetected()
    {
        //wallCollider = Physics.OverlapSphere(wallCheck.position, wallCheckRadius, whatIsWall);
        //if (wallCollider.Length > 0)
        //{
        //    Debug.Log("" + wallCollider[0].)
        //}

        wallHitInfo = Physics.SphereCastAll(wallCheck.position, wallCheckRadius, wallCheck.forward, 0f, whatIsWall);

        if (wallHitInfo.Length > 0)
        {
            //Debug.Log("wallHitInfo.normal : " + wallHitInfo[0].normal);
            float angle = Vector3.Angle(wallHitInfo[0].normal, Vector3.up);
            //Debug.Log($"\nangle : {angle}\nnormal : {wallHitInfo[0].normal}\n{wallHitInfo[0].collider.name}");
            //Debug.Log("wallHitInfo.Length : " + wallHitInfo.Length);

            //if (angle > 65f)
            //    return true;
        }

        return false;
        //.=> Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }


    protected virtual void OnDrawGizmos()
    {
        //땅 체크
        for (int i = 0; i < groundCheck.Length; i++)
        {
            Gizmos.DrawLine(groundCheck[i].position, new Vector3(
                groundCheck[i].position.x, groundCheck[i].position.y - groundCheckDistance, groundCheck[i].position.z));
        }

        ////벽체크
        //Gizmos.DrawRay(wallCheck.position, wallCheck.forward * 0f);
        //Gizmos.DrawWireSphere(wallCheck.position + wallCheck.forward * 0f, wallCheckRadius);

        //Gizmos.color = Color.red;

        //Gizmos.DrawLine(wallCheck.position, new Vector3(
        //    wallCheck.position.x + wallCheckDistance * facingDir, wallCheck.position.y));

        //Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion
}
