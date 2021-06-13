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

    private void PauseGame()
    {
        Time.timeScale = 0;
        optionsMenu.SetActive(true);
        //Disable scripts that still work while timescale is set to 0
    }
    private void ContinueGame()
    {
        Time.timeScale = 1;
        optionsMenu.SetActive(false);
        //enable the scripts again
    }
}
