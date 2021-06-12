using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp_Object : MonoBehaviour
{
    public PowerUp_Scriptable powerup;
    [SerializeField] private SpriteRenderer sprite;

    public void Initialize(PowerUp_Scriptable powerup)
    {
        this.powerup = powerup;
        sprite.sprite = powerup.sprite;
    }

    private void OnValidate()
    {
        Initialize(powerup);
    }
}
