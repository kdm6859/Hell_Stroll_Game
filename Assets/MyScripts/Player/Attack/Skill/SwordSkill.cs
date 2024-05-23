using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI.Table;

public class SwordSkill : SkillBase
{
    private int magnifyingDamages;
    private GameObject swordEnergyCore;
    private GameObject aura;
    private GameObject swordSkill;

    private const int magnifyingDamagesCount = 1;
    private const int effectCount = 3;


    public override void SkillInitialized(Transform[] skillEffectTransform)
    {
        if(skill_Format.magnifyingDamages.Length == magnifyingDamagesCount)
        {
            magnifyingDamages = skill_Format.magnifyingDamages[0];
        }

        if (skill_Format.skillPrefabs.Length == effectCount)
        {
            swordEnergyCore = Instantiate(skill_Format.skillPrefabs[0], skillEffectTransform[0]);
            aura = Instantiate(skill_Format.skillPrefabs[1], skillEffectTransform[1]);
            //swordSkill = Instantiate(skill_Format.skillPrefabs[2], skillEffectTransform[2]);
            swordSkill = skill_Format.skillPrefabs[2];
        }

        if(swordEnergyCore)
            swordEnergyCore.SetActive(false);
        if(aura)
            aura.SetActive(false);
        //swordSkill.SetActive(false);
    }

    public override void SkillStart()
    {
        //��ų �غ� ����Ʈ Ȱ��ȭ
        if (swordEnergyCore)
            swordEnergyCore.SetActive(true);
        if (aura)
            aura.SetActive(true);
    }

    public override void Skill(Transform entity, Transform firePoint, int attackPower)
    {
        base.Skill(entity, firePoint, attackPower);

        GameObject skill;

        //��ų �غ� ����Ʈ ��Ȱ��ȭ
        if (swordEnergyCore)
            swordEnergyCore.SetActive(false);
        if (aura)
            aura.SetActive(false);

        Quaternion rot = Quaternion.Euler(entity.rotation.eulerAngles.x, entity.rotation.eulerAngles.y, -90f);
        skill = Instantiate(swordSkill, new Vector3(entity.position.x, entity.position.y + 1.2f, entity.position.z), rot);
        //attack.transform.localScale = new Vector3(-attack.transform.localScale.x, attack.transform.localScale.y, attack.transform.localScale.z);
        skill.transform.localScale *= 4f;

        //��ų ������ ����
        skill.GetComponent<SwordHit>().SetAttackPower(attackPower, magnifyingDamages);

        //��ų �������� �߻�
        skill.GetComponent<Rigidbody>().AddForce(swordSkill.transform.forward * 60f, ForceMode.Impulse);
    }

    public override void SkillEnd()
    {

    }

    //��ų ���� ���� �� ����(��ų ��� �� ��� ��)
    public override void ShutDownSkill()
    {
        if (swordEnergyCore)
            swordEnergyCore.SetActive(false);
        if (aura)
            aura.SetActive(false);
    }

    //public void SwordSkillAura()
    //{

    //}
}
