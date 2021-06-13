using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SerializedData.InitalStats();
        GameManager.instance.sceneSwitcher.SwitchScene("Oskars Scene");
    }

    public void Options()
    {
        GameManager.instance.optionsMenu.SetActive(true);
    }

    public void Quit()
    {
        GameManager.instance.sceneSwitcher.QuitGame();
    }
}
