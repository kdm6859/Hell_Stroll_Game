using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class MagicForm : AttackForm
{
    [SerializeField] GameObject skill_Laser;
    [SerializeField] GameObject[] skill_Lasers;
    bool isSkill = true;

    public override void Attack(Transform entity, Transform firePoint, int comboNum, int attackPower)
    {
        base.Attack(entity, firePoint, comboNum, attackPower);

        GameObject attack = Instantiate(attackFormData.attackPrefabs[0], firePoint.position, entity.rotation);

        float magnifyingPower = 1f;

        attack.GetComponent<MagicHit>().SetAttackPower(attackPower, magnifyingPower);

    }

    public override void Skill(Transform entity, Transform firePoint, int attackPower)
    {
        base.Skill(entity, firePoint, attackPower);

        float magnifyingPower = 0.3f;

        skill_Lasers[0].SetActive(true);

        //skill_Lasers[0].GetComponent<MagicHit>().SetAttackPower(attackPower, magnifyingPower);
        StartCoroutine(LaserMiddleOn(attackPower, magnifyingPower));
 
    }

    IEnumerator LaserMiddleOn(int attackPower, float magnifyingPower)
    {
        yield return new WaitForSeconds(1f);

        skill_Lasers[0].SetActive(false);
        skill_Lasers[1].SetActive(true);

        skill_Lasers[1].GetComponent<MagicHit>().SetAttackPower(attackPower, magnifyingPower);

        while (isSkill)
        {
            yield return null;
        }
        isSkill = true;

        skill_Lasers[1].SetActive(false);
        skill_Lasers[2].SetActive(true);

        yield return new WaitForSeconds(1.5f);
        skill_Lasers[2].SetActive(false);
    }

    public void IsSkill_False()
    {
        isSkill = false;
    }
}
