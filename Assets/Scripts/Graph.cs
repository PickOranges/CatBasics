using UnityEngine;
using UnityEngine.Rendering;
using static FunctionLibrary;

public class Graph : MonoBehaviour
{
    [SerializeField]
    Transform pointPrefab;

    [SerializeField,Range(10,200)]
    int resolution=10;

    Transform[] points;

    [SerializeField]
    FunctionLibrary.FunctonName function;

    private void Awake()
    {
        float step = 2f / resolution;
        var scale = Vector3.one*step;
        points=new Transform[resolution * resolution];
        for (int i=0; i<points.Length; ++i)
        {
            Transform point=points[i]=Instantiate(pointPrefab);

            point.localScale = scale;
            point.SetParent(transform,false);  // transform: the TF that attached to this gameobject(i.e. Controller)
        }
    }

    private void Update()
    {
        Function f = GetFunction(function); // function pointer/lambda/delegate
        float time = Time.time;
        float step = 2f / resolution;
        float v = 0.5f * step - 1f;  // init value: z=0
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            if (x == resolution)
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) * step - 1f;  // we only update v when z is changed.
            }
            float u = (x + 0.5f) * step - 1f;  
            points[i].localPosition = f(u, v, time);
        }

        // Why not 2-folded loop?
        // Because then outer loop with x, and inner with z,
        // it is NOT convenient when we change the resolution.
        // Because that will change the number of points in total.
        // Thus we loop exact the same #points everytime, even with the change of resolution.
    }

}
