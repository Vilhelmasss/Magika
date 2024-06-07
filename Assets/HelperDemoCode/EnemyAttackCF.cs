using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public GameObject projectile;
    void Start()
    {
        AttackPlayer();
    }


    void AttackPlayer()
    {
        Instantiate(projectile, gameObject.transform.position, Quaternion.identity);
        Invoke("AttackPlayer", 1f);
    }
}
