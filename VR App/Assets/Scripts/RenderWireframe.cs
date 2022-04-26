using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class RenderWireframe : MonoBehaviour
{

    ProBuilderMesh mesh;
    LineRenderer frameRenderer;

    internal bool isStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<ProBuilderMesh>();
        frameRenderer = GetComponent<LineRenderer>();
        Vertex[] vertices = mesh.GetVertices();
        Vector3[] positions = new Vector3[vertices.Length];
        for(int i = 0; i < vertices.Length; i++){
            positions[i] = transform.TransformPoint(vertices[i].position);
        }
        frameRenderer.positionCount = positions.Length;
        frameRenderer.SetPositions(positions);
        isStarted = true;
    }
}
