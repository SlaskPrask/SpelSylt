using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SerializedData
{
    private static PlayerData activeData;
    public static bool gamePause;

    public static int PowerCount { get => activeData.powerUps.Count; }

    static SerializedData()
    {
        InitalStats();
    }

    private static void InitalStats()
    {
        activeData = new PlayerData()
        {
            data = new float[]
            {
                40f, //Max Speed
                10f, //Max HP
                3f, //Size
                1f, //Fire rate
                1f, //Absorbtion cooldown
                60f, //Acceleration
                10f, //Deceleration
                10f, //Curren HP
                .5f, //Invincibility Time
                0f, //Knockback resistance
                0f, //Selected slot in powerup bar
                .75f, //Base Damage
            },
            baseShot = Resources.Load<PowerUp_Scriptable>("Base Shot").GetPowerup<PowerUp_ExtraShot>(),
            powerUps = new List<PowerUp>()
        };
    }


    public static void UpdateStat(PlayerStats stat, float value)
    {
        activeData.data[(byte)stat] = value;
    }

    public static bool TryGetPowerupSprite(int index, out Sprite sprite)
    {

        sprite = null;
        return false;
    }

    public static float GetStat(PlayerStats stat)
    {
        return activeData.data[(byte)stat];
    }

    public static List<PowerUp_ExtraShot> GetShots()
    {
        List<PowerUp_ExtraShot> shots = new List<PowerUp_ExtraShot>();
        shots.Add(activeData.baseShot);

        foreach (PowerUp power in activeData.powerUps)
        {
            if (power.type == PowerUpType.EXTRA_SHOT)
            {
                shots.Add((PowerUp_ExtraShot)power);
            }
        }

        return shots;
    }

    public static void RemoveSelectedPowerUp()
    {
        activeData.powerUps.RemoveAt((int)GetStat(PlayerStats.SELECTED_SLOT));
    }

    public static List<GameObject> GetShotHitInitializers(out bool destroyOnImpact)
    {
        List<GameObject> go = new List<GameObject>();
        int destroy = 0;

        foreach (PowerUp power in activeData.powerUps)
        {
            if (power.type == PowerUpType.SHOT_IMPACT_BEHAVIOUR)
            {
                PowerUp_BulletImpact bulletImp = (PowerUp_BulletImpact)power;
                destroy += (bulletImp.destroyOnImpact ? 1 : -1);
                go.Add(bulletImp.instantiateOnHit);
            }
        }

        destroyOnImpact = destroy >= 0;
        return go.Count == 0 ? null : go;
    }

    public static ShotModifiers GetShotModifiers()
    {
        ShotModifiers mods = new ShotModifiers()
        {
            rateMultiplier = 1,
            speedMultiplier = 1,
            sizeMultiplier = 1,
            damageMultiplier = 1,
        };

        foreach (PowerUp power in activeData.powerUps)
        {
            if (power.type == PowerUpType.SHOT_MODIFIER)
            {
                PowerUp_BulletModifier bulletMod = (PowerUp_BulletModifier)power;
                mods.rateAdder += bulletMod.shotModifiers.rateAdder;
                mods.sizeAdder += bulletMod.shotModifiers.sizeAdder;
                mods.speedAdder += bulletMod.shotModifiers.speedAdder;
                mods.damageAdder += bulletMod.shotModifiers.damageAdder;
                
                mods.rateMultiplier *= bulletMod.shotModifiers.rateMultiplier;
                mods.sizeMultiplier *= bulletMod.shotModifiers.sizeMultiplier;
                mods.speedMultiplier *= bulletMod.shotModifiers.speedMultiplier;
                mods.damageMultiplier += bulletMod.shotModifiers.damageMultiplier;
            }
        }

        return mods;
    }

    public static int AddPowerUp(PowerUp power)
    {
        activeData.powerUps.Add(power);
        return activeData.powerUps.Count - 1;
    }

    [System.Serializable]
    public struct PlayerData
    {
        public float[] data;
        public PowerUp_ExtraShot baseShot;
        public List<PowerUp> powerUps;
    }
}

public enum PlayerStats : byte
{
    MAX_SPEED = 0,
    MAX_HP = 1,
    SIZE = 2,
    FIRE_RATE = 3,
    ABSORB_COOLDOWN = 4,
    ACCELERATION = 5,
    DECELERATION = 6,
    CURRENT_HP = 7,
    INVINCIBILITY_TIME = 8,
    KNOCKBACK_RESISTANCE = 9,
    SELECTED_SLOT = 10,
    DAMAGE = 11,
}