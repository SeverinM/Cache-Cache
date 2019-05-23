using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandeRoue : MonoBehaviour
{
    [SerializeField]
    BoutonGrandeRoue Bouton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Bouton.turnTheWheel)
        {
            this.transform.Rotate(1, 0, 0);
        }
    }
}
