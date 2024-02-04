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
        // The definition of the function ConfigureProcedural is bellow.
        #pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural  

        // turn on the synchronized shader compilation, wo that this shader will be compiled right before the 1st time we use it.
        #pragma editor_sync_compilation 
        #pragma target 4.5

        struct Input
        {
            float3 worldPos;
        };

        float _Smoothness;

        float _Step;

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

                unity_ObjectToWorld = 0.0;
				unity_ObjectToWorld._m03_m13_m23_m33 = float4(position, 1.0);   // translation
				unity_ObjectToWorld._m00_m11_m22 = _Step;   // scaling
                // BUT why? Model transformation should not include a scaling.
                // Does that mean we combined MVP transformations into one single matrix?
                // Is the scaling factor belong to "P"(perspective projection) matrix?
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
