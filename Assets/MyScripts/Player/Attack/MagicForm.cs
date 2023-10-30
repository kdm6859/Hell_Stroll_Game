using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class MagicForm : AttackForm
{
    //public override void Initialize()
    //{
    //    base.Initialize();

    //    attackFormData.formName = "Magic";
    //    attackFormData.comboMaxCount = 3;
    //}

    public override void Attack(Transform entity, Transform firePoint, int comboNum, int attackPower)
    {
        base.Attack(entity, firePoint, comboNum, attackPower);

        GameObject attack = Instantiate(attackFormData.attackPrefabs[0], firePoint.position, entity.rotation);
    }

}
