using UnityEngine;
using UnityEngine.Rendering;
using static FunctionLibrary;

public class GraphGPU : MonoBehaviour   // C# script to manage a compute shader
{
    const int maxResolution = 1000;

    [SerializeField,Range(10, maxResolution)]
    int resolution=10;

    [SerializeField]
    FunctionLibrary.FunctonName function;

    ComputeBuffer positionsBuffer;

    [SerializeField]
    ComputeShader computeShader;

    [SerializeField]
    Material material;

    [SerializeField]
    Mesh mesh;

    static readonly int
    positionsId = Shader.PropertyToID("_Positions"),
    resolutionId = Shader.PropertyToID("_Resolution"),
    stepId = Shader.PropertyToID("_Step"),
    timeId = Shader.PropertyToID("_Time");
    void UpdateFunctionOnGPU()
    {
        float step = 2f / resolution;
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);


        // following functions use var kernelIndex as param
        var kernelIndex = (int)function;

        computeShader.SetBuffer(kernelIndex, positionsId, positionsBuffer);

        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(kernelIndex, groups, groups, 1);
    }

    //private void Awake()
    //{
    //    positionsBuffer = new ComputeBuffer(resolution * resolution, 3 * 4);  // 3*4 ? Without any alignment to 16 ?
    //}

    void OnEnable()
    {
        // Because the ComputeBuffer cannot be resized, thus we allocate with max count of points,
        // in this way we can make compute shader update points according to different resolutions 
        // we changed in play mode via inspector.
        positionsBuffer = new ComputeBuffer(maxResolution * maxResolution, 3 * 4);
    }

    void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
    }


    private void Update()
    {
        // 1. update data for compute shader
        UpdateFunctionOnGPU();  


        // 2. update data for rendering, i.e. send data to the shader that the material below used.
        float step = 2f / resolution;
        material.SetBuffer(positionsId, positionsBuffer);
        material.SetFloat(stepId, step);
        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
        //Graphics.DrawMeshInstancedProcedural(mesh,0,material,bounds,positionsBuffer.count);
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, resolution * resolution);
    }

}
