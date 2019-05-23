using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nasselle : MonoBehaviour
{
    [SerializeField]
    Transform NasselleSpot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = NasselleSpot.position;
    }
}
