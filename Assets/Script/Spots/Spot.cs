using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spot : MonoBehaviour
{
    //Le drag rentre dans le spot
    public abstract void CursorEnter();

    //Le drag quitte le spot
    public abstract void CursorExit();

    //Le drag prend fin
    public abstract void CursorRelease();

}
