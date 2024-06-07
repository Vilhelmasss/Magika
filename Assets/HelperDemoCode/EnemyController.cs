using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform target;
    void Start()
    {
        // target = PlayerController.Instance.transform;
    }


    void FixedUpdate()
    {
        gameObject.transform.LookAt(target.position);
        gameObject.transform.Translate(target.position * Time.deltaTime * 1/10);
    }
}
