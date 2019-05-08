using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWaypoints : MonoBehaviour
{
    [System.Serializable]
    struct Waypoint
    {
        public Transform relativeDestination;
        public float duration;
        public AnimationCurve curve;
    }

    [SerializeField]
    List<Waypoint> allWaipoints;

    bool ended = true;
    public delegate void noParams();

    public void StartNextWaypoint(noParams atEnd)
    {
        if (ended)
        {
            StartCoroutine(StartAnimationCamera());
        }
    }

    IEnumerator StartAnimationCamera()
    {
        yield return null;
    }
}
