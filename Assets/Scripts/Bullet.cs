using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Bullet : MonoBehaviour
{
    [SerializeField] Rigidbody2D body;
    bool destroyOnImpact;
    [SerializeField] List<GameObject> initOnHit;
    public float damage { get; private set; }
    private Color col;

    public void Initialize(Vector2 velocity, bool destOnImpact, List<GameObject> initOnHit, Color color, int layer, float dmg)
    {
        damage = dmg;
        gameObject.layer = layer;
        destroyOnImpact = destOnImpact;
        col = GetComponent<SpriteRenderer>().color = color;
        body.velocity = velocity;
        this.initOnHit = initOnHit;
        RuntimeManager.PlayOneShotAttached("Event:/SFX/Shotsound", gameObject);
        Destroy(gameObject, 2f);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (initOnHit != null)
        {
            foreach (GameObject go in initOnHit)
            {
                Instantiate(go, transform.position, Quaternion.identity, null);
            }
        }

        if (destroyOnImpact)
        {
            RuntimeManager.PlayOneShotAttached("Event:/SFX/HitEnemy", gameObject);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        ParticleSystem ps = Instantiate(GameManager.instance.bulletBreakPrefab, transform.position, Quaternion.identity, null).GetComponent<ParticleSystem>();
        ps.transform.localScale = transform.localScale;
        var main = ps.main;
        main.startColor = col;
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
    public float damageAdder;
    public float damageMultiplier;
}

[Flags]
public enum ImpactBehaviour : byte
{
    DESTROY = 1, //if not set, pass through
    EXPLODE = 2
}