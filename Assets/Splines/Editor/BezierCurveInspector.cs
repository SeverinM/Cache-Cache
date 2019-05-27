using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Spline
{
    [CustomEditor(typeof(BezierCurve))]
    public class BezierCurveInspector : Editor
    {
        private BezierCurve curve;
        private Transform handleTransform;
        private Quaternion handleRotation;

        private const int lineSteps = 10;
        private const float directionScale = 0.5f;

        private void OnSceneGUI()
        {
            curve = target as BezierCurve;
            handleTransform = curve.transform;
            handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

            Vector3 point0 = ShowPoint(0);
            Vector3 point1 = ShowPoint(1);
            Vector3 point2 = ShowPoint(2);
            Vector3 point3 = ShowPoint(3);

            Handles.color = Color.grey;
            Handles.DrawLine(point0, point1);
            Handles.DrawLine(point2, point3);
            ShowDirections();
            Handles.DrawBezier(point0, point3, point1, point2, Color.white, null, 2f);
        }

        private void ShowDirections()
        {
            Handles.color = Color.green;
            Vector3 point = curve.GetPoint(0f);
            Handles.DrawLine(point, point + curve.GetDirection(0f) * directionScale);
            for(int i = 1; i <= lineSteps; i++)
            {
                point = curve.GetPoint(i / (float)lineSteps);
                Handles.DrawLine(point, point + curve.GetDirection(i / (float)lineSteps) * directionScale);
            }
        }

        private Vector3 ShowPoint(int index)
        {
            Vector3 point = handleTransform.TransformPoint(curve.points[index]);
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curve, "MovePoint");
                EditorUtility.SetDirty(curve);
                curve.points[index] = handleTransform.InverseTransformPoint(point);
            }
            return point;
        }
    }
}
