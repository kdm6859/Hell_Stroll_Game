using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IAttackable
{
    int AttackPower { get; set; }
    bool IsAttack { get; set; }
    int ManaPoint {  get; set; }

    /// <summary>
    /// 실제 공격
    /// </summary>
    void Attack();

    /// <summary>
    /// 공격 후 다음 콤보 입력 가능 시점에서 호출
    /// </summary>
    void NextAttackInputPossible();

    /// <summary>
    /// 공격 애니메이션 종료시점에서 호출. 
    /// </summary>
    void AttackEnd();
}
