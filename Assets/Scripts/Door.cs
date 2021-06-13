using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Door : MonoBehaviour
{
    [SerializeField] private Sprite onOpen;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            if (SerializedData.HasKey(KeyItemType.KEY, out int index))
            {
                collision.gameObject.GetComponent<Player_Controller>().ForceDigest(index);
                GetComponentInChildren<SpriteRenderer>(true).sprite = onOpen;
                Destroy(GetComponent<BoxCollider2D>());
                RuntimeManager.PlayOneShotAttached("event:/SfX/WetExplosion", gameObject);
                Destroy(this);
            }
        }
    }
}
