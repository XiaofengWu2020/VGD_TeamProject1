using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeColliders : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        GetComponent<TerrainCollider>().enabled = false;
        GetComponent<TerrainCollider>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
