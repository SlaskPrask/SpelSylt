using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HintSystem : MonoBehaviour
{
    public GameObject player;
    public Collider2D collider;
    public TextMeshProUGUI textBox;
    public byte textCount = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //transform.anchoredPosition = new Vector2(transform.anchoredPosition.x, transform.anchoredPosition.y);
        if (collider.IsTouching(player.GetComponent<Collider2D>()))
        {
            switch (textCount)
            {
                case 0:
                    textBox.SetText("W/A/S/D TO MOVE\nARROW KEYS TO SHOOT");
                    break;
                case 1:
                    textBox.SetText("SPACE TO\nABSORB");
                    break;
                case 2:
                    textBox.SetText("TAB TO CYCLE POWERUPS\nQ TO DIGEST");
                    break;
                case 3:
                    textBox.SetText("MONSTERS HAVE TYPINGS\nYOU ARE WHAT YOU EAT");
                    break;
            }
            textBox.gameObject.SetActive(true);
        }
        else
        {
            textBox.gameObject.SetActive(false);
        }
    }
}
