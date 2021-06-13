using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Bullet : MonoBehaviour, IDamageSource
{
    [SerializeField] Rigidbody2D body;
    bool destroyOnImpact;
    [SerializeField] List<GameObject> initOnHit;
    private float damage;
    private Color col;
    float audioCooldown = .1f;
    float lastTimePlayed = 0f;

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
                GameObject temp = Instantiate(go, transform.position, Quaternion.identity, null);
                temp.transform.localScale = transform.localScale;
            }
        }

        if (destroyOnImpact || collision.gameObject.layer == 0)
        {
            if (collision.gameObject.layer != 6)
            {
                RuntimeManager.PlayOneShotAttached("Event:/SFX/HitEnemy", gameObject);
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer != 6)
        {
            if (lastTimePlayed <= Time.time - audioCooldown)
            {
                lastTimePlayed = Time.time;
                RuntimeManager.PlayOneShotAttached("Event:/SFX/HitEnemy", gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        ParticleSystem ps = Instantiate(GameManager.instance.bulletBreakPrefab, transform.position, Quaternion.identity, null).GetComponent<ParticleSystem>();
        ps.transform.localScale = transform.localScale;
        var main = ps.main;
        main.startColor = col;
    }

    public float GetDamage()
    {
        return damage;
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
    public bool destroyOnContact;
}

[Flags]
public enum ImpactBehaviour : byte
{
    DESTROY = 1, //if not set, pass through
    EXPLODE = 2
}