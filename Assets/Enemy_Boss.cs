using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Enemy_Boss : Enemy_Controller, IDamageSource
{
    [SerializeField] private float patrolRadius = 10;
    private Vector2 home;
    private Vector2 targetPoint;
    private Transform target;
    private float knockbackTime = .1f;
    private float knockbackTimer;
    [SerializeField] private float patrolSpeed = 3;
    [SerializeField] private float huntSpeed = 5;
    private float targetTime = 0f;
    [SerializeField] private float minPatrol = 1f;
    [SerializeField] private float maxPatrol = 6f;
    [SerializeField] private CircleCollider2D playerTrigger;
    [SerializeField] private float contactDamage = 2f;

    protected override void AwakeInit()
    {
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        currentState = EnemyState.PATROLLING;
        anim = GetComponent<Animator>();
        knockbackTimer = 0;
        home = transform.position;
        playerTrigger.radius = patrolRadius;
        playerTrigger.transform.parent = null;
        GetNewTargetPoint();
    }

    void Update()
    {
        if (knockbackTimer > 0)
            knockbackTimer -= Time.deltaTime;
        switch (currentState)
        {
            case EnemyState.PATROLLING:
                Patrolling();
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
        if (knockbackTimer <= 0)
            body.velocity = ((Vector2)(target.position - transform.position)).normalized * huntSpeed;
    }

    private void Patrolling()
    {

        targetTime -= Time.deltaTime;
        if (targetTime <= 0f ||
            Vector2.Distance(targetPoint, transform.position) < .25f ||
            Vector2.Distance(home, transform.position) > patrolRadius)
        {
            GetNewTargetPoint();
        }
    }

    private void GetNewTargetPoint()
    {
        targetPoint = home + Random.insideUnitCircle * patrolRadius * .5f;
        targetTime = Random.Range(minPatrol, maxPatrol);
        body.velocity = (targetPoint - (Vector2)transform.position).normalized * patrolSpeed;
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
            GetNewTargetPoint();
            currentState = EnemyState.PATROLLING;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            home = transform.position;
        }
        Gizmos.DrawWireSphere(home, patrolRadius);
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
