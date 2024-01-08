using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class LakeNoise : MonoBehaviour
{
    public float power = 0.1f;
    public float scale = 10f;
    public float timeScale = 0.25f;

    private MeshFilter meshFilter;
    private Vector3[] originalVertices;
    private float xOffset;
    private float yOffset;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        originalVertices = meshFilter.mesh.vertices; // Store the original vertices
        xOffset = Random.Range(0f, 9999f); // Random offset for more natural look
        yOffset = Random.Range(0f, 9999f);
    }

    void Update()
    {
        CreateWave();
        xOffset += Time.deltaTime * timeScale;
        yOffset += Time.deltaTime * timeScale;
    }

    void CreateWave()
    {
        Vector3[] vertices = new Vector3[originalVertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = originalVertices[i];
            vertex.y += CalculateHeight(vertex.x, vertex.z) * power;
            vertices[i] = vertex;
        }

        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.RecalculateNormals(); // Update normals for proper lighting
    }

    float CalculateHeight(float x, float y)
    {
        float xCord = x * scale + xOffset;
        float yCord = y * scale + yOffset;

        return Mathf.PerlinNoise(xCord, yCord);
    }
}
