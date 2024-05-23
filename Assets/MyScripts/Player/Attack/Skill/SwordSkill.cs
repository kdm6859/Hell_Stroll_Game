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
        //스킬 준비 이펙트 활성화
        if (swordEnergyCore)
            swordEnergyCore.SetActive(true);
        if (aura)
            aura.SetActive(true);
    }

    public override void Skill(Transform entity, Transform firePoint, int attackPower)
    {
        base.Skill(entity, firePoint, attackPower);

        GameObject skill;

        //스킬 준비 이펙트 비활성화
        if (swordEnergyCore)
            swordEnergyCore.SetActive(false);
        if (aura)
            aura.SetActive(false);

        Quaternion rot = Quaternion.Euler(entity.rotation.eulerAngles.x, entity.rotation.eulerAngles.y, -90f);
        skill = Instantiate(swordSkill, new Vector3(entity.position.x, entity.position.y + 1.2f, entity.position.z), rot);
        //attack.transform.localScale = new Vector3(-attack.transform.localScale.x, attack.transform.localScale.y, attack.transform.localScale.z);
        skill.transform.localScale *= 4f;

        //스킬 데미지 설정
        skill.GetComponent<SwordHit>().SetAttackPower(attackPower, magnifyingDamages);

        //스킬 전방으로 발사
        skill.GetComponent<Rigidbody>().AddForce(swordSkill.transform.forward * 60f, ForceMode.Impulse);
    }

    public override void SkillEnd()
    {

    }

    //스킬 강제 종료 시 실행(스킬 사용 중 사망 등)
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
