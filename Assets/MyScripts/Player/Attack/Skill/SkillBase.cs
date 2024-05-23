using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBase : MonoBehaviour
{
    [SerializeField]
    protected SkillFormat skill_Format;
    //public SkillFormat Skill_Format { get { return skill_Format; } set { skill_Format = value; } }

    public virtual void SkillInitialized(Transform[] skillEffectPoints)
    {

    }

    public virtual void SkillStart()
    {

    }

    public virtual void Skill(Transform entity, Transform firePoint, int attackPower)
    {

    }

    public virtual void SkillEnd()
    {

    }

    public virtual void ShutDownSkill()
    {

    }

    public virtual void keyDownSkillEnd()
    {

    }
}
