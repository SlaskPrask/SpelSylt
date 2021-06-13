using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.SceneManagement;

public class scene_transition : MonoBehaviour
{
    // Start is called before the first frame update
    void sceneTransition() {
        SceneManager.LoadScene("Main Menu");
    }

    void playSFX1() {
        RuntimeManager.PlayOneShot("event:/SfX/GoopHurt");
    }

    void playSFX2() {
        RuntimeManager.PlayOneShot("event:/SfX/Absorb");
    }

    void playSFX3() {
        RuntimeManager.PlayOneShot("event:/SfX/SpookyActivate");
    }
}
