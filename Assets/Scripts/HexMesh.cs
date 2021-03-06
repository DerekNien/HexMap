using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    // Data
    private Mesh _hexMesh;
    private MeshCollider _meshCollider;
    private List<Vector3> _vertices;
    private List<int> _triangles;
    private List<Color> _colors;

    // Life Cycle
    private void Awake()
    {
        var meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = _hexMesh = new Mesh();
        _hexMesh.name = "HexMesh";
        _meshCollider = gameObject.AddComponent<MeshCollider>();
        _vertices = new List<Vector3>();
        _triangles = new List<int>();
        _colors = new List<Color>();
    }

    // Interfaces
    public void Triangulate(HexCell[] hexCells)
    {
        _vertices.Clear();
        _triangles.Clear();
        _colors.Clear();
        foreach (var hexCell in hexCells) Triangulate(hexCell);

        _hexMesh.vertices = _vertices.ToArray();
        _hexMesh.triangles = _triangles.ToArray();
        _hexMesh.colors = _colors.ToArray();
        _hexMesh.RecalculateNormals();
        _meshCollider.sharedMesh = _hexMesh;
    }

    // Methods
    private void Triangulate(HexCell hexCell)
    {
        var center = hexCell.transform.localPosition;
        for (var i = 0; i < 6; i++)
        {
            AddTriangle(center, center + HexMetrics.Corners[i], center + HexMetrics.Corners[i + 1]);
            AddTriangleColor(hexCell.color);
        }

    }

    private void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        var vertexIndex = _vertices.Count;
        _vertices.Add(v1);
        _vertices.Add(v2);
        _vertices.Add(v3);
        _triangles.Add(vertexIndex);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 2);
    }

    private void AddTriangleColor(Color color)
    {
        _colors.Add(color);
        _colors.Add(color);
        _colors.Add(color);
    }
}