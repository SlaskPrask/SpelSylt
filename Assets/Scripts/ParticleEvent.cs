using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleEvent : MonoBehaviour
{
    ParticleSystem particle;
    bool triggeredOnEnd = false;
    public UnityEvent onStopped = new UnityEvent();
    
    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
    }

    private void LateUpdate()
    {
        if (!triggeredOnEnd && particle.isPlaying == false)
        {
            triggeredOnEnd = true;
            onStopped.Invoke();
        }
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
