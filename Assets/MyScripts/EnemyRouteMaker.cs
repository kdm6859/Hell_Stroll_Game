using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PointPositions
{
    public List<Vector3> pointPositions { get; set; } = new List<Vector3>();
}

public class EnemyRouteMaker : MonoBehaviour
{
    public static EnemyRouteMaker instance;

    [SerializeField] public GameObject pointPrefab;
    public int manualFontSize = 13;
    //public List<GameObject> points { get; set; } = new List<GameObject>();
    public List<PointPositions> enemyMoveArea { get; set; } = new List<PointPositions>();
    public int enemyMoveAreaIndex { get; set; } = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    // Start is called before the first frame update
    void Start()
    {

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
                for (int j = 0; j < enemyMoveArea[i].pointPositions.Count; j++)
                {
                    if (i == enemyMoveAreaIndex)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawLineStrip(enemyMoveArea[i].pointPositions.ToArray(), true);
                        Gizmos.color = Color.red;
                    }
                    else
                        Gizmos.DrawLineStrip(enemyMoveArea[i].pointPositions.ToArray(), true);
                }
            }

        }
    }
}
