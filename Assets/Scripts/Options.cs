using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    FMOD.Studio.Bus Master;
    FMOD.Studio.Bus Bgm;
    FMOD.Studio.Bus Se;
    static float bgmVolume = 0.5f;
    static float seVolume = 0.5f;
    static float masterVolume = 1f;
    public Slider sfxSlider;
    public Slider mSlider;

    void Awake()
    {
        Bgm = RuntimeManager.GetBus("bus:/Master/Music");
        Se = RuntimeManager.GetBus("bus:/Master/Sound");
    }

    void Update()
    {
        Bgm.setVolume(mSlider.value);
        Se.setVolume(sfxSlider.value);
        Master.setVolume(masterVolume);
    }

    public void MasterVolumeLevel(float newMasterVolume)
    {
        masterVolume = newMasterVolume;
    }

    public void BgmVolumeLevel(float newBgmVolume)
    {
        bgmVolume = newBgmVolume;
    }

    public void SeVolumeLevel(float newSeVolume)
    {
        seVolume = newSeVolume;
    }
}
