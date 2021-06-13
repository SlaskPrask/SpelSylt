using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Controller : Entity_Controller
{
    //Logic is for pink
    [SerializeField] protected float timeBetweenAttacks = 2f;
    [SerializeField] protected float health = 5;
    [SerializeField] protected float knockback = 10;
    public float shotStart = .25f;
    public List<PowerUp_Scriptable> shots = new List<PowerUp_Scriptable>();
    public DropTable drops;
    
    protected EnemyState currentState;
    protected Animator anim;


    /*
     Constantly idle
     On player close it mianders and shoots towards player
     */

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.IDLE:
                break;
            case EnemyState.PATROLLING:
                break;
            case EnemyState.MIANDERING:
                break;
            case EnemyState.HUNTING:
                break;
            case EnemyState.ATTACKING:
                break;
            default:
                break;
        }
    }

}

public enum EnemyState
{
    IDLE = 0,
    PATROLLING = 1,
    MIANDERING = 2,
    HUNTING = 3,
    ATTACKING = 4,
    DAMAGED = 5,
    DEAD = 6,
  
}