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
                3f, //Max Speed
                10f, //Max HP
                1f, //Size
                1f, //Fire rate
                1f, //Absorbtion cooldown
                25f, //Acceleration
                10f, //Deceleration
            },
            shots = new List<PowerUp_ExtraShot>()
            {
                Resources.Load<PowerUp_Scriptable>("Base Shot").GetPowerup<PowerUp_ExtraShot>()
            }
        };
    }

    public static List<PowerUp_ExtraShot> GetShots()
    {
        return activeData.shots;
    }

    public static void UpdateStat(PlayerStats stat, float value)
    {
        activeData.data[(byte)stat] = value;
    }

    public static float GetStat(PlayerStats stat)
    {
        return activeData.data[(byte)stat];
    }

    [System.Serializable]
    public struct PlayerData
    {
        public float[] data;
        public List<PowerUp_ExtraShot> shots;
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