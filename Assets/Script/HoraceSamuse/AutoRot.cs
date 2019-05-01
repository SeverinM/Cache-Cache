using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRot : MonoBehaviour
{
    public Vector3 rotationVec = Vector3.up;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationVec * Time.deltaTime, Space.World);
    }
}
