using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IAttackable
{
    int AttackPower { get; set; }
    bool IsAttack { get; set; }
    int ManaPoint {  get; set; }

    /// <summary>
    /// ���� ����
    /// </summary>
    void Attack();

    /// <summary>
    /// ���� �� ���� �޺� �Է� ���� �������� ȣ��
    /// </summary>
    void NextAttackInputPossible();

    /// <summary>
    /// ���� �ִϸ��̼� ����������� ȣ��. 
    /// </summary>
    void AttackEnd();
}
