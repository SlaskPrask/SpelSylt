using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AbsorbText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public GameObject player;
    private string currentString;
    private RectTransform transform;
    private float textSpeed = 0.2f;
    private float fadeTime = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        text.fontMaterial.color = Color.white;
        transform = gameObject.GetComponent<RectTransform>();
        transform.anchoredPosition = new Vector2(player.transform.position.x, player.transform.position.y - 10);
        //SetText(GetText());
        //text.SetText(currentString);
        
        text.SetText("Explosive bullet acquired!");
        StartCoroutine(FadeCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        transform.anchoredPosition = new Vector2(transform.anchoredPosition.x, transform.anchoredPosition.y + textSpeed);
        if (text.fontMaterial.color == Color.clear)
        {
            gameObject.SetActive(false);
        }
    }

    //public string GetText()
    //{
    //    //run when player absorbs powerup
    //}
    public void SetText(string newText)
    {
        currentString = newText;
    }
    IEnumerator FadeCoroutine()
    {
        float waitTime = 0;
        while (waitTime < 1)
        {
            text.fontMaterial.SetColor("_FaceColor", Color.Lerp(Color.white, Color.clear, waitTime));
            yield return null;
            waitTime += Time.deltaTime / fadeTime;
        }
    }
}
