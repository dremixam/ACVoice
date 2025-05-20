using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeWobbler : MonoBehaviour
{
    public float speed = 1.0f; //how fast it shakes
    public float amount = 1.0f; //how much it shakes


    void Update()
    {
        float size = 1 - Mathf.Sin(Time.time * speed) * amount;
        transform.localScale = new Vector3(size, size, 1);
    }
}
