using Unity.VisualScripting;
using UnityEngine;

public class FractalGPU : MonoBehaviour
{
    [SerializeField, Range(1, 8)]
    int depth = 4;

    [SerializeField]
    Mesh mesh;

    [SerializeField]
    Material material;

    FractalPart[][] parts;
    Matrix4x4[][] matrices;
    ComputeBuffer[] matricesBuffers;
    static readonly int matricesId = Shader.PropertyToID("_Matrices");  // This is for read data into shader for rendering
    static MaterialPropertyBlock propertyBlock;

    static Vector3[] directions = {
        Vector3.up, Vector3.right, Vector3.left, Vector3.forward, Vector3.back
    };

    static Quaternion[] rotations = {
        Quaternion.identity,
        Quaternion.Euler(0f, 0f, -90f), Quaternion.Euler(0f, 0f, 90f),
        Quaternion.Euler(90f, 0f, 0f), Quaternion.Euler(-90f, 0f, 0f)
    };

    struct FractalPart
    {
        public Vector3 direction, worldPosition;
        public Quaternion rotation, worldRotation;
        // 1. use this to store deltaRotation 
        // 2. and update the angle via ADD instead of MULTIPLICATION,
        //    to avoid quaternion float err accumulation and finally turns to invalid qauternion.
        public float spinAngle; 
    }

    FractalPart CreatePart(int childIndex) => new FractalPart
    {
        direction = directions[childIndex],
        rotation = rotations[childIndex]
    };

    // Change it to a function that will update whenever the object is enabled.
    // If we use Awake, this function will only be called once, at the beginning of the scene loading.
    //void Awake()
    void OnEnable() 
    {
        // S1. create a vector
        parts = new FractalPart[depth][ ];
        matrices = new Matrix4x4[depth][ ];
        matricesBuffers = new ComputeBuffer[depth];
        int stride = 16 * 4;
        for (int i = 0, length = 1; i < parts.Length; i++, length *= 5)
        {
            parts[i] = new FractalPart[length];
            matrices[i]= new Matrix4x4[length];
            matricesBuffers[i] = new ComputeBuffer(length, stride);    // we'll send data to GPU level-by-level
        }



        // S2. then init elements 
        parts[0][0]=CreatePart(0);       // root node is NOT a child of any node.
        for (int li = 1; li < parts.Length; li++)  // level idx
        {
            FractalPart[] levelParts = parts[li];   // take the init. memory blocks and fill it
            for (int fpi = 0; fpi < levelParts.Length; fpi+=5)  // first place idx
            {
                for (int ci = 0; ci < 5; ci++)
                {
                    levelParts[fpi+ci]=CreatePart(ci);
                }
            } 
        }



        //if (propertyBlock == null)
        //{
        //    propertyBlock = new MaterialPropertyBlock();
        //}
        propertyBlock ??= new MaterialPropertyBlock();  // a simplified version of above
    }

    // This function makes it possible, that we can change some params in inspector in play mode.
    private void OnValidate()
    {
        if (parts != null && enabled)
        {
            OnDisable();
            OnEnable();
        }
    }

    void OnDisable()
    {
        for (int i = 0; i < matricesBuffers.Length; i++)
        {
            matricesBuffers[i].Release();   // clear the buffer level-by-level
        }
        parts = null;
        matrices = null;
        matricesBuffers = null;
    }

    void Update()
    {
        float spinAngleDelta = 22.5f * Time.deltaTime;
        FractalPart rootPart = parts[0][0];

        rootPart.spinAngle += spinAngleDelta;
        rootPart.worldRotation = rootPart.rotation * Quaternion.Euler(0f, rootPart.spinAngle, 0f);
       

        parts[0][0] = rootPart;
        matrices[0][0] = Matrix4x4.TRS(
            rootPart.worldPosition, rootPart.worldRotation, Vector3.one
        );

        // Root node cannot rotate(actually it can rotate, but that will make no difference).
        // Thus we don't have to update root node.
        float scale = 1f;
        for (int li = 1; li < parts.Length; li++)
        {
            scale *= 0.5f;
            FractalPart[] levelParts = parts[li];
            FractalPart[] parentParts = parts[li - 1];
            Matrix4x4[] levelMatrices = matrices[li];
            for (int fpi = 0; fpi < levelParts.Length; fpi++)
            {
                FractalPart parent = parentParts[fpi / 5];
                FractalPart part = levelParts[fpi];
                
                rootPart.spinAngle += spinAngleDelta;
                part.worldRotation = parent.worldRotation *
                                     (part.rotation * Quaternion.Euler(0f, part.spinAngle, 0f));

                part.worldPosition =
                    parent.worldPosition +
                    parent.worldRotation * (1.5f * scale * part.direction);
                levelMatrices[fpi] = Matrix4x4.TRS(
                    part.worldPosition, part.worldRotation, scale * Vector3.one
                );
                levelParts[fpi] = part;
            }
        }





        var bounds = new Bounds(Vector3.zero, 3f * Vector3.one);
        // 1. Send data to GPU after finish the calculation
        // 2. And then call the function for procedural drawing
        for (int i = 0; i < matricesBuffers.Length; i++)
        {
            ComputeBuffer buffer = matricesBuffers[i];
            buffer.SetData(matrices[i]);
            //material.SetBuffer(matricesId, buffer);
            //Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, buffer.count);

            propertyBlock.SetBuffer(matricesId, buffer);
            Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, buffer.count, propertyBlock);
        }
    }
}
