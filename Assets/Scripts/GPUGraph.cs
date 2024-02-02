using UnityEngine;
using UnityEngine.Rendering;
using static FunctionLibrary;

public class GraphGPU : MonoBehaviour   // C# script to manage a compute shader
{
    [SerializeField,Range(10,200)]
    int resolution=10;

    [SerializeField]
    FunctionLibrary.FunctonName function;


    private void Update()
    {
        Function f = GetFunction(function); 
        float time = Time.time;
        float step = 2f / resolution;
        float v = 0.5f * step - 1f;  


    }

}
