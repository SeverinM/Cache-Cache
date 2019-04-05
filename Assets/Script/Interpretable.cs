using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class Interpretable : MonoBehaviour
{
    [SerializeField]
    protected GameObject prefab;

    public abstract void Interpret(Vector3 position, GameObject gob);
}
