using System.Collections;
using System.Collections.Generic;
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
    public Transform attackCheck;
    public float attackCheckRadius;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;


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
    }

    protected virtual void Update()
    {
        
    }

    protected virtual void FixedUpdate()
    {
        
    }

    public virtual void Damage()
    {
        //fx.StartCoroutine("FlashFX");
        //StartCoroutine("HitKnockback");

        Debug.Log(gameObject.name + "µ¥¹ÌÁö");
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

        rb.velocity = velocity;
    }
    #endregion

    #region Collision
    public virtual bool IsGroundDetected()
    { return false; }
        //=> Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

    //public virtual bool IsWallDetected()
    //    => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);

    //protected virtual void OnDrawGizmos()
    //{
    //    Gizmos.DrawLine(groundCheck.position, new Vector3(
    //        groundCheck.position.x, groundCheck.position.y - groundCheckDistance));

    //    Gizmos.color = Color.red;

    //    //Gizmos.DrawLine(wallCheck.position, new Vector3(
    //    //    wallCheck.position.x + wallCheckDistance * facingDir, wallCheck.position.y));

    //    Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    //}
    #endregion
}
