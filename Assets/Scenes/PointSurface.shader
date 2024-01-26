Shader "Graph/Point Surface" 
{
    SubShader
    {
        CGPROGRAM
        #pragma surface ConfigureSurface Standard fullforwardshadows  // shader + func + lighting + shadow 
        #pragma target 3.0

        strut Input
        {
            float3 worldPos;
        };

        ENDCG
    }

    FallBack "Diffuse"
}
