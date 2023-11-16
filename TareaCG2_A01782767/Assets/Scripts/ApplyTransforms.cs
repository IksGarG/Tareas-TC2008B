using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyTransforms : MonoBehaviour
{
    [SerializeField] Vector3 displacement;
    [SerializeField] float Angle;
    [SerializeField] float carAngle;
    [SerializeField] AXIS rotationAXIS;
    [SerializeField] private Transform[] wheelTransforms;

    // [SerializeField] float speed = 1f;

    Mesh mesh;
    Vector3[] baseVertices;
    Vector3[] newVertices;

    Mesh meshW1;
    Vector3[] baseVerticesW1;
    Vector3[] newVerticesW1;

    Mesh meshW2;
    Vector3[] baseVerticesW2;
    Vector3[] newVerticesW2;

    Mesh meshW3;
    Vector3[] baseVerticesW3;
    Vector3[] newVerticesW3;

    Mesh meshW4;
    Vector3[] baseVerticesW4;
    Vector3[] newVerticesW4;

    void Start()
    {
        mesh = GetComponentInChildren<MeshFilter>().mesh;
        baseVertices = mesh.vertices;

        newVertices = new Vector3[baseVertices.Length];
        baseVertices.CopyTo(newVertices, 0);

        for (int i = 0; i < wheelTransforms.Length; i++)
        {
            Mesh meshW = wheelTransforms[i].GetComponentInChildren<MeshFilter>().mesh;
            Vector3[] baseVerticesW = meshW.vertices;

            Vector3[] newVerticesW = new Vector3[baseVerticesW.Length];
            baseVerticesW.CopyTo(newVerticesW, 0);

            switch (i)
            {
                case 0:
                    meshW1 = meshW;
                    baseVerticesW1 = baseVerticesW;
                    newVerticesW1 = newVerticesW;
                    break;
                case 1:
                    meshW2 = meshW;
                    baseVerticesW2 = baseVerticesW;
                    newVerticesW2 = newVerticesW;
                    break;
                case 2:
                    meshW3 = meshW;
                    baseVerticesW3 = baseVerticesW;
                    newVerticesW3 = newVerticesW;
                    break;
                case 3:
                    meshW4 = meshW;
                    baseVerticesW4 = baseVerticesW;
                    newVerticesW4 = newVerticesW;
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        DoTransforms();
    }
    private void ApplyCompositeTransformToVertices(Vector3[] baseVertices, Vector3[] newVertices, Matrix4x4 compositeTransform)
    {
        for (int i = 0; i < newVertices.Length; i++)
        {
            Vector4 temp = new(baseVertices[i].x, baseVertices[i].y, baseVertices[i].z, 1);
            newVertices[i] = compositeTransform * temp;
        }
    }

    void DoTransforms()
    {

        // Car center
        // Mesh carMesh = GetComponentInChildren<MeshFilter>().mesh;
        // Vector3 sumOfVertices = Vector3.zero;

        // foreach (Vector3 vertex in carMesh.vertices) {
        //     sumOfVertices += vertex;
        // }

        // Vector3 carCenter = sumOfVertices / carMesh.vertexCount;
        // // center = transform.TransformPoint(center); // Convert local to world coordinates


        // float radians = carAngle * Mathf.Deg2Rad; // Convert degrees to radians
        // Vector3 forwardDirection = new(Mathf.Sin(radians), 0, Mathf.Cos(radians));

        // Vector3 displacement2 = forwardDirection * speed * Time.deltaTime;

        // Matrix4x4 toOrigin = HW_Transforms.TranslationMat(-carCenter.x, -carCenter.y, -carCenter.z);
        // Matrix4x4 fromOrigin = HW_Transforms.TranslationMat(carCenter.x + displacement2.x, 
        //                                                     carCenter.y + displacement2.y, 
        //                                                     carCenter.z + displacement2.z);

        // Wheel Inital Positions
        Vector3 wheel1Pos = new(-1.7f, 1f, -1.8f);
        Vector3 wheel2Pos = new(1.7f, 1f, 3.78f);
        Vector3 wheel3pos = new(1.7f, 1f, -1.8f);
        Vector3 wheel4pos = new(-1.7f, 1f, 3.78f);

        // Wheel Positions
        Matrix4x4 moveW1 = HW_Transforms.TranslationMat(wheel1Pos.x, wheel1Pos.y, wheel1Pos.z);
        Matrix4x4 moveW2 = HW_Transforms.TranslationMat(wheel2Pos.x, wheel2Pos.y, wheel2Pos.z);
        Matrix4x4 moveW3 = HW_Transforms.TranslationMat(wheel3pos.x, wheel3pos.y, wheel3pos.z);
        Matrix4x4 moveW4 = HW_Transforms.TranslationMat(wheel4pos.x, wheel4pos.y, wheel4pos.z);

        // Car Movement
        Matrix4x4 moveCar = HW_Transforms.TranslationMat(displacement.x * Time.time, 
                                                        displacement.y * Time.time, 
                                                        displacement.z * Time.time);

        Matrix4x4 rotateWheel = HW_Transforms.RotateMat(Angle * Time.time, AXIS.X);
        Matrix4x4 rotateCarMatrix = HW_Transforms.RotateMat(carAngle, rotationAXIS);


        Matrix4x4 composite = rotateCarMatrix * moveCar;
        Matrix4x4 compositeWheel1 =  rotateCarMatrix * moveW1 * moveCar * rotateWheel;
        Matrix4x4 compositeWheel2 =  rotateCarMatrix * moveW2 * moveCar * rotateWheel;
        Matrix4x4 compositeWheel4 =  rotateCarMatrix * moveW3 * moveCar * rotateWheel;
        Matrix4x4 compositeWheel3 =  rotateCarMatrix * moveW4 * moveCar * rotateWheel;

        // Aplicar la transformación compuesta a los vértices
        for (int i = 0; i < newVertices.Length; i++)
        {
            Vector4 temp = new(baseVertices[i].x, baseVertices[i].y, baseVertices[i].z, 1);
            newVertices[i] = composite * temp;
        }

        ApplyCompositeTransformToVertices(baseVerticesW1, newVerticesW1, compositeWheel1);
        ApplyCompositeTransformToVertices(baseVerticesW2, newVerticesW2, compositeWheel2);
        ApplyCompositeTransformToVertices(baseVerticesW3, newVerticesW3, compositeWheel3);
        ApplyCompositeTransformToVertices(baseVerticesW4, newVerticesW4, compositeWheel4);

        // Replace the vertices in the mesh
        mesh.vertices = newVertices;
        mesh.RecalculateNormals();

        // Replace the vertices in the mesh of wheel1
        meshW1.vertices = newVerticesW1;
        meshW1.RecalculateNormals();

        // Replace the vertices in the mesh of wheel2
        meshW2.vertices = newVerticesW2;
        meshW2.RecalculateNormals();

        // Replace the vertices in the mesh of wheel3
        meshW3.vertices = newVerticesW3;
        meshW3.RecalculateNormals();

        // Replace the vertices in the mesh of wheel4
        meshW4.vertices = newVerticesW4;
        meshW4.RecalculateNormals();

    }
}

