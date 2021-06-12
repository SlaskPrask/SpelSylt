using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] Rigidbody2D body;
    public void Initialize(Vector2 velocity, Color color, string tag)
    {
        this.tag = tag;
        body.velocity = velocity;
        Destroy(gameObject, 2f);
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