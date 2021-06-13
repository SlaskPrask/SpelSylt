using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp_KeyItem : PowerUp
{
    public KeyItemType itemType;
}

public enum KeyItemType
{
    KEY = 0,
}