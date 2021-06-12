using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] Rigidbody2D body;
    bool destroyOnImpact;
    GameObject initOnHit;

    public void Initialize(Vector2 velocity, bool destOnImpact, GameObject initOnHit, Color color, int layer)
    {
        gameObject.layer = layer;
        destroyOnImpact = destOnImpact;
        body.velocity = velocity;
        this.initOnHit = initOnHit;
        Destroy(gameObject, 2f);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (initOnHit != null)
        {
            Instantiate(initOnHit, transform.position, Quaternion.identity, null);
        }

        if (destroyOnImpact)
        {
            Destroy(gameObject);
        }
    }
}

[System.Serializable]
public struct ShotModifiers
{
    public float sizeAdder;
    public float sizeMultiplier;
    public float speedAdder;
    public float speedMultiplier;
    public float rateAdder;
    public float rateMultiplier;
}

[Flags]
public enum ImpactBehaviour : byte
{
    DESTROY = 1, //if not set, pass through
    EXPLODE = 2
}