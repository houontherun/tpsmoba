using UnityEngine;
using System.Collections.Generic;


namespace NavMeshEx
{
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class NavMeshObject : MonoBehaviour
{
    [HideInInspector]
    public bool editable;

    [HideInInspector]
    public List<Vector3> vertices = new List<Vector3>();//本地坐标

    [HideInInspector]
    public List<int> triangles = new List<int>();//三角形顶点索引

    [HideInInspector]
    public List<int> selecteds = new List<int>();//最多选中三个

    public Mesh mesh
    {
        get;
        private set;
    }

    public Color normalColor = Color.black, selectedColor = Color.white, outlineColor = Color.black;

    public void Awake()
    {
        mesh = new Mesh();
        mesh.name = "NavMeshObject Mesh";
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void OnDestroy()
    {
        GetComponent<MeshFilter>().mesh = null;
    }

    public void AddPoint(Vector3 point)
    {
        vertices.Add(transform.InverseTransformPoint(point));
        UpdateMesh();
    }

    public void RemovePointAt(int index)
    {
        vertices.RemoveAt(index);

        for (int i = 0; i < triangles.Count; i++)
        {
            if (triangles[i] == index)
            {
                triangles.RemoveRange(i - i % 3, 3);
                i = i - i % 3 - 1;
            }
        }

        for (int i = 0; i < triangles.Count; i++)
        {
            if (triangles[i] > index)
                triangles[i]--;
        }

        UpdateMesh();
    }

    public void PositionHandleAt(int index, Vector3 delta)
    {
        vertices[index] += delta;
        UpdateMesh();
    }

    public List<Vector3> TransformPoints()//转换为世界坐标
    {
        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i < vertices.Count; i++)
            points.Add(transform.TransformPoint(vertices[i]));

        return points;
    }

    public void AddTriangle(int[] indices)
    {
        for (int i = 0; i < triangles.Count / 3; i++)
        {
            if (triangles[i * 3] == indices[0] && triangles[i * 3 + 1] == indices[1] && triangles[i * 3 + 2] == indices[2])
                return;
        }

        triangles.Add(indices[0]);
        triangles.Add(indices[1]);
        triangles.Add(indices[2]);

        triangles.Add(indices[1]);
        triangles.Add(indices[0]);
        triangles.Add(indices[2]);

        UpdateMesh();
    }

    public void UpdateMesh()
    {
        mesh.triangles = null;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
    }
}
}