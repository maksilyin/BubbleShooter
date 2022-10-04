using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public float time = 1;
    void Start()
    {
        Invoke("Destr", time);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Destr()
    {
        Destroy(gameObject);
    }
}
