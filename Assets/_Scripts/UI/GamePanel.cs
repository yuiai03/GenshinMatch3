using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    public GameObject Menu;
    public GameObject MatchedTilesViewHolder;

    public Slider PlayerHPBar;
    public Slider EnemyHPBar;

    private float playerMaxHP;
    private float enemyMaxHP;
    private Coroutine playerHPUpdateCoroutine;
    private Coroutine enemyHPUpdateCoroutine;

    private void Awake()
    {
        Menu.SetActive(false);
    }

    public void SetPlayerMaxHealth(float maxHealth)
    {
        playerMaxHP = maxHealth;
        PlayerHPBar.maxValue = maxHealth;
        PlayerHPBar.value = maxHealth;
    }

    public void SetEnemyMaxHealth(float maxHealth)
    {
        enemyMaxHP = maxHealth;
        EnemyHPBar.maxValue = maxHealth;
        EnemyHPBar.value = maxHealth;
    }

    public void UpdatePlayerHealth(float currentHealth)
    {
        if (playerHPUpdateCoroutine != null)
            StopCoroutine(playerHPUpdateCoroutine);

        playerHPUpdateCoroutine = StartCoroutine(SmoothHealthUpdate(PlayerHPBar, currentHealth));
    }

    public void UpdateEnemyHealth(float currentHealth)
    {
        if (enemyHPUpdateCoroutine != null)
            StopCoroutine(enemyHPUpdateCoroutine);

        enemyHPUpdateCoroutine = StartCoroutine(SmoothHealthUpdate(EnemyHPBar, currentHealth));
    }

    private IEnumerator SmoothHealthUpdate(Slider healthBar, float targetValue)
    {
        float startValue = healthBar.value;
        float elapsedTime = 0f;
        float duration = 0.5f; // Duration of the animation in seconds

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            healthBar.value = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            yield return null;
        }

        healthBar.value = targetValue;
    }
}
