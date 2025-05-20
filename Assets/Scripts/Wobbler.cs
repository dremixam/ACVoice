using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobbler : MonoBehaviour
{
    public float speed = 1.0f; //how fast it shakes
    public float amount = 1.0f; //how much it shakes

    private float initialHeight;

    private void Start()
    {
        initialHeight = transform.position.y;
    }

    void Update()
    {
        transform.position =  new Vector3(transform.position.x, initialHeight + Mathf.Sin(Time.time * speed) * amount, transform.position.z);
    }
}
