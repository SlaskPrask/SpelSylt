using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System.Net.Http.Headers;

public class Enemy_Boss : Enemy_Controller, IDamageSource
{
    private Vector2 home;
    private Vector2 targetPoint;
    private Transform target;
    private float knockbackTime = .1f;
    private float knockbackTimer;
    [SerializeField] private float huntSpeed = 5;
    [SerializeField] private float chargeSpeed = 10;
    private float targetTime = 0f;
    [SerializeField] private float minPatrol = 1f;
    [SerializeField] private float maxPatrol = 6f;
    [SerializeField] private BoxCollider2D playerTrigger;
    [SerializeField] private float contactDamage = 2f;

    [SerializeField] private float chargeTime = 1f;
    private float chargeTimer;
    [SerializeField] private float timeBetweenShots;
    private float shotTimer;


    private float switchPatternTime = 0;
    [SerializeField]private float minTime = 5;
    [SerializeField]private float maxTime = 10;

    private enum AttackPattern
    {
        HUNT = 0,
        CHARGE = 1,
        SHOTS = 2
    }

    private AttackPattern pattern;

    protected override void AwakeInit()
    {
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        currentState = EnemyState.PATROLLING;
        anim = GetComponent<Animator>();
        knockbackTimer = 0;
        home = transform.position;
        playerTrigger.transform.parent = null;
        pattern = AttackPattern.HUNT;
    }

    void Update()
    {
        if (knockbackTimer > 0)
            knockbackTimer -= Time.deltaTime;
        switch (currentState)
        {
            case EnemyState.IDLE:
                break;
            case EnemyState.HUNTING:
                HuntTarget();
                break;
            case EnemyState.DEAD:
                return;
            default:
                break;
        }

        Vector2 vel = body.velocity;
    }

    private void HuntTarget()
    {
        if (switchPatternTime > 0)
            switchPatternTime -= Time.deltaTime;
        else
            GetNewPattern();

        switch (pattern)
        {
            case AttackPattern.HUNT:
                if (knockbackTimer <= 0)
                    body.velocity = ((Vector2)(target.position - transform.position)).normalized * huntSpeed;
                break;
            case AttackPattern.CHARGE:
                if (chargeTimer >= 0)
                {
                    chargeTimer -= Time.deltaTime;
                    if (knockbackTimer <= 0)
                    {
                        body.velocity = Vector2.zero;
                    }
                    if (chargeTime < 0f)
                    {
                        body.velocity = Vector2.zero;
                        body.AddForce(((Vector2)(target.position - transform.position)).normalized * chargeSpeed, ForceMode2D.Impulse);
                    }
                }
                else
                {
                    if (body.velocity.magnitude <= .5f)
                        chargeTimer = chargeTime;
                }  
                break;
            case AttackPattern.SHOTS:
                if (knockbackTimer <= 0)
                    body.velocity = Vector2.zero;

                if (shotTimer >= 0f)
                {
                    shotTimer -= Time.deltaTime;
                }
                else
                {
                    foreach (PowerUp_Scriptable item in shots)
                    {
                        if (item.type == PowerUpType.EXTRA_SHOT)
                        {
                            item.GetPowerup<PowerUp_ExtraShot>().Shoot(this, ((Vector2)(target.position - transform.position)).normalized, true);
                        }
                    }
                    shotTimer = timeBetweenShots;
                }

                break;

            default:
                break;
        }

        
    }

    private void GetNewPattern()
    {
        switchPatternTime = Random.Range(minTime, maxTime);
        pattern = (AttackPattern)Random.Range(0, 3);
        chargeTimer = chargeTime;
    }

    public void EnteredTriggerRange(Collider2D col)
    {
        if (currentState == EnemyState.PATROLLING)
        {
            RuntimeManager.PlayOneShotAttached("Event:/SFX/SpookyActivate", gameObject);
            currentState = EnemyState.HUNTING;
        }
        target = col.transform;
    }

    public void ExitedTriggerRange(Collider2D col)
    {
        if (currentState != EnemyState.DEAD)
        {
            body.velocity = Vector2.zero;
            currentState = EnemyState.PATROLLING;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.layer)
        {
            case 7: //player bullet
                Bullet bullet = collision.GetComponent<Bullet>();
                health -= bullet.GetDamage();

                if (health <= 0 && currentState != EnemyState.DEAD)
                {
                    currentState = EnemyState.DEAD;
                    body.velocity = Vector2.zero;
                    StartCoroutine(Die());
                }
                else if (currentState != EnemyState.DEAD)
                {
                    Vector2 dir = transform.position - bullet.transform.position;
                    body.AddForce(dir.normalized * knockback, ForceMode2D.Impulse);
                    knockbackTimer = knockbackTime;
                }
                break;
            default:
                break;
        }
    }

    private IEnumerator Die()
    {
        anim.SetTrigger("Death");
        Destroy(GetComponent<CircleCollider2D>());
        renderer.sortingOrder = 8;
        body.isKinematic = true;
        Destroy(playerTrigger.gameObject);
        RuntimeManager.PlayOneShotAttached("Event:/SFX/DeathSoundBest", gameObject);
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("Finished"))
        {
            yield return null;
        }
        if (drops != null)
            drops.CalculateDrop().Instantiate(transform.position);
        Destroy(gameObject);
    }

    public float GetDamage()
    {
        return contactDamage;
    }
}
