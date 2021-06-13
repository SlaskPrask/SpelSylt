using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour, IDamageSource
{
    [SerializeField] private float damage = 1;

    public float GetDamage()
    {
        return damage;
    }
}
