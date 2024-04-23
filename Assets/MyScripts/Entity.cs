using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Entity : MonoBehaviour, IMovable
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
    [SerializeField] Vector3 boxSize;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected Transform[] groundChecks;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;
    public RaycastHit[] groundHitInfo;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckRadius;
    [SerializeField] protected LayerMask whatIsWall;
    public RaycastHit[] wallHitInfo;


    #region IMovable
    [Header("Move Info")]
    protected float moveSpeed = 6f;
    protected float runSpeed = 12f;
    protected float rotSpeed = 2f;
    protected float jumpForce = 16f;

    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }
    public float RunSpeed { get { return runSpeed; } set { runSpeed = value; } }
    public float RotSpeed { get { return rotSpeed; } set { rotSpeed = value; } }
    public float JumpForce { get { return jumpForce; } set { jumpForce = value; } }
    #endregion


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

        groundHitInfo = new RaycastHit[groundChecks.Length];
    }

    protected virtual void Update()
    {

    }

    protected virtual void FixedUpdate()
    {

    }

    //public virtual void TakeDamage(int attackPower)
    //{
    //    //fx.StartCoroutine("FlashFX");
    //    //StartCoroutine("HitKnockback");

    //    Debug.Log(gameObject.name + " : µ¥¹ÌÁö");
    //}

    //protected virtual IEnumerator HitKnockback()
    //{
    //    isKnocked = true;

    //    rb.velocity = new Vector2(knockbackDriction.x * -facingDir, knockbackDriction.y);

    //    yield return new WaitForSeconds(knockbackDuration);
    //    isKnocked = false;
    //}

    #region IMovable
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

        //rb.velocity = new Vector3(xVelocity, yVelocity, zVelocity);

        SetVelocity(new Vector3(xVelocity, yVelocity, zVelocity));
    }

    public virtual void SetVelocity(Vector3 velocity)
    {
        //if (isKnocked)
        //    return;
        //rb.velocity = Vector3.zero;

        Vector3 addVelocity = velocity - rb.velocity;

        rb.AddForce(addVelocity, ForceMode.VelocityChange);
        //rb.velocity = velocity;
    }
    #endregion

    #region Collision
    public virtual bool IsGroundDetected()
    {
        return Physics.BoxCast(groundCheck.position, boxSize / 2.0f, -transform.up, transform.rotation, groundCheckDistance, whatIsGround);

        //for (int i = 0; i < groundChecks.Length; i++)
        //{
        //    if (Physics.Raycast(groundChecks[i].position, Vector3.down, out groundHitInfo[i], groundCheckDistance, whatIsGround))
        //    {
        //        Debug.Log("¶¥¶¥");
        //        return true;
        //    }
        //}

        //return false;
    }

    //public virtual bool IsGroundCollision(Collision collision)
    //{
    //    for (int i = 0; i < groundCheck.Length; i++)
    //    {
    //        if (groundHitInfo[i].collider == collision.collider)
    //            return true;
    //    }

    //    return false;
    //}

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
        //¶¥ Ã¼Å©
        Gizmos.DrawCube(groundCheck.position - transform.up * groundCheckDistance, boxSize);
        //for (int i = 0; i < groundChecks.Length; i++)
        //{
        //    Gizmos.DrawLine(groundChecks[i].position, new Vector3(
        //        groundChecks[i].position.x, groundChecks[i].position.y - groundCheckDistance, groundChecks[i].position.z));
        //}

        ////º®Ã¼Å©
        //Gizmos.DrawRay(wallCheck.position, wallCheck.forward * 0f);
        //Gizmos.DrawWireSphere(wallCheck.position + wallCheck.forward * 0f, wallCheckRadius);

        //Gizmos.color = Color.red;

        //Gizmos.DrawLine(wallCheck.position, new Vector3(
        //    wallCheck.position.x + wallCheckDistance * facingDir, wallCheck.position.y));

        //Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion
}
