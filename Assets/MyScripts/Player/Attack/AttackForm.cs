using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AttackFormData
{
    public string formName;
    public GameObject[] attackPrefabs;
    public GameObject[] skillPrefabs;
    public int comboMaxCount;
}

public class AttackForm : MonoBehaviour
{
    public AttackFormData attackFormData;

    //public virtual void Initialize()
    //{

    //}

    public virtual void Attack(Transform entity, Transform firePoint, int comboNum, int attackPower)
    {

    }

    public virtual void Skill(Transform entity, Transform firePoint, int attackPower)
    {

    }
}
