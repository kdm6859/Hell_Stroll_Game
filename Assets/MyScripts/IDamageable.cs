using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IDamageable
{
    int HealthPoint { get; set; }
    int Defense { get; set; }
    void Die();
    void TakeDamage(int Damage);
    void RestoreHealth();
}