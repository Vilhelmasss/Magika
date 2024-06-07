using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{

    void Update()
    {
        gameObject.transform.Translate(transform.forward * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 3)
        {
            collision.gameObject.GetComponent<HP>().TakeDamage(10f);
            collision.gameObject.GetComponent<HP>().Bleed();
            Destroy(gameObject);
        }
    }
}
