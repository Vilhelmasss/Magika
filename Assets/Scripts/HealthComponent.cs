using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float currentHealth;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject healthBarNumber;
    public Slider healthSlider;
    public Slider easeHealthSlider;
    private float lerpSpeed = 0.005f;

    private void Awake()
    {
        currentHealth = maxHealth;
        if (gameObject.layer != 3)
        {
            healthSlider.value = healthSlider.maxValue = maxHealth;
            easeHealthSlider.maxValue = healthSlider.maxValue;
        }
    }

    void Update()
    {
        UpdateUI();
        if (gameObject.layer != 3) {
            UpdateHealthBar();
        }
    }

    private void UpdateUI()
    {
        if (LayerMask.NameToLayer("Player") == gameObject.layer)
        {
            healthBar.GetComponent<Image>().fillAmount = currentHealth / maxHealth;
            healthBarNumber.GetComponent<TextMeshProUGUI>().text = currentHealth.ToString("F0");
        }
    }
    private void UpdateHealthBar()
    {
        if (healthSlider.value != currentHealth)
        {
            healthSlider.value = currentHealth;

        }
        if (healthSlider.value != easeHealthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, currentHealth, lerpSpeed);
        }
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(float damageToApply)
    {
        currentHealth -= damageToApply;

        if (currentHealth <= 0)
        {

            if (this.gameObject.layer == 3)
            {
                Destroy(this.gameObject);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else
            {

            }
        }
        UpdateUI();
    }
}
