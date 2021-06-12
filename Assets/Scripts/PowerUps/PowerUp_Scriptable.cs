using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Shot", menuName = "Power Up")]
public class PowerUp_Scriptable : ScriptableObject
{
    public PowerUpType type;
    [SerializeReference]
    public PowerUp power;
    public Sprite sprite;

    public T GetPowerup<T>() where T : PowerUp
    {
        return (T)power;
    }
}

[System.Serializable]
public class PowerUp
{
    public PowerUpType type;
}

public enum PowerUpType
{
    EXTRA_SHOT,
    SHOT_MODIFIER,
    STAT_MODIFIER,
    ELEMENTAL_TYPING,
    FAMILIAR
}