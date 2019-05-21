using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spline
{
    [AddComponentMenu("Spline/Spline Decorator")]
    [RequireComponent(typeof(BezierSpline))]
    public class SplineDecorator : MonoBehaviour
    {
        private BezierSpline path;

        [Tooltip("Number of items to spawn along the spline")]
        public int frequency;

        [Tooltip("Have the vertical axe be the splines normal or the world up")]
        public bool localOffset = false;
        [Tooltip("How much you should move the decorations vertically")]
        public float heightOffset;

        public GameObject[] items;

        [Tooltip("Apply curves rotation to the objects instantiated")]
        public bool curveRotation = false;

        //public bool distanceBased = false;

        private void Awake()
        {
            path = GetComponent<BezierSpline>();

            if (frequency <= 0 || items.Length == 0 || path == null)
            {
                return;
            }

            int index;
            float step = 1f / ((float)frequency + 1f);
            Vector3 pos;
            Vector3 localDir;
            GameObject go;

            for(int i = 1; i <= frequency; i++)
            {
                index = Random.Range(0, items.Length - 1);
                pos = path.GetPoint(i * step);

                if (localOffset)
                {
                    localDir = path.GetNormal(i * step);

                    go = Instantiate(items[index], pos + (heightOffset * localDir) , items[index].transform.rotation, transform);
                }
                else
                {
                    go = Instantiate(items[index], pos + new Vector3(0,heightOffset,0), items[index].transform.rotation, transform);
                }

                //Turns object to go in the direction of the bezier curve
                if (curveRotation)
                {
                    go.transform.forward = path.GetDirection(i * step);
                }
            }
        }
    }
}
