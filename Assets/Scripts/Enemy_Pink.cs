using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Pink : Enemy_Controller
{

    private float attackTimer;
    private Transform target;
    private float knockbackTime = .1f;
    private float knockbackTimer;
    protected override void AwakeInit()
    {
        currentState = EnemyState.IDLE;
        attackTimer = timeBetweenAttacks;
        anim = GetComponent<Animator>();
        knockbackTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (knockbackTimer > 0)
            knockbackTimer -= Time.deltaTime;
        switch (currentState)
        {
            case EnemyState.IDLE:
                break;
            case EnemyState.MIANDERING:
                Miander();

                Vector2 dir = target.position - transform.position;
                dir.Normalize();

                anim.SetFloat("Horizontal", dir.x);
                anim.SetFloat("Vertical", dir.y);
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0f)
                {
                    currentState = EnemyState.ATTACKING;
                    for (int i = 0; i < shots.Count; i++)
                    {
                        shots[i].GetPowerup<PowerUp_ExtraShot>().Shoot(this, dir, true);
                    }
                    anim.SetTrigger("Attack");
                    if (knockbackTimer <= 0)
                    {
                        body.velocity = Vector2.zero;
                    }
                }
                break;
            case EnemyState.ATTACKING:
                if (knockbackTimer <= 0)
                {
                    body.velocity = Vector2.zero;
                }
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Fire"))
                {
                    currentState = EnemyState.MIANDERING;
                    attackTimer = timeBetweenAttacks;
                }
                break;
            default:
                break;
        }
    }

    private void Miander()
    {
        if (knockbackTimer <= 0f)
            body.velocity = Random.insideUnitCircle*2;
    }

    public void EnteredTriggerRange(Collider2D col)
    {
        if (currentState == EnemyState.IDLE)
        {
            currentState = EnemyState.MIANDERING;
        }
        target = col.transform;
    }

    public void ExitedTriggerRange(Collider2D col)
    {
        if (currentState != EnemyState.DEAD)
        {
            body.velocity = Vector2.zero;
            attackTimer = timeBetweenAttacks;
            currentState = EnemyState.IDLE;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.layer)
        {
            case 7: //player bullet
                Bullet bullet = collision.GetComponent<Bullet>();
                health -= bullet.damage;

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
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("Finished"))
        { 
            yield return null;
        }
        drops.CalculateDrop().Instantiate(transform.position);
        Destroy(gameObject);
    }

}
