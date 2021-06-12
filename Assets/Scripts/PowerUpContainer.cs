using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpContainer : MonoBehaviour
{
    public List<GameObject> powerups;

    private void Awake()
    {
        powerups = new List<GameObject>();
        InitializeSerializedPowerups();
    }

    private void InitializeSerializedPowerups()
    {

    }

    public void UpdateScales()
    {
        float size = (1f / transform.lossyScale.x);
        foreach (GameObject go in powerups)
        {
            go.transform.localScale = Vector3.one * size;
        }
    }

    public void CreatePowerUpVisual(Sprite sprite)
    {
        GameObject visual = new GameObject("Power Up " + sprite.name);
        SpriteRenderer rend = visual.AddComponent<SpriteRenderer>();
        rend.sprite = sprite;
        rend.sortingOrder = 9;
        visual.transform.SetParent(transform);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = Vector3.one * (1f / transform.lossyScale.x);
        powerups.Add(visual);
    }
}
