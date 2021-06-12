using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupSlot : MonoBehaviour
{
    private Image frameRenderer;
    private Image itemRenderer;
    public bool hasItem { get; private set; }
    public void Initialize(Color def, Color sel, Sprite defS, Sprite selS)
    {
        itemRenderer = transform.GetChild(1).GetComponent<Image>();
        frameRenderer = transform.GetChild(2).GetComponent<Image>();

        if (SerializedData.TryGetPowerupSprite(transform.GetSiblingIndex(), out Sprite sprite))
        {
            hasItem = true;
            itemRenderer.sprite = sprite;
            itemRenderer.color = Color.white;

            if (transform.GetSiblingIndex() == SerializedData.GetStat(PlayerStats.SELECTED_SLOT))
            {
                frameRenderer.sprite = selS;
                frameRenderer.color = sel;
            }
            else
            {
                frameRenderer.sprite = defS;
                frameRenderer.color = def;
            }
        }
        else
        {
            hasItem = false;
            itemRenderer.color = Color.clear;
            frameRenderer.sprite = defS;
            frameRenderer.color = def;
        }
    }

    public void SetItem(Sprite sprite)
    {
        itemRenderer.sprite = sprite;
        hasItem = sprite != null;
        itemRenderer.color = hasItem ? Color.white : Color.clear;
    }

    public void SetFrame(Color col, Sprite frame)
    {
        frameRenderer.sprite = frame;
        frameRenderer.color = col;
    }

    public void MoveData(PowerupSlot slot)
    {
        itemRenderer.sprite = slot.itemRenderer.sprite;
        itemRenderer.color = slot.itemRenderer.color;
        hasItem = slot.hasItem;
    }

    public void ClearData()
    {
        itemRenderer.sprite = null;
        itemRenderer.color = Color.clear;
        hasItem = false;
    }
}
