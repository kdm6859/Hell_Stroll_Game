using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackFormManager : MonoBehaviour
{
    //public static AttackFormManager instance;

    [SerializeField]
    AttackForm[] attackForms;

    //private void Awake()
    //{
    //    if(instance == null)
    //    {
    //        instance = this;
    //    }
    //    else
    //    {
    //        Destroy(this.gameObject);
    //    }
    //}

    public void SkillInitialized(int attackFormNum, Transform[] skillEffectPoints)
    {
        attackForms[attackFormNum].SkillScript.SkillInitialized(skillEffectPoints);
    }

    public AttackForm GetAttackForm(int attackFormNum)
    {
        return attackForms[attackFormNum];
    }

    public int GetAttackFormsMaxNum()
    {
        return attackForms.Length;
    }
}
