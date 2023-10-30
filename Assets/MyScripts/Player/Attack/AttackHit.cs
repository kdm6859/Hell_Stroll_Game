using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHit : MonoBehaviour
{
    protected const int enemyLayer = 6;

    protected int attackPower = 1;
    [SerializeField] protected float magnifyingPower = 1;

    protected virtual void Start()
    {
        
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
