using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
public class RandomPoopSFX : MonoBehaviour
{
    private float timer;
    private float soundTime;
    private float random;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        random = 1;
        soundTime = Random.Range(0, 60 * 8);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer == soundTime)
        {
            RuntimeManager.PlayOneShot("Event:/SFX/RandomPoop");
        }
        if (timer >= 60 * 8)
        {
            timer = 0;
        }
        else timer++;
    }
}
