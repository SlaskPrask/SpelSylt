using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Shot", menuName = "Power Up/Shot")]
public class Power_Shot : PowerUpBase
{
    public float baseFireRate;
    public float baseFireSpeed;
    public ShotDirection shotDirections;
    public Color color;
    public bool addativeToMain;
    public bool addativeToSub;
    [NonSerialized] public float cooldown;

    public void Shoot(GameObject shooter, Vector2 shotDirection)
    {
        for (int i = 1; i <= 128; i += i)
        {
            if (shotDirections.HasFlag((ShotDirection)i))
            {
                float dir = Mathf.Log(i, 2);
                Debug.Log(dir);
                Bullet bullet = Instantiate(GameManager.instance.bulletPrefab, shooter.transform.position, Quaternion.identity).GetComponent<Bullet>();
                bullet.Initialize((Quaternion.AngleAxis(45 * dir, Vector3.forward) * (Vector3)shotDirection ) * baseFireSpeed, color);
            }
        }
        
        
    }
}

[Flags]
public enum ShotDirection : byte
{
    UP = 1,
    UP_RIGHT = 2,
    RIGHT = 4,
    DOWN_RIGHT = 8,
    DOWN = 16,
    DOWN_LEFT = 32,
    LEFT = 64,
    UP_LEFT = 128
}