using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackFormManager_Enemy : MonoBehaviour
{
    public static AttackFormManager_Enemy instance;

    [SerializeField]
    AttackForm[] attackForms;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public AttackForm SetAttackForm(int attackFormNum)
    {
        return attackForms[attackFormNum];
    }

    public int AttackFormsMaxNum()
    {
        return attackForms.Length;
    }
}
