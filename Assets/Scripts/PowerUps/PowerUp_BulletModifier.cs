using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PowerUp_BulletModifier : PowerUp
{
    public ShotModifiers shotModifiers = new ShotModifiers
    {
        sizeAdder = 0f,
        sizeMultiplier = 1f,
        
        speedAdder = 0f,
        speedMultiplier = 1f,
        
        rateAdder = 0f,
        rateMultiplier = 1f
    };
}
