using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpContainer : MonoBehaviour
{
    public List<GameObject> powerups;

    private void Awake()
    {
        powerups = new List<GameObject>();
    }

    public void CreatePowerUpVisual()
    {

    }
}
