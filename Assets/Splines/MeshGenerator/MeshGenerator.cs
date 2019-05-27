using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spline
{
    [AddComponentMenu("Spline/Spline Mesh Generator")]
    [RequireComponent(typeof(BezierSpline))]
    public class MeshGenerator : MonoBehaviour
    {
        private BezierSpline path;

        public Material material;

        [Tooltip("The thickness of the mesh")]
        public float meshWidth;
    
        [Min(2)]
        public int subDivisions;

        private void Awake()
        {
            DrawMesh();
        }

        public void DrawMesh()
        {
            path = GetComponent<BezierSpline>();

            GameObject go = new GameObject("Wire Mesh");

            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();

            go.transform.SetParent(transform, false);

            Mesh mesh = new Mesh();

            mesh.vertices = WireVertices();
            mesh.triangles = WireTriangles();
            mesh.RecalculateNormals();

            meshRenderer.material = material;

            meshFilter.sharedMesh = mesh;
        }

        Vector3[] WireVertices()
        {
            Vector3[] vertices = new Vector3[subDivisions * 4];

            for(int z = 0, i = 0; z < subDivisions; z++)
            {
                float t = Mathf.InverseLerp(0, subDivisions - 1, z);
                Vector3 center = path.GetPoint(t);
                Vector3 direction = path.GetDirection(t);

                for(int x = 0; x < 4; x++, i++)
                {
                    float angle = Mathf.InverseLerp(0, 3, x) * 360;
                    Quaternion rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(Vector3.forward * angle);
                    vertices[i] = (rotation * Vector3.up * (meshWidth) + transform.InverseTransformPoint(center));
                }
            }

            return vertices;
        }

        int[] WireTriangles()
        {
            int numTriangles = ((subDivisions - 1) * 4) * 2;
            int[] triangles = new int[numTriangles * 3];

            for(int t = 3, i = 0; t < triangles.Length-6; t += 6, i += 1)
            {
                triangles[t] = i + 4;
                triangles[t + 1] = i;
                triangles[t + 2] = i+1;

                triangles[t + 3] = i + 4 + 1;
                triangles[t + 4] = i + 4;
                triangles[t + 5] = i + 1;
            }

            return triangles;
        }
    }
}
