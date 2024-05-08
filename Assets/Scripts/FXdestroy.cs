using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXdestroy : MonoBehaviour
{
    public float LifeTime = 1.5f;
    void Start()
    {
        Invoke("destroyMe", LifeTime);
    }

    void destroyMe()
    {
        Destroy(this.gameObject);
    }
}
