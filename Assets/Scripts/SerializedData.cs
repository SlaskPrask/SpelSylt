using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SerializedData
{
    private static PlayerData activeData;
    public static bool gamePause;

    public static int PowerCount { get => activeData.powerUps.Count; }
    private static bool shotsChanged;
    private static bool modChanged;
    private static bool impactChanged;

    //Cache stuff
    private static List<PowerUp_ExtraShot> shotsCache;
    private static ShotModifiers modCache;
    private static List<GameObject> impactCache;
    private static bool destroyCache;

    static SerializedData()
    {
        InitalStats();
        shotsChanged = modChanged = impactChanged = true;
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
                50f, //Acceleration
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
        if (shotsChanged)
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

            shotsCache = shots;
            shotsChanged = false;
        }

        return shotsCache;
    }

    private static void ModifyStats(PowerUp_Stat power, bool remove)
    {
        if (remove)
        {
            activeData.data[(int)PlayerStats.ACCELERATION] -= power.moveSpeed;
        }
        else
        {
            activeData.data[(int)PlayerStats.ACCELERATION] += power.moveSpeed;
        }
    }

    public static void RemoveSelectedPowerUp()
    {
        int sel = (int)GetStat(PlayerStats.SELECTED_SLOT);
        
        
        if (activeData.powerUps[sel].type == PowerUpType.STAT_MODIFIER)
        {
            ModifyStats((PowerUp_Stat)activeData.powerUps[sel], true);         
        }

        
        activeData.powerUps.RemoveAt(sel);
        modChanged = impactChanged = shotsChanged = true;
    }

    public static List<GameObject> GetShotHitInitializers(out bool destroyOnImpact)
    {
        if (impactChanged)
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

            destroyCache =  destroy >= 0;
            impactCache = go.Count == 0 ? null : go;
            impactChanged = false;
        }
        destroyOnImpact = destroyCache;
        return impactCache;
    }

    public static ShotModifiers GetShotModifiers()
    {
        if (modChanged)
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
                    mods.destroyOnContact = mods.destroyOnContact && bulletMod.shotModifiers.destroyOnContact;
                
                }
            }
            modChanged = false;
            modCache = mods;
            return mods;
        }
        else
        {
            return modCache;
        }
    }

    public static int AddPowerUp(PowerUp power)
    {
        activeData.powerUps.Add(power);
        if (power.type == PowerUpType.STAT_MODIFIER)
        {
            ModifyStats((PowerUp_Stat)power, false);
        }
        modChanged = impactChanged = shotsChanged = true;
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