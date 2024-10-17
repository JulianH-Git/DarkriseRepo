using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed;

    private Vector3 targetPos; 
    private Vector3 newPos;

    public Vector3 minPos;
    public Vector3 maxPos;

    // LateUpdate is called after all other Updates
    void LateUpdate()
    {
        if(transform.position != player.position)
        {
            targetPos = player.position;

            Vector3 camBoundaryPos = new Vector3(
                Mathf.Clamp(targetPos.x, minPos.x, maxPos.x),
                Mathf.Clamp(targetPos.y, minPos.y, maxPos.y),
                Mathf.Clamp(targetPos.z, minPos.z, maxPos.z));

            newPos = Vector3.Lerp(transform.position, camBoundaryPos, smoothSpeed);
            transform.position = newPos;

        }
    }
}
