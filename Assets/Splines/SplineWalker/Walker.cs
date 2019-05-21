using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spline
{
    [AddComponentMenu("Spline/Spline Walker")]
    public class Walker : MonoBehaviour
    {
        public BezierSpline path;

        public float duration;

        private float progress;

        public bool lookForwards;

        public WalkerMode mode;

        public bool backAndForthAlwaysForward;

        private bool goingForward = true;

        private void Update()
        {
            if (goingForward)
            {
                progress += Time.deltaTime / duration;
                if (progress > 1f)
                {
                    if(mode == WalkerMode.Once)
                    {
                        progress = 1f;
                    }
                    else if(mode == WalkerMode.Loop)
                    {
                        progress -= 1f;
                    }
                    else
                    {
                        progress = 2f - progress;
                        goingForward = false;
                    }
                }
            }
            else
            {
                progress -= Time.deltaTime / duration;
                if(progress < 0)
                {
                    progress = -progress;
                    goingForward = true;
                }
            }
            Vector3 position = path.GetPoint(progress);
            transform.localPosition = position;
            if (!backAndForthAlwaysForward)
            {
                if (lookForwards)
                    transform.LookAt(position + path.GetDirection(progress));
            }
            else
            {
                if(goingForward)
                    transform.LookAt(position + path.GetDirection(progress));
                else
                    transform.LookAt(position - path.GetDirection(progress));
            }
        }
    }
}

