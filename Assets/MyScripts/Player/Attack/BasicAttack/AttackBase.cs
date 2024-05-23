using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBase : MonoBehaviour
{
    [SerializeField]
    protected AttackFormat attackFormat;
    public AttackFormat AttackFormat { get { return attackFormat; } set { attackFormat = value; } }

    public virtual void Attack(Transform entity, Transform firePoint, int comboNum, int attackPower)
    {

    }
}
