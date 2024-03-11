using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Spell : MonoBehaviour
{
    public SpellScriptableObject SpellToCast;
    private SphereCollider myCollider;
    private Rigidbody myRigidBody;

    private void Awake()
    {
        myCollider=GetComponent<SphereCollider>();
        myCollider.isTrigger = true;
        myCollider.radius=SpellToCast.SpellRadius;

        myRigidBody = GetComponent<Rigidbody>();
        myRigidBody.isKinematic = true;
        //transform.rotation = Quaternion.LookRotation(transform.forward);

        Destroy(this.gameObject, SpellToCast.LifeTime);

    }
    private void Update()
    {
        if(SpellToCast.Speed > 0)
        {
            transform.Translate(Vector3.forward * SpellToCast.Speed*Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //apply hit particles
        //apply  spell effects
        //apply sounds
        if(other.gameObject.CompareTag("Enemy"))
        {
            HealthComponent enemyHealth = other.GetComponent<HealthComponent>();
            enemyHealth.TakeDamage(SpellToCast.DamageAmmount);

        }
        Destroy(this.gameObject);
    }
}
