Shader "Graph/Point Surface GPU" 
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
        #pragma instancing_options procedural:ConfigureProcedural  // the def of the function ConfigureProcedural is bellow.
        #pragma target 4.5

        struct Input
        {
            float3 worldPos;
        };

        float _Smoothness;

        // ReadOnly, cannot be modified. This is for reading each box's position data from GPU,
        // which calculated by compute shader we wrote.
        // NOTE: This buffer will ONLY be read when the shader is for procedural drawing,
        // i.e. the shader with #pragma instancing_options procedural...
        #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)    
			StructuredBuffer<float3> _Positions;
		#endif
        
        // Same as above,
        // The code of this function will ONLY be included, when the shader is for procedural drawing,
        // i.e. the shader with #pragma instancing_options procedural...
        void ConfigureProcedural () {
            #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                float3 position = _Positions[unity_InstanceID];
		    #endif
        }

        void ConfigureSurface (Input input, inout SurfaceOutputStandard surface) {
            surface.Smoothness = 0.5;
            surface.Albedo=input.worldPos*0.5+0.5;
        }

        ENDCG
    }

    FallBack "Diffuse"
}
