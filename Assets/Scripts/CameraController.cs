using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float seekSpeed = 4;
    public float maxDist = 3;
    public float beginSeekDist = 2;
    public float seekOvershoot = .25f;
    private int seeking;

    public void LateUpdate()
    {
        float distance = Vector2.Distance(transform.position, target.position);
        if (seeking == 0 && distance >= beginSeekDist)
            seeking = 1;

        if (seeking <= 0)
            return;

        if (seeking == 1)
        {
            Vector2 des = Vector2.MoveTowards(transform.position, target.position, Time.deltaTime * seekSpeed * Mathf.Clamp(distance * .25f, 0f, 2f));
            transform.position = new Vector3(des.x, des.y, -10);

            if (distance >= maxDist)
            {
                seeking = 2;
            }
            else if (distance < beginSeekDist - seekOvershoot)
            {
                seeking = 0;
            }
        }
        else
        {
            if (distance < maxDist)
            {
                seeking = 1;
                return;
            }

            Vector2 offset = transform.position - target.position;
            Vector2 des = (Vector2)target.position + offset.normalized * maxDist;

            transform.position = new Vector3(des.x, des.y, -10);
        }


    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position, beginSeekDist);
        Gizmos.DrawWireSphere((Vector2)transform.position, maxDist);
    }
}
