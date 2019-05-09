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
        public Transform lookAt;
    }

    [SerializeField]
    List<Waypoint> allWaypoints;

    bool ended = true;
    public delegate void noParams();

    public void StartNextWaypoint(noParams atEnd)
    {
        if (ended && allWaypoints.Count > 0)
        {
            //act like a stack (topmost first);
            StartCoroutine(StartAnimationCamera(allWaypoints[0], atEnd));
            allWaypoints.RemoveAt(0);
        }
    }

    IEnumerator StartAnimationCamera(Waypoint currentWaypoint, noParams end)
    {
        ended = false;
        float normalizedTime = 0;
        Vector3 forwardOrigin = transform.forward;
        Vector3 originPosition = transform.position;
        Vector3 destinationPosition = currentWaypoint.relativeDestination.position;
        Vector3 destinationForward = (currentWaypoint.lookAt ? currentWaypoint.lookAt.transform.position - transform.position : currentWaypoint.relativeDestination.forward);

        while (normalizedTime < 1)
        {
            normalizedTime += Time.deltaTime / currentWaypoint.duration;
            transform.position = Vector3.Lerp(originPosition, destinationPosition, currentWaypoint.curve.Evaluate(normalizedTime));
            transform.forward = Vector3.Lerp(forwardOrigin, destinationForward, currentWaypoint.curve.Evaluate(normalizedTime));
            yield return null;
        }

        end();
        ended = true;
    }
}
