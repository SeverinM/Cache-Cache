using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Spline
{
    [CustomEditor(typeof(Line))]
    public class LineInspector : Editor
    {
        private void OnSceneGUI()
        {
            Line line = target as Line;

            Transform handleTransform = line.transform;
            Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

            Vector3 point0 = handleTransform.TransformPoint(line.point0);
            Vector3 point1 = handleTransform.TransformPoint(line.point1);

            Handles.color = Color.white;
            Handles.DrawLine(line.point0, line.point1);

            EditorGUI.BeginChangeCheck();
            point0 = Handles.DoPositionHandle(point0, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(line, "Move Point 0");
                EditorUtility.SetDirty(line);
                line.point0 = handleTransform.InverseTransformPoint(point0);
            }

            EditorGUI.BeginChangeCheck();
            point1 = Handles.DoPositionHandle(point1, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(line, "Move Point 1");
                EditorUtility.SetDirty(line);
                line.point1 = handleTransform.InverseTransformPoint(point1);
            }
        }
    }
}
