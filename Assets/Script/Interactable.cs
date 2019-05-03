using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField]
    bool blockAtFirst;
    public bool BlockAt => blockAtFirst;

    public abstract void MouseDown(int id);
    public abstract void MouseUp(int id);
    public abstract void MouseMove(int id, Vector3 delta);
    public abstract void MouseEnter(int id);
    public abstract void MouseLeave(int id);
}
