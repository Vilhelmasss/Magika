using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealingFountain : MonoBehaviour
{
    [SerializeField] private float healAmount = 10f;
    [SerializeField] private float healFrequency = 3f;
    [SerializeField] private float currHealCharge = 0f;

    void Start()
    {
        currHealCharge -= healFrequency;
    }

    void Update()
    {
        currHealCharge -= Time.deltaTime;
        if (currHealCharge < 0f) {
            currHealCharge = healFrequency;
            foreach (var target in healthComponents)
            {
                target.TakeDamage(-healAmount);
            }
        }
    }
    public List<HealthComponent> healthComponents = new List<HealthComponent>();
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            healthComponents.Add(other.gameObject.GetComponent<HealthComponent>());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            healthComponents.Remove(other.gameObject.GetComponent<HealthComponent>());
        }
    }



}
