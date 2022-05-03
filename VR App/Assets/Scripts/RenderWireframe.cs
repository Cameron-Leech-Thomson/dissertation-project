using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using TMPro;

public class RenderWireframe : MonoBehaviour
{

    public bool redShift = true;
    ProBuilderMesh mesh;
    LineRenderer frameRenderer;

    internal bool isStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<ProBuilderMesh>();
        frameRenderer = GetComponent<LineRenderer>();
        if (redShift){
            frameRenderer.material = Resources.Load("WireframeMat") as Material;
        } else{
            frameRenderer.material = Resources.Load("BlueWireframeMat") as Material;
        }

        IList<Vector3> vertices = mesh.positions;
        Vector3[] positions = new Vector3[vertices.Count];
        for(int i = 0; i < vertices.Count; i++){
            positions[i] = transform.TransformPoint(vertices[i]);
        }

        frameRenderer.positionCount = positions.Length;
        frameRenderer.SetPositions(positions);

        isStarted = true;
    }
}
