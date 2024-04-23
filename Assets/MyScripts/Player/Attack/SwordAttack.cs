using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : AttackBase
{

    public override void Attack(Transform entity, Transform firePoint, int comboNum, int attackPower)
    {
        base.Attack(entity, firePoint, comboNum, attackPower);

        GameObject attack;
        //float magnifyingPower = 1f;

        if (comboNum == 1)
        {
            //���� X rot ���� ������Ʈ Z rot ���� �־ ���� ������ ����Ʈ�� ������ ��ġ��Ŵ
            Quaternion rot = Quaternion.Euler(entity.rotation.eulerAngles.x, entity.rotation.eulerAngles.y, 90f - firePoint.rotation.eulerAngles.x);
            //attack = Instantiate(attackFormData.attackPrefabs[0], firePoint.position, rot);
            attack = Instantiate(AttackFormat.attackPrefabs[0], new Vector3(entity.position.x, entity.position.y + 1.2f, entity.position.z), rot);
            attack.transform.localScale = new Vector3(-attack.transform.localScale.x, attack.transform.localScale.y, attack.transform.localScale.z);
            attack.transform.localScale *= 1.5f;

            //magnifyingPower = 1.2f;
        }
        else if (comboNum == 2)
        {
            //���� X rot ���� ������Ʈ Z rot ���� �־ ���� ������ ����Ʈ�� ������ ��ġ��Ŵ
            //Quaternion rot = Quaternion.Euler(entity.rotation.eulerAngles.x, entity.rotation.eulerAngles.y, 90f - firePoint.rotation.eulerAngles.x);
            //���Ƿ� ������Ʈ ���� ����
            Quaternion rot = Quaternion.Euler(entity.rotation.eulerAngles.x, entity.rotation.eulerAngles.y, 210f);
            //firePoint.Translate(-entity.transform.right / 1f);//51.12052f); entity.position + (entity.forward * 10f)+(entity.up*2f)
            attack = Instantiate(AttackFormat.attackPrefabs[0], new Vector3(entity.position.x, entity.position.y + 1.6f, entity.position.z), rot);
            attack.transform.localScale = new Vector3(-attack.transform.localScale.x, attack.transform.localScale.y, attack.transform.localScale.z);
            attack.transform.localScale *= 1.5f;

            //magnifyingPower = 1.2f;
        }
        else if (comboNum >= 3)
        {
            //���� X rot ���� ������Ʈ Z rot ���� �־ ���� ������ ����Ʈ�� ������ ��ġ��Ŵ
            Quaternion rot = Quaternion.Euler(entity.rotation.eulerAngles.x, entity.rotation.eulerAngles.y, 90f - firePoint.rotation.eulerAngles.x);
            //firePoint.Translate(-entity.transform.right / 1f);//51.12052f); entity.position + (entity.forward * 10f)+(entity.up*2f)
            attack = Instantiate(AttackFormat.attackPrefabs[0], new Vector3(entity.position.x, entity.position.y + 1.2f, entity.position.z), rot);
            attack.transform.localScale = new Vector3(-attack.transform.localScale.x, attack.transform.localScale.y, attack.transform.localScale.z);
            attack.transform.localScale *= 2.5f;

            //magnifyingPower = 1.5f;
        }
        else
            return;

        //attack.GetComponent<SwordHit>().SetAttackPower(attackPower, magnifyingPower);
        attack.GetComponent<SwordHit>().SetAttackPower(attackPower, AttackFormat.magnifyingDamages[comboNum - 1]);
    }
}
