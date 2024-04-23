using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeavesFalling : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        // gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, PlayerController.Instance.gameObject.transform.position, 1);
    }
}
