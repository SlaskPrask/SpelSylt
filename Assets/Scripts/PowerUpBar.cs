using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PowerUpBar : MonoBehaviour
{
    [SerializeField] private Sprite stdFrame;
    [SerializeField] private Sprite selectionFrame;
    [SerializeField] private Color stdColor;
    [SerializeField] private Color selectionColor;
    private List<PowerupSlot> powerSlots;
    private Player_Controller pc;
    

    private void Awake()
    {
        //Temp fetch player controller
        Transform parent = transform.parent;     
        while (!parent.TryGetComponent<Player_Controller>(out pc) && parent != null)
        {
            parent = parent.parent;
        }
        powerSlots = new List<PowerupSlot>();
        for (int i = 0; i < transform.childCount; i++)
        {
            powerSlots.Add(transform.GetChild(i).GetComponent<PowerupSlot>());
            powerSlots[i].Initialize(stdColor, selectionColor, stdFrame, selectionFrame);
        }
    }

    private void OnEnable()
    {
        pc.onAbsorbedItem.AddListener(AddItem);
        pc.onNext.AddListener(NextSlot);
        pc.onNumberInput.AddListener(SwitchPowerSlot);
        pc.onDigest.AddListener(DestroyItem);
    }

    private void OnDisable()
    {
        pc.onAbsorbedItem.RemoveListener(AddItem);
        pc.onNext.RemoveListener(NextSlot);
        pc.onNumberInput.RemoveListener(SwitchPowerSlot);
        pc.onDigest.RemoveListener(DestroyItem);
    }

    private void DestroyItem()
    {
        int selected = (int)SerializedData.GetStat(PlayerStats.SELECTED_SLOT);

        //move all items back after the selected one,
        //if there isn't an item next, dont.
        for (int i = selected; i < 8; i++)
        {
            powerSlots[i].MoveData(powerSlots[i + 1]);
        }
        powerSlots[8].ClearData();

        if (!powerSlots[selected].hasItem)
        {
            SwitchPowerSlot(selected - 1);
        }
    }

    private void AddItem(PowerUp_Object power)
    {
        int currentSlot = (int)SerializedData.GetStat(PlayerStats.SELECTED_SLOT);
        for (int i = currentSlot; i < 9; i++)
        {
            if (!powerSlots[i].hasItem)
            {
                if (i == currentSlot)
                {
                    powerSlots[i].SetFrame(selectionColor, selectionFrame);
                }

                powerSlots[i].SetItem(power.powerup.sprite);
                break;
            }
        }
    }

    private void SwitchPowerSlot(int goTo)
    {
        if (!powerSlots[goTo].hasItem || goTo == (int)SerializedData.GetStat(PlayerStats.SELECTED_SLOT))
            return;
        
        powerSlots[(int)SerializedData.GetStat(PlayerStats.SELECTED_SLOT)].SetFrame(stdColor, stdFrame);
        powerSlots[goTo].SetFrame(selectionColor, selectionFrame);
        SerializedData.UpdateStat(PlayerStats.SELECTED_SLOT, goTo);
        RuntimeManager.PlayOneShot("event:/SfX/CyclePowerupReal");
    }

    private void NextSlot()
    {
        int current = (int)SerializedData.GetStat(PlayerStats.SELECTED_SLOT);
        int next = (current + 1) % 9;

        if (!powerSlots[next].hasItem)
            next = 0;

        if (current == next)
            return;

        powerSlots[current].SetFrame(stdColor, stdFrame);
        powerSlots[next].SetFrame(selectionColor, selectionFrame);
        SerializedData.UpdateStat(PlayerStats.SELECTED_SLOT, next);
        RuntimeManager.PlayOneShot("event:/SfX/CyclePowerupReal");
    }
}
