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

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (LayerMask.NameToLayer("Player") == gameObject.layer)
        {
            healthBar.GetComponent<Image>().fillAmount = currentHealth / maxHealth;
            healthBarNumber.GetComponent<TextMeshProUGUI>().text = currentHealth.ToString("F0");
        }
    }

    public void TakeDamage(float damageToApply)
    {
        currentHealth -= damageToApply;

        if (currentHealth <= 0)
        {
            
            if(this.gameObject.layer == 3)
            {
                Destroy(this.gameObject);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
}
