using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public GameObject bulletPrefab;
    public GameObject powerPrefab;
    private void Awake()
    {
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

    private void Initialize()
    {
        DontDestroyOnLoad(gameObject);
    }
}
