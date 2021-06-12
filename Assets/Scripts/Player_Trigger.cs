using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player_Trigger : MonoBehaviour
{
    public UnityEvent<Collider2D> onTriggerEnter = new UnityEvent<Collider2D>();
    public UnityEvent<Collider2D> onTriggerExit = new UnityEvent<Collider2D>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        onTriggerEnter.Invoke(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        onTriggerExit.Invoke(collision);
    }
}
