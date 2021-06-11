using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Controller : MonoBehaviour
{
    protected Vector2 move;
    protected Rigidbody2D body;
    protected new SpriteRenderer renderer;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        renderer = GetComponentInChildren<SpriteRenderer>(true);
        AwakeInit();
    }
    private void Start()
    {
        StartInit();
    }

    protected virtual void AwakeInit() { }
    protected virtual void StartInit() { }
}
