using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordForm : AttackForm
{
    public override void Attack(Transform entity, Transform firePoint, int comboNum, int attackPower)
    {
        base.Attack(entity, firePoint, comboNum, attackPower);

        GameObject attack;



        if (comboNum == 1)
        {
            //���� X rot ���� ������Ʈ Z rot ���� �־ ���� ������ ����Ʈ�� ������ ��ġ��Ŵ
            Quaternion rot = Quaternion.Euler(entity.rotation.eulerAngles.x, entity.rotation.eulerAngles.y, 90f - firePoint.rotation.eulerAngles.x);
            attack = Instantiate(attackFormData.attackPrefabs[0], firePoint.position, rot);
            attack.transform.localScale = new Vector3(-attack.transform.localScale.x, attack.transform.localScale.y, attack.transform.localScale.z);
        }
        else if (comboNum == 2)
        {
            //���� X rot ���� ������Ʈ Z rot ���� �־ ���� ������ ����Ʈ�� ������ ��ġ��Ŵ
            Quaternion rot = Quaternion.Euler(entity.rotation.eulerAngles.x, entity.rotation.eulerAngles.y, 90f - firePoint.rotation.eulerAngles.x);
            //firePoint.Translate(-entity.transform.right / 1f);//51.12052f); entity.position + (entity.forward * 10f)+(entity.up*2f)
            attack = Instantiate(attackFormData.attackPrefabs[0], new Vector3(entity.position.x, entity.position.y + 1.2f, entity.position.z), rot);
            attack.transform.localScale = new Vector3(-attack.transform.localScale.x, attack.transform.localScale.y, attack.transform.localScale.z);
            attack.transform.localScale *= 2.5f;
        }
        else if (comboNum >= 3)
        {
            //���� X rot ���� ������Ʈ Z rot ���� �־ ���� ������ ����Ʈ�� ������ ��ġ��Ŵ
            Quaternion rot = Quaternion.Euler(entity.rotation.eulerAngles.x, entity.rotation.eulerAngles.y, firePoint.rotation.eulerAngles.x);
            attack = Instantiate(attackFormData.attackPrefabs[0], firePoint.position, rot);
        }
        else
            return;

        attack.GetComponent<SwordHit>().SetAttackPower(attackPower);
    }
}