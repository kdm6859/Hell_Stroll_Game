using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovePoint : MonoBehaviour
{
    //public static EnemyMovePoint instance;

    [SerializeField] Transform[] movePoints;

    //private void Awake()
    //{
    //    for(int i = 0; i < transform.childCount; i++)
    //    {
    //        movePoints[i] = transform.GetChild(i);
    //        Debug.Log(movePoints[i].name);
    //    }
    //}

    public Transform[] MovePoints { get {  return movePoints; } }
}
