using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    public int AttackPower { get; set; }
    public bool IsAttack { get; set; }
    public int ManaPoint {  get; set; }

    public void Attack();
}
