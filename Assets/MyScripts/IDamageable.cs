using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public int HealthPoint { get; set; }
    public int Defense { get; set; }
    public void Die();
    public void TakeDamage(int Damage);
    public void RestoreHealth();
}