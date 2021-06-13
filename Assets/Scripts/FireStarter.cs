using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class FireStarter : MonoBehaviour
{
    void Start()
    {
        GetComponent<Animator>().Play("torch_anim", 0, UnityEngine.Random.Range(0f, 1f));
        Destroy(this);
    }
}
