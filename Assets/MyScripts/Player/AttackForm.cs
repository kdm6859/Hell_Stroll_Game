using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AttackFormData
{
    public GameObject[] attackPrefabs;
    public GameObject[] skillPrefabs;
}

public class AttackForm : MonoBehaviour
{
    public AttackFormData attackFormData;

    
}
