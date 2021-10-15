using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnedObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("Second gem is Awake");
    }
    void OnEnable()
    {
        Debug.Log("Second gem is Enabled");
    }
    void Start()
    {
        Debug.Log("Second gem is Started");
    }
}
