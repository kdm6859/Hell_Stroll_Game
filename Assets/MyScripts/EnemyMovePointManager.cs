using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovePointManager : MonoBehaviour
{
    public static EnemyMovePointManager instance;

    [SerializeField] EnemyMovePoint[] enemyMovePoints;
    public bool[] areaAllocation;

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

        areaAllocation = new bool[enemyMovePoints.Length];
    }

    public bool SetArea(out EnemyMovePoint enemyMovePoint)
    {
        for (int i = 0; i < enemyMovePoints.Length; i++)
        {
            if (!areaAllocation[i])
            {
                areaAllocation[i] = true;

                enemyMovePoint = enemyMovePoints[i];

                return true;
            }
        }

        enemyMovePoint = null;

        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        foreach (var enemyMovePoint in enemyMovePoints)
        {
            Vector3[] points = new Vector3[enemyMovePoint.MovePoints.Length];
            for (int i=0; i< enemyMovePoint.MovePoints.Length; i++)
            {
                points[i] = enemyMovePoint.MovePoints[i].position;
            }

            Gizmos.DrawLineStrip(points, true);
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
}
