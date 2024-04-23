using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AttackFormData
{
    public string formName;
    public float magnifyingDamage;
    public float coolTime;
    public string animationName;
    public Sprite icon;
    public GameObject[] attackPrefabs;
    public GameObject[] skillPrefabs;
    public int comboMaxCount;

}

public class AttackForm : MonoBehaviour
{
    public AttackFormData attackFormData;

    protected SkillFormat skill_Format;
    public SkillFormat Skill_Format { get { return skill_Format; } set { skill_Format = value; } }

    //public virtual void Initialize()
    //{

    //}

    public virtual void Attack(Transform entity, Transform firePoint, int comboNum, int attackPower)
    {

    }

    public virtual void Skill(Transform entity, Transform firePoint, int attackPower)
    {

    }

    public virtual void ShutDownSkill()
    {
        
    }

    public virtual void keyDownSkillEnd()
    {

    }

}
