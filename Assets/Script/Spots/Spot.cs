using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spot : MonoBehaviour
{
    //Le drag rentre dans le spot
    public abstract void CursorEnter(GameObject gob);

    //Le drag quitte le spot
    public abstract void CursorExit(GameObject gob);

    //Le drag prend fin
    public abstract void CursorRelease(GameObject gob);

    public abstract void SetState(bool value);
}
