using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PowerUp_ExtraShot : PowerUp
{
    public float baseFireRate = 2f;
    public float baseFireSpeed = 4f;
    public float baseSize = .75f;
    public ShotDirection shotDirections = ShotDirection.UP;
    public Color color = Color.white;
    [NonSerialized] private float lastShotTime;

    public void Shoot(Entity_Controller shooter, Vector2 shotDirection)
    {
        if (lastShotTime - Time.time > 0f)
            return;

        for (int i = 1; i <= 128; i += i)
        {
            if (shotDirections.HasFlag((ShotDirection)i))
            {
                float dir = Mathf.Log(i, 2);
                Vector2 shotDir = Quaternion.AngleAxis(45 * dir, Vector3.forward) * shotDirection;
                Bullet bullet = UnityEngine.Object.Instantiate(GameManager.instance.bulletPrefab, (Vector2)shooter.transform.position + shotDir * (SerializedData.GetStat(PlayerStats.SIZE) * .35f), Quaternion.identity).GetComponent<Bullet>();
                bullet.transform.localScale = Vector3.one * baseSize;
                string tag;
                if (shooter.gameObject.layer == 6)
                {
                    tag = "Player";
                }
                else
                {
                    tag = "Enemy";
                }

                bullet.Initialize(shotDir * baseFireSpeed, color, tag);
            }
        }
        lastShotTime = Time.time + 1f / baseFireRate;
    }
}

[Flags]
public enum ShotDirection : short
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