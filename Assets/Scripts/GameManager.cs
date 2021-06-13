using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public GameObject bulletPrefab;
    public GameObject powerPrefab;
    public GameObject bulletBreakPrefab;
    public GameObject optionsMenu;
    public SceneSwitcher sceneSwitcher;
    private bool paused;
    private void Awake()
    {
        paused = false;
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Initialize();
    }

    private void Update()
    {

    }

    public static void PauseGame()
    {
        if (instance.paused == false)
        {
            Time.timeScale = 0;
            instance.optionsMenu.SetActive(true);
            //Disable scripts that still work while timescale is set to 
            instance.paused = true;
        }

        else
        {
            Time.timeScale = 1;
            instance.optionsMenu.SetActive(false);
            //enable the scripts again
            instance.paused = false;
        }
    }

    private void Initialize()
    {
        DontDestroyOnLoad(gameObject);
    }
}
