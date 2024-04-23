using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AttackFormat : ScriptableObject
{
    public string attackFormName;
    public int[] magnifyingDamages;
    //public string[] animationName;
    public Sprite icon;
    public GameObject[] attackPrefabs;
    //public GameObject[] skillPrefabs;
    public int comboMaxCount;
}