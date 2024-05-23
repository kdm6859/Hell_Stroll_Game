using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : AttackBase
{

    public override void Attack(Transform entity, Transform firePoint, int comboNum, int attackPower)
    {
        base.Attack(entity, firePoint, comboNum, attackPower);

        //실제 공격 오브젝트
        GameObject attackObj;
        //float magnifyingPower = 1f;

        if (comboNum == 1)
        {
            //검의 X rot 값을 검이펙트 Z rot 값에 넣어서 검의 각도와 이펙트의 각도를 일치시킴
            Quaternion rot = Quaternion.Euler(entity.rotation.eulerAngles.x, entity.rotation.eulerAngles.y, 90f - firePoint.rotation.eulerAngles.x);
            //attack = Instantiate(attackFormData.attackPrefabs[0], firePoint.position, rot);
            attackObj = Instantiate(AttackFormat.attackPrefabs[0], new Vector3(entity.position.x, entity.position.y + 1.2f, entity.position.z), rot);
            attackObj.transform.localScale = new Vector3(-attackObj.transform.localScale.x, attackObj.transform.localScale.y, attackObj.transform.localScale.z);
            attackObj.transform.localScale *= 1.5f;

            //magnifyingPower = 1.2f;
        }
        else if (comboNum == 2)
        {
            //검의 X rot 값을 검이펙트 Z rot 값에 넣어서 검의 각도와 이펙트의 각도를 일치시킴
            //Quaternion rot = Quaternion.Euler(entity.rotation.eulerAngles.x, entity.rotation.eulerAngles.y, 90f - firePoint.rotation.eulerAngles.x);
            //임의로 검이펙트 값을 설정
            Quaternion rot = Quaternion.Euler(entity.rotation.eulerAngles.x, entity.rotation.eulerAngles.y, 210f);
            //firePoint.Translate(-entity.transform.right / 1f);//51.12052f); entity.position + (entity.forward * 10f)+(entity.up*2f)
            attackObj = Instantiate(AttackFormat.attackPrefabs[0], new Vector3(entity.position.x, entity.position.y + 1.6f, entity.position.z), rot);
            attackObj.transform.localScale = new Vector3(-attackObj.transform.localScale.x, attackObj.transform.localScale.y, attackObj.transform.localScale.z);
            attackObj.transform.localScale *= 1.5f;

            //magnifyingPower = 1.2f;
        }
        else if (comboNum >= 3)
        {
            //검의 X rot 값을 검이펙트 Z rot 값에 넣어서 검의 각도와 이펙트의 각도를 일치시킴
            Quaternion rot = Quaternion.Euler(entity.rotation.eulerAngles.x, entity.rotation.eulerAngles.y, 90f - firePoint.rotation.eulerAngles.x);
            //firePoint.Translate(-entity.transform.right / 1f);//51.12052f); entity.position + (entity.forward * 10f)+(entity.up*2f)
            attackObj = Instantiate(AttackFormat.attackPrefabs[0], new Vector3(entity.position.x, entity.position.y + 1.2f, entity.position.z), rot);
            attackObj.transform.localScale = new Vector3(-attackObj.transform.localScale.x, attackObj.transform.localScale.y, attackObj.transform.localScale.z);
            attackObj.transform.localScale *= 2.5f;

            //magnifyingPower = 1.5f;
        }
        else
            return;

        //attack.GetComponent<SwordHit>().SetAttackPower(attackPower, magnifyingPower);
        //공격 오브젝트의 데미지 설정
        attackObj.GetComponent<AttackHit>().SetAttackPower(attackPower, AttackFormat.magnifyingDamages[comboNum - 1]);
    }
}
