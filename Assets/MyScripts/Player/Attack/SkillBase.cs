using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBase : MonoBehaviour
{
    protected SkillFormat skill_Format;
    public SkillFormat Skill_Format { get { return skill_Format; } set { skill_Format = value; } }

    public virtual void Skill(Transform entity, Transform firePoint, int attackPower)
    {

    }
}
