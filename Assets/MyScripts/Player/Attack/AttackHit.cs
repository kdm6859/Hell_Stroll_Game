using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHit : MonoBehaviour
{
    protected const int enemyLayer = 6;

    protected int attackPower = 1;

    protected virtual void Start()
    {
        
    }

    public virtual void SetAttackPower(int attackPower, float magnifyingPower)
    {
        this.attackPower = (int)(attackPower * magnifyingPower);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {

    }


    //protected virtual void OnParticleCollision(GameObject other)
    //{

    //}

    //protected virtual void OnParticleTrigger()
    //{

    //}

}
