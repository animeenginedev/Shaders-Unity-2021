using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkAsStatic : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Destroy(this);
    }

}
