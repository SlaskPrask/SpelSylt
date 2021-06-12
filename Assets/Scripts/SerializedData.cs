using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SerializedData
{
    private static PlayerData activeData;


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
                50f, //Max Speed
                10f, //Max HP
                3f, //Size
                1f, //Fire rate
                1f, //Absorbtion cooldown
                70f, //Acceleration
                10f, //Deceleration
            },
            baseShot = Resources.Load<PowerUp_Scriptable>("Base Shot").GetPowerup<PowerUp_ExtraShot>(),
            powerUps = new List<PowerUp>()
        };
    }


    public static void UpdateStat(PlayerStats stat, float value)
    {
        activeData.data[(byte)stat] = value;
    }

    public static float GetStat(PlayerStats stat)
    {
        return activeData.data[(byte)stat];
    }

    public static List<PowerUp_ExtraShot> GetShots()
    {
        List<PowerUp_ExtraShot> shots = new List<PowerUp_ExtraShot>();
        shots.Add(activeData.baseShot);

        return shots;
    }

    public static ShotModifiers GetShotModifiers()
    {
        ShotModifiers mods = new ShotModifiers()
        {
            rateMultiplier = 1,
            speedMultiplier = 1,
            sizeMultiplier = 1
        };

        foreach (PowerUp power in activeData.powerUps)
        {
            if (power.type == PowerUpType.SHOT_MODIFIER)
            {
                PowerUp_BulletModifier bulletMod = (PowerUp_BulletModifier)power;
                mods.rateAdder += bulletMod.shotModifiers.rateAdder;
                mods.sizeAdder += bulletMod.shotModifiers.sizeAdder;
                mods.speedAdder += bulletMod.shotModifiers.speedAdder;
                
                mods.rateMultiplier *= bulletMod.shotModifiers.rateMultiplier;
                mods.sizeMultiplier *= bulletMod.shotModifiers.sizeMultiplier;
                mods.speedMultiplier *= bulletMod.shotModifiers.speedMultiplier;
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
    DECELERATION = 6
}