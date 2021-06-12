using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector2 vel;
    public void Initialize(Vector2 velocity, Color color)
    {
        vel = velocity;
    }

    public void Update()
    {
        transform.position += (Vector3)vel;
    }
}
