using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    private RectTransform healthBarTransform;
    Vector2 healthBarAnchored;

    private Player_Controller pc;
    private Coroutine healthRoutine;

    public void Awake()
    {
        pc = GetComponentInParent<Player_Controller>();
        healthBar.value = SerializedData.GetStat(PlayerStats.CURRENT_HP) / SerializedData.GetStat(PlayerStats.MAX_HP);
        healthBarTransform = healthBar.transform as RectTransform;
        healthBarAnchored = healthBarTransform.anchoredPosition;
    }

    private void OnEnable()
    {
        pc.onHealthChange.AddListener(UpdateHealthBar);
    }

    private void OnDisable()
    {
        pc.onHealthChange.RemoveListener(UpdateHealthBar);
    }

    private void UpdateHealthBar(float hp, float maxHp)
    {
        if (healthRoutine != null)
        {
            StopCoroutine(healthRoutine);
        }
        healthRoutine = StartCoroutine(JuiceHealthBar(hp / maxHp));
    }

    IEnumerator JuiceHealthBar(float percentage)
    {
        float shakeAmount = Mathf.Max(0f, healthBar.value - percentage) * 250;
        float t = 0f;
        while (t < .25f)
        {
            healthBarTransform.anchoredPosition = healthBarAnchored + Random.insideUnitCircle * shakeAmount;
            healthBar.value = Mathf.Lerp(healthBar.value, percentage, t / .25f);

            t += Time.deltaTime;
            yield return null;
        }

        healthBarTransform.anchoredPosition = healthBarAnchored;
        healthBar.value = percentage;
    }
}
