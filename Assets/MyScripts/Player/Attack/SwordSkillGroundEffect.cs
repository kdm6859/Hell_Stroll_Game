using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSkillGroundEffect : MonoBehaviour
{
    [SerializeField] GameObject groundEffectPrefab;
    [SerializeField] Transform groundCheck;
    RaycastHit groundHitInfo;
    [SerializeField] float groundCheckDistance = 1f;
    bool isCreate = false;
    int isCreateCount = 0;


    private void FixedUpdate()
    {
        if (isCreate)
        {
            if (isCreateCount == 0)
            {
                Instantiate(groundEffectPrefab, groundHitInfo.point + (transform.forward * 1f), Quaternion.identity);
            }

            isCreateCount++;

            if (isCreateCount >= 3)
            {
                isCreateCount = 0;
            }

        }
        else
        {
            isCreateCount = 0;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (IsGroundDetected())
            isCreate = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isCreate = false;
    }


    public bool IsGroundDetected()
    {
        return Physics.Raycast(groundCheck.position, Vector3.down, out groundHitInfo, groundCheckDistance);
    }

    protected virtual void OnDrawGizmos()
    {
        //¶¥ Ã¼Å©
        Gizmos.DrawLine(groundCheck.position, new Vector3(
                groundCheck.position.x, groundCheck.position.y - groundCheckDistance, groundCheck.position.z));
    }

}
