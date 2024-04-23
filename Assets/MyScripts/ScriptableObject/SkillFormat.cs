using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SkillFormat : ScriptableObject
{
    public string attackFormName;
    public int[] magnifyingDamages;
    public float coolTime;
    public int manaCost;
    //public string[] animationName;
    public Sprite icon;
    //public GameObject[] attackPrefabs;
    public GameObject[] skillPrefabs;
    //public int comboMaxCount;
}
