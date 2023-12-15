using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[Serializable]
public class MovePoints
{
    [SerializeField] List<Vector3> pointPositions = new List<Vector3>();
    public List<Vector3> PointPositions { get { return pointPositions; } set { pointPositions = value; } }
}

public class EnemyRouteMaker : MonoBehaviour
{
    //public static EnemyRouteMaker instance;

    [SerializeField] public GameObject enemyPrefab;

    public int manualFontSize = 13;
    //public List<GameObject> points { get; set; } = new List<GameObject>();
    [SerializeField] List<MovePoints> enemyMoveArea = new List<MovePoints>();
    int enemyMoveAreaIndex = 0;
    public List<MovePoints> EnemyMoveArea { get { return enemyMoveArea; } set { enemyMoveArea = value; } }
    public int EnemyMoveAreaIndex { get { return enemyMoveAreaIndex; } set { enemyMoveAreaIndex = value; } } 
    
    //private void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //    }
    //    else
    //    {
    //        Destroy(this.gameObject);
    //    }

    //}

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("??????????");
        for (int i = 0; i < enemyMoveArea.Count; i++)
        {
            Debug.Log("적 생성!!");
            Instantiate(enemyPrefab, enemyMoveArea[i].PointPositions[0], Quaternion.identity).GetComponent<Enemy>().SetArea(enemyMoveArea[i]);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }



    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //Enemy 이동 경로 표시
        if (enemyMoveArea.Count > 0)
        {
            for (int i = 0; i < enemyMoveArea.Count; i++)
            {
                for (int j = 0; j < enemyMoveArea[i].PointPositions.Count; j++)
                {
                    if (i == enemyMoveAreaIndex)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawLineStrip(enemyMoveArea[i].PointPositions.ToArray(), true);
                        Gizmos.color = Color.red;
                    }
                    else
                        Gizmos.DrawLineStrip(enemyMoveArea[i].PointPositions.ToArray(), true);
                }
            }

        }
    }
}
