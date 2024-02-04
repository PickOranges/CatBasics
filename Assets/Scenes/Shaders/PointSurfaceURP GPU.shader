Shader "Graph/Point Surface URP GPU" 
{
    Properties
    {
        _Smoothness("Smoothness", Range(0,1))=0.5
    }
    SubShader
    {
        CGPROGRAM
        //#pragma surface ConfigureSurface Standard fullforwardshadows  // shader + func + lighting + shadow 
        
        // "addshadow" is for a custom shadow pass, so that we can also call the function ConfigureProcedural on shadow pass.
        // Because by default this function will NOT be called by a default shadow pass.
        #pragma surface ConfigureSurface Standard fullforwardshadows addshadow 

        // see Unity instancing_options for details
        // procedural:FunctionName is used for: 
        // To set up the instance data manually, add per-instance data to this function in the same way you would normally add per-instance data to a shader. Unity also calls this function at the beginning of a fragment shader if any of the fetched instance properties are included in the fragment shader.
        // This "procedural drawing" is similar to GPU Instancting, only you manually set the GPU function that will calculate the per-instance data for you,
        // instead of pass per-instance data from CPU side via C# script.
        // The definition of the function ConfigureProcedural is bellow.
        #pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural  

        // turn on the synchronized shader compilation, wo that this shader will be compiled right before the 1st time we use it.
        #pragma editor_sync_compilation 
        #pragma target 4.5

        #include "PointURP_GPU.hlsl"

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
