using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject optionsMenu;
    void Start()
    {
        optionsMenu.SetActive(false);
    }

}
