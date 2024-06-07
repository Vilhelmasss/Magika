using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    public float maxHitpoints = 100f;
    public float hitpoints;
    [SerializeField] Slider hpSlider;
    void Start()
    {
        hitpoints = maxHitpoints;
        this.TakeDamage(30f);
    }

    public void TakeDamage(float damage)
    {
        hitpoints -= damage;
        if(hitpoints <= 0)
        {
            Destroy(gameObject);
        }
        UpdateUI();
    }
    private void UpdateUI()
    {
        hpSlider.value = hitpoints / maxHitpoints;
    }
    public void Bleed()
    {
        StartCoroutine(IBleedTicking());
    }

    private IEnumerator IBleedTicking()
    {
        for (int i = 0; i < 5; i++)
        {
            TakeDamage(5);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
