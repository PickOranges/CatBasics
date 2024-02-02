Shader "Graph/Point Surface GPU" 
{
    Properties
    {
        _Smoothness("Smoothness", Range(0,1))=0.5
    }
    SubShader
    {
        CGPROGRAM
        #pragma surface ConfigureSurface Standard fullforwardshadows  // shader + func + lighting + shadow 

        // see Unity instancing_options for details
        // procedural:FunctionName is used for: 
        // To set up the instance data manually, add per-instance data to this function in the same way you would normally add per-instance data to a shader. Unity also calls this function at the beginning of a fragment shader if any of the fetched instance properties are included in the fragment shader.
        // This "procedural drawing" is similar to GPU Instancting, only you manually set the GPU function that will calculate the per-instance data for you,
        // instead of pass per-instance data from CPU side via C# script.
        #pragma instancing_options procedural:ConfigureProcedural
        #pragma target 4.5

        struct Input
        {
            float3 worldPos;
        };

        float _Smoothness;

        void ConfigureSurface (Input input, inout SurfaceOutputStandard surface) {
            surface.Smoothness = 0.5;
            surface.Albedo=input.worldPos*0.5+0.5;
        }

        ENDCG
    }

    FallBack "Diffuse"
}
